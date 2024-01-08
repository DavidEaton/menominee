﻿using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Menominee.Domain.BaseClasses.Entity;

namespace Menominee.Domain.Entities.RepairOrders
{
    // TODO: DDD: Rename this class to Ticket?
    public class RepairOrder : Entity
    {
        // TODO: Make these validation values available to user as configurable application settings:
        public static readonly double MaximumValue = 99999.00;
        public static readonly long MinimumNumberValue = 1;
        public static readonly string InvalidValueMessage = $"Value(s) must be under {MaximumValue}.";
        public static readonly string InvalidNumberMessage = $"Invoice and Repair Order numbers can't be under  {MinimumNumberValue}.";
        public static readonly string RequiredMessage = $"Please include all required items.";
        public static readonly string DateInvalidMessage = $"Date cannot be in the future.";
        public static readonly int AccountingDateGracePeriodInDays = 10;
        public static readonly string AccountingDateInvalidMessage = $"Date cannot be >{AccountingDateGracePeriodInDays} days in the future.";
        public static readonly string InvalidStatusMessage = $"Invalid operation for the current Status";

        public long RepairOrderNumber { get; private set; } // Only populated when saving as a status less than Invoice.  Number generated by us.  Current format YYYYMMDDNNN, where NNN starts at 001. -Al
        public long InvoiceNumber { get; private set; } // Only populated when saving as a status of Invoice or higher.  Number is incremented each time but user will have ability to set the next number. It can have whatever starting number they choose. The next number is stored somewhere and we just increment it each time. They can edit what the next invoice number is, then we keep incrementing it until they decide the next number is something else.  We have allowed them to set the next invoice number backwards which could be useful in cases where they were trying to clean up some kind of mess.  We haven't cared if they reuse a number.-Al

        // TODO: Both customer and vehicle can be null at the beginning of the repair order process. They may not have all the info they need before saving it. The user will be able to configure the point in the process at which they will be required. -AL

        public Customer Customer { get; private set; }
        public Vehicle Vehicle { get; private set; }
        public double PartsTotal => Services.Select(
            service => service.PartsTotal).Sum();
        public double LaborTotal => Services.Select(
            service => service.LaborTotal).Sum();
        public double DiscountTotal => Services.Select(
            service => service.DiscountTotal).Sum();
        public double TaxTotal => Taxes.Select(
            tax => tax.PartTax.Amount + tax.LaborTax.Amount).Sum();
        public double ServicesTaxTotal => Services.Select(
            service => service.ServiceTaxTotal).Sum();
        public double ExciseFeesTotal => Services.Select(
            service => service.ExciseFeesTotal).Sum();
        public double ShopSuppliesTotal => Services.Select(
            service => service.ShopSuppliesTotal).Sum();
        public double Total => Services.Select(
            service => service.Total).Sum();
        public double TotalWithTax => Total + TaxTotal;
        public DateTime DateCreated { get; private set; } // Only set once ever: on object creation
        public DateTime DateModified { get; private set; } // Set every time it's saved
        public DateTime AccountingDate { get; private set; } // Users can select which date a given day's sales are reported under. Must be within user-configurable AccountingDateGracePeriodInDays.
        public DateTime? DateInvoiced =>
            statuses
                .Where(repairOrderStatus => repairOrderStatus.Status == Status.Invoiced)
                .OrderBy(repairOrderStatus => repairOrderStatus.Date)
                .Select(repairOrderStatus => repairOrderStatus.Date)
                .FirstOrDefault();
        public Status Status =>
            statuses.OrderByDescending(repairOrderStatus => repairOrderStatus.Date)
                    .Select(repairOrderStatus => repairOrderStatus.Status)
                    .FirstOrDefault();

        private readonly List<RepairOrderStatus> statuses = new();
        public IReadOnlyList<RepairOrderStatus> Statuses => statuses.ToList();

        private readonly List<RepairOrderService> services = new();
        public IReadOnlyList<RepairOrderService> Services => services.ToList();

        private readonly List<RepairOrderTax> taxes = new();
        public IReadOnlyList<RepairOrderTax> Taxes => taxes.ToList();

        private readonly List<RepairOrderPayment> payments = new();
        public IReadOnlyList<RepairOrderPayment> Payments => payments.ToList();

        private RepairOrder(
            Customer customer,
            Vehicle vehicle,
            DateTime accountingDate,
            long nextRepairOrderNumber,
            long nextInvoiceNumber,
            List<RepairOrderStatus> statuses,
            List<RepairOrderService> services,
            List<RepairOrderTax> taxes,
            List<RepairOrderPayment> payments)
        {
            Customer = customer;
            RepairOrderNumber = nextRepairOrderNumber;
            InvoiceNumber = nextInvoiceNumber;
            Vehicle = vehicle;
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
            AccountingDate = accountingDate;
            this.statuses = (statuses == null || statuses.Count == 0) ? InitialStatus() : statuses;
            this.services = services ?? new List<RepairOrderService>();
            this.taxes = taxes ?? new List<RepairOrderTax>();
            this.payments = payments ?? new List<RepairOrderPayment>();
        }

        public static Result<RepairOrder> Create(
            Customer customer,
            Vehicle vehicle,
            DateTime accountingDate,
            IReadOnlyList<long> repairOrderNumbers,
            long lastInvoiceNumber,
            List<RepairOrderStatus> statuses = null,
            List<RepairOrderService> services = null,
            List<RepairOrderTax> taxes = null,
            List<RepairOrderPayment> payments = null)
        {
            // TODO: The user will be able to configure the point in the process at which they (customer, vehicle) will be required. -AL
            //if (customer is null || vehicle is null)
            //    return Result.Failure<RepairOrder>(RequiredMessage);

            if (accountingDate > DateTime.Today.AddDays(AccountingDateGracePeriodInDays))
                return Result.Failure<RepairOrder>(AccountingDateInvalidMessage);

            var nextRepairOrderNumber = CalculateNextRepairOrderNumber(repairOrderNumbers, DateTime.Today);
            var nextInvoiceNumber = CalculateNextInvoiceNumber(lastInvoiceNumber, statuses);

            return Result.Success(new RepairOrder(customer, vehicle, accountingDate, nextRepairOrderNumber, nextInvoiceNumber, statuses, services, taxes, payments));
        }

        public Result UpdatePayments(IReadOnlyList<RepairOrderPayment> payments)
        {
            var toAdd = payments
                .Where(payment => payment.Id == 0)
                .ToList();

            var toDelete = Payments
                .Where(payment => !payments.Any(callerPayment => callerPayment.Id == payment.Id))
                .ToList();

            var toModify = Payments
                .Where(payment => payments.Any(callerPayment => callerPayment.Id == payment.Id))
                .ToList();

            var addResults = toAdd
                .Select(payment => AddPayment(payment))
                .ToList();

            var deleteResults = toDelete
                .Select(payment => RemovePayment(payment))
                .ToList();

            foreach (var payment in toModify)
            {
                var paymentFromCaller = payments.Single(callerPayment => callerPayment.Id == payment.Id);

                if (payment.PaymentMethod != paymentFromCaller.PaymentMethod)
                {
                    var result = payment.SetPaymentMethod(paymentFromCaller.PaymentMethod);
                    if (result.IsFailure)
                        return Result.Failure(result.Error);
                }

                if (payment.Amount != paymentFromCaller.Amount)
                {
                    var result = payment.SetAmount(paymentFromCaller.Amount);
                    if (result.IsFailure)
                        return Result.Failure(result.Error);
                }
            }

            return Result.Success();
        }

        public Result UpdateTaxes(IReadOnlyList<RepairOrderTax> taxes)
        {
            var toAdd = taxes
                .Where(tax => tax.Id == 0)
                .ToList();

            var toDelete = Taxes
                .Where(tax => !taxes.Any(callerTax => callerTax.Id == tax.Id))
                .ToList();

            var toModify = Taxes
                .Where(tax => taxes.Any(callerTax => callerTax.Id == tax.Id))
                .ToList();

            var addResults = toAdd
                .Select(tax => AddTax(tax))
                .ToList();

            var deleteResults = toDelete
                .Select(tax => RemoveTax(tax))
                .ToList();

            foreach (var tax in toModify)
            {
                var taxFromCaller = taxes.Single(callerTax => callerTax.Id == tax.Id);

                if (tax.LaborTax != taxFromCaller.LaborTax)
                {
                    var result = tax.SetLaborTax(taxFromCaller.LaborTax);
                    if (result.IsFailure)
                        return Result.Failure(result.Error);
                }

                if (tax.PartTax != taxFromCaller.PartTax)
                {
                    var result = tax.SetPartTax(taxFromCaller.PartTax);
                    if (result.IsFailure)
                        return Result.Failure(result.Error);
                }
            }

            return Result.Success();
        }

        public Result UpdateServices(List<RepairOrderService> services)
        {
            var toAdd = services
                .Where(service => service.Id == 0)
                .ToList();

            var toDelete = Services
                .Where(service => !services.Any(callerService => callerService.Id == service.Id))
                .ToList();

            var toModify = Services
                .Where(service => services.Any(callerService => callerService.Id == service.Id))
                .ToList();

            var addResults = toAdd
                .Select(service => AddService(service))
                .ToList();

            var deleteResults = toDelete
                .Select(service => RemoveService(service))
                .ToList();

            foreach (var service in toModify)
            {
                var serviceFromCaller = services.Single(callerService => callerService.Id == service.Id);

                if (service.SaleCode != serviceFromCaller.SaleCode)
                {
                    var result = service.SetSaleCode(serviceFromCaller.SaleCode);
                    if (result.IsFailure)
                        return Result.Failure(result.Error);
                }

                if (service.ServiceName != serviceFromCaller.ServiceName)
                {
                    var result = service.SetServiceName(serviceFromCaller.ServiceName);
                    if (result.IsFailure)
                        return Result.Failure(result.Error);
                }
            }

            return Result.Success();
        }

        public Result UpdateStatuses(IReadOnlyList<RepairOrderStatus> statuses, List<long> repairOrderNumbers)
        {
            var toAdd = statuses
                .Where(status => status.Id == 0)
                .ToList();

            var toDelete = Statuses
                .Where(status => !statuses.Any(callerStatus => callerStatus.Id == status.Id))
                .ToList();

            var toModify = Statuses
                .Where(status => statuses.Any(callerStatus => callerStatus.Id == status.Id))
                .ToList();

            var addResults = toAdd
                .Select(status => AddStatus(status, repairOrderNumbers))
                .ToList();

            var deleteResults = toDelete
                .Select(status => RemoveStatus(status))
                .ToList();


            foreach (var status in toModify)
            {
                var statusFromCaller = statuses.Single(callerStatus => callerStatus.Id == status.Id);

                if (status.Description != statusFromCaller.Description)
                {
                    var result = status.SetDescription(statusFromCaller.Description);
                    if (result.IsFailure)
                        return Result.Failure(result.Error);
                }

                if (status.Status != statusFromCaller.Status)
                {
                    var result = status.SetStatus(statusFromCaller.Status);
                    if (result.IsFailure)
                        return Result.Failure(result.Error);
                }
            }

            return Result.Success();
        }

        private static long CalculateNextRepairOrderNumber(IReadOnlyList<long> repairOrderNumbers, DateTime currentDate)
        {
            var today = long.Parse(currentDate.ToString("yyyyMMdd"));

            if (repairOrderNumbers.Count == 0)
                return (today * 1000) + 1;

            var maxRepairOrderNumber = repairOrderNumbers.Max();
            var lastThreeDigits = (maxRepairOrderNumber % 1000) + 1;

            if (lastThreeDigits >= 1000)
                lastThreeDigits = 1;

            return (today * 1000) + lastThreeDigits;
        }

        private static long CalculateNextInvoiceNumber(long lastInvoiceNumber, List<RepairOrderStatus> statuses)
        {
            return statuses?.Count == 0 || statuses?.Max(status => status.Status) <= Status.Invoiced ? lastInvoiceNumber : lastInvoiceNumber++;
        }

        private static List<RepairOrderStatus> InitialStatus()
        {
            // If caller didn't include an initial status, create one
            return new List<RepairOrderStatus>()
            {
                RepairOrderStatus.Create(Status.New, "New Repair Order").Value
            };
        }


        public Result<Customer> SetCustomer(Customer customer)
        {
            if (customer is null)
                return Result.Failure<Customer>(RequiredMessage);

            DateModified = DateTime.Today;
            return Result.Success(Customer = customer);
        }

        public Result<Vehicle> SetVehicle(Vehicle vehicle)
        {
            if (vehicle is null)
                return Result.Failure<Vehicle>(RequiredMessage);

            DateModified = DateTime.Today;
            return Result.Success(Vehicle = vehicle);
        }

        public Result<long> SetInvoiceNumber(long invoiceNumber)
        {
            if (invoiceNumber < MinimumNumberValue)
                return Result.Failure<long>(InvalidNumberMessage);

            DateModified = DateTime.Today;
            return Result.Success(InvoiceNumber = invoiceNumber);
        }

        public Result<long> SetRepairOrderNumber(List<long> repairOrderNumbers, DateTime dateModified)
        {
            var nextRepairOrderNumber = CalculateNextRepairOrderNumber(repairOrderNumbers, dateModified);

            DateModified = dateModified;
            return Result.Success(RepairOrderNumber = nextRepairOrderNumber);
        }

        public Result SetRepairOrderNumber(long repairOrderNumber, DateTime dateModified)
        {
            if (repairOrderNumber < MinimumNumberValue)
                return Result.Failure<long>(InvalidNumberMessage);

            DateModified = dateModified;
            return Result.Success(RepairOrderNumber = repairOrderNumber);
        }

        public Result<DateTime> SetDateModified(DateTime dateModified)
        {
            if (dateModified > DateTime.Today)
                return Result.Failure<DateTime>(DateInvalidMessage);

            return Result.Success(DateModified = dateModified);
        }

        public Result<DateTime> SetAccountingDate(DateTime accountingDate)
        {
            if (accountingDate > DateTime.Today.AddDays(AccountingDateGracePeriodInDays))
                return Result.Failure<DateTime>(AccountingDateInvalidMessage);

            DateModified = DateTime.Today;
            return Result.Success(AccountingDate = accountingDate);
        }

        public Result<RepairOrderStatus> AddStatus(RepairOrderStatus status, List<long> repairOrderNumbers)
        {
            if (status is null)
                return Result.Failure<RepairOrderStatus>(RequiredMessage);

            repairOrderNumbers ??= new List<long>();

            statuses.Add(status);

            if (status.Status <= Status.Invoiced)
                RepairOrderNumber = RepairOrderNumber > 0 ? RepairOrderNumber : CalculateNextRepairOrderNumber(repairOrderNumbers, DateTime.Today);

            if (status.Status > Status.Invoiced)
                InvoiceNumber = InvoiceNumber > 0 ? InvoiceNumber : CalculateNextInvoiceNumber(0, statuses);

            DateModified = DateTime.Today;
            return Result.Success(status);
        }

        public Result<RepairOrderStatus> RemoveStatus(RepairOrderStatus status)
        {
            if (status is null)
                return Result.Failure<RepairOrderStatus>(RequiredMessage);

            var isStatusRemoved = statuses.Remove(status);

            if (isStatusRemoved)
            {
                DateModified = DateTime.Today;
                return Result.Success(status);
            }
            else
                return Result.Failure<RepairOrderStatus>(RequiredMessage);
        }

        public Result<RepairOrderService> AddService(RepairOrderService service)
        {
            if (service is null)
                return Result.Failure<RepairOrderService>(RequiredMessage);

            services.Add(service);

            DateModified = DateTime.Today;
            return Result.Success(service);
        }

        public Result<RepairOrderService> RemoveService(RepairOrderService service)
        {
            if (service is null)
                return Result.Failure<RepairOrderService>(RequiredMessage);

            var isServiceRemoved = services.Remove(service);

            if (isServiceRemoved)
            {
                DateModified = DateTime.Today;
                return Result.Success(service);
            }
            else
                return Result.Failure<RepairOrderService>(RequiredMessage);
        }

        public Result<RepairOrderTax> AddTax(RepairOrderTax tax)
        {
            if (tax is null)
                return Result.Failure<RepairOrderTax>(RequiredMessage);

            taxes.Add(tax);

            DateModified = DateTime.Today;
            return Result.Success(tax);
        }

        public Result<RepairOrderTax> RemoveTax(RepairOrderTax tax)
        {
            if (tax is null)
                return Result.Failure<RepairOrderTax>(RequiredMessage);

            var isTaxRemoved = taxes.Remove(tax);

            if (isTaxRemoved)
            {
                DateModified = DateTime.Today;
                return Result.Success(tax);
            }
            else
                return Result.Failure<RepairOrderTax>(RequiredMessage);
        }

        public Result<RepairOrderPayment> AddPayment(RepairOrderPayment payment)
        {
            if (payment is null)
                return Result.Failure<RepairOrderPayment>(RequiredMessage);

            payments.Add(payment);

            DateModified = DateTime.Today;
            return Result.Success(payment);
        }

        public Result<RepairOrderPayment> RemovePayment(RepairOrderPayment payment)
        {
            if (payment is null)
                return Result.Failure<RepairOrderPayment>(RequiredMessage);

            var isPaymentRemoved = payments.Remove(payment);

            if (isPaymentRemoved)
            {
                DateModified = DateTime.Today;
                return Result.Success(payment);
            }
            else
                return Result.Failure<RepairOrderPayment>(RequiredMessage);
        }

        #region ORM

        // EF requires a parameterless constructor
        protected RepairOrder()
        {
            statuses = new List<RepairOrderStatus>();
            services = new List<RepairOrderService>();
            taxes = new List<RepairOrderTax>();
            payments = new List<RepairOrderPayment>();
        }

        #endregion
    }
}

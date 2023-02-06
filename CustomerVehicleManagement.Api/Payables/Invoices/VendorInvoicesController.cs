﻿using CSharpFunctionalExtensions;
using CustomerVehicleManagement.Api.Data;
using CustomerVehicleManagement.Api.Manufacturers;
using CustomerVehicleManagement.Api.Payables.PaymentMethods;
using CustomerVehicleManagement.Api.Payables.Vendors;
using CustomerVehicleManagement.Api.SaleCodes;
using CustomerVehicleManagement.Api.Taxes;
using CustomerVehicleManagement.Domain.Entities;
using CustomerVehicleManagement.Domain.Entities.Inventory;
using CustomerVehicleManagement.Domain.Entities.Payables;
using CustomerVehicleManagement.Shared.Models.Payables.Invoices;
using CustomerVehicleManagement.Shared.Models.Payables.Invoices.LineItems;
using CustomerVehicleManagement.Shared.Models.Payables.Invoices.LineItems.Items;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerVehicleManagement.Api.Payables.Invoices
{
    public class VendorInvoicesController : ApplicationController
    {
        private readonly IVendorInvoiceRepository repository;
        private readonly IVendorRepository vendorRepository;
        private readonly IVendorInvoicePaymentMethodRepository paymentMethodRepository;
        private readonly ISalesTaxRepository salesTaxRepository;
        private readonly IManufacturerRepository manufacturerRepository;
        private readonly ISaleCodeRepository saleCodeRepository;
        private readonly string BasePath = "/api/vendorinvoices";

        public VendorInvoicesController(
            IVendorInvoiceRepository repository,
            IVendorRepository vendorRepository,
            IVendorInvoicePaymentMethodRepository paymentMethodRepository,
            ISalesTaxRepository salesTaxRepository,
            IManufacturerRepository manufacturerRepository,
            ISaleCodeRepository saleCodeRepository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.vendorRepository = vendorRepository ?? throw new ArgumentNullException(nameof(vendorRepository)); ;
            this.paymentMethodRepository = paymentMethodRepository ?? throw new ArgumentNullException(nameof(paymentMethodRepository));
            this.salesTaxRepository = salesTaxRepository ?? throw new ArgumentNullException(nameof(salesTaxRepository));
            this.manufacturerRepository = manufacturerRepository ?? throw new ArgumentNullException(nameof(manufacturerRepository));
            this.saleCodeRepository = saleCodeRepository ?? throw new ArgumentNullException(nameof(saleCodeRepository));
        }

        [Route("listing")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<VendorInvoiceToReadInList>>> GetInvoiceListAsync([FromQuery] ResourceParameters resourceParameters)
        {
            var result = await repository.GetInvoiceListAsync(resourceParameters);

            return result is null
                ? NotFound()
                : Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<VendorInvoiceToRead>>> GetInvoices([FromQuery] ResourceParameters resourceParameters)
        {
            var result = await repository.GetInvoices(resourceParameters);

            return result is null
                ? NotFound()
                : Ok(result);
        }

        [HttpGet("{id:long}", Name = "GetInvoiceAsync")]
        public async Task<ActionResult<VendorInvoiceToRead>> GetInvoiceAsync(long id)
        {
            var result = await repository.GetInvoiceAsync(id);

            return result is null
                ? NotFound()
                : Ok(result);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateInvoiceAsync(long id, VendorInvoiceToWrite invoiceFromCaller)
        {
            var invoiceFromRepository = await repository.GetInvoiceEntityAsync(id);

            if (invoiceFromRepository is null)
                return NotFound($"Could not find Vendor Invoice to update with Id = {id}.");

            // Getting the following limited entity lists keeps all objects tracked
            // by the same db context instance, preventing spurious updates to unmodified
            // related entities.
            // Create entity lists, limited to those contained in invoiceFromCaller:
            IReadOnlyList<Manufacturer> manufacturers = await GetManufacturersInInvoice(invoiceFromCaller);
            IReadOnlyList<SaleCode> salesCodes = await GetSaleCodesInInvoice(invoiceFromCaller);
            IReadOnlyList<Vendor> vendors = await GetVendorsInInvoice(invoiceFromCaller);

            // TODO: VK: Since FluentValidation runs all doimain validations before request
            // arrives here at the Controller, are we coding all of these Result.IsFailure()
            // checks for nothing? Is it redundant code that will never get executed?
            // Are we returning the correct error codes?

            // Update each member of VendorInvoice, returning a Bad Resuest response
            // in case of Failure.
            // Aggregate root entity VendorInvoice:
            if (invoiceFromRepository.Vendor.Id != invoiceFromCaller.Vendor.Id)
                if (invoiceFromRepository.SetVendor(
                    vendors.FirstOrDefault(
                        vendor => vendor.Id == invoiceFromCaller.Vendor.Id))
                        .IsFailure)
                    return BadRequest(/* TODO: Return Fesult.Error */);

            if (invoiceFromRepository.Status != invoiceFromCaller.Status)
                if (invoiceFromRepository.SetVendorInvoiceStatus(invoiceFromCaller.Status).IsFailure)
                    return BadRequest(/* TODO: Return Fesult.Error */);

            if (invoiceFromRepository.Total != invoiceFromCaller.Total)
                if (invoiceFromRepository.SetTotal(invoiceFromCaller.Total).IsFailure)
                    return BadRequest(/* TODO: Return Fesult.Error */);

            if (invoiceFromRepository.InvoiceNumber != invoiceFromCaller.InvoiceNumber)
            {
                var vendorInvoiceNumbers = await repository.GetVendorInvoiceNumbers(invoiceFromCaller.Vendor.Id);

                if (invoiceFromRepository.SetInvoiceNumber(invoiceFromCaller.InvoiceNumber, vendorInvoiceNumbers).IsFailure)
                    return BadRequest(/* TODO: Return Fesult.Error */);
            }

            if (invoiceFromCaller.Date is not null)
            {
                if (invoiceFromRepository.Date != invoiceFromCaller.Date)
                    if (invoiceFromRepository.SetDate(invoiceFromCaller.Date).IsFailure)
                        return BadRequest(/* TODO: Return Fesult.Error */);
            }

            if (invoiceFromCaller.Date is null && invoiceFromRepository.Date is not null)
                invoiceFromRepository.ClearDate();

            if (invoiceFromCaller.DatePosted is not null)
            {
                if (invoiceFromRepository.DatePosted != invoiceFromCaller.DatePosted)
                    if (invoiceFromRepository.SetDatePosted(invoiceFromCaller.DatePosted).IsFailure)
                        return BadRequest(/* TODO: Return Fesult.Error */);
            }

            if (invoiceFromCaller.DatePosted is null && invoiceFromRepository.DatePosted is not null)
                invoiceFromRepository.ClearDatePosted();

            // Line Items
            foreach (var lineItemFromCaller in invoiceFromCaller?.LineItems)
            {
                // Added
                if (lineItemFromCaller.Id == 0)
                    invoiceFromRepository.AddLineItem(
                        VendorInvoiceLineItem.Create(
                            lineItemFromCaller.Type,
                            item: VendorInvoiceItemHelper.ConvertWriteDtoToEntity(
                                lineItemFromCaller.Item,
                                manufacturers,
                                salesCodes),
                            lineItemFromCaller.Quantity,
                            lineItemFromCaller.Cost,
                            lineItemFromCaller.Core,
                            lineItemFromCaller.PONumber,
                            lineItemFromCaller.TransactionDate)
                        .Value);
                // Updated
                if (lineItemFromCaller.Id != 0)
                {
                    var lineItemFromRepository = invoiceFromRepository?.LineItems.FirstOrDefault(
                        contextLineItem =>
                        contextLineItem.Id == lineItemFromCaller.Id);

                    if (lineItemFromRepository.Type != lineItemFromCaller.Type)
                        lineItemFromRepository.SetType(lineItemFromCaller.Type);

                    // Each VendorInvoiceLineItem (in LineItems) is an Entity that
                    // contains the VendorInvoiceItem Value Object.
                    // VendorInvoiceItem (LineItem.Item) is a Value Object (no Id)
                    // that contains the Item (aka Part).
                    if (ItemHasEdits(lineItemFromCaller, lineItemFromRepository))
                        lineItemFromRepository.SetItem(VendorInvoiceItemHelper.ConvertWriteDtoToEntity(
                            lineItemFromCaller.Item,
                            manufacturers,
                            salesCodes));

                    if (lineItemFromRepository.Quantity != lineItemFromCaller.Quantity)
                        lineItemFromRepository.SetQuantity(lineItemFromCaller.Quantity);

                    if (lineItemFromRepository.Cost != lineItemFromCaller.Cost)
                        lineItemFromRepository.SetCost(lineItemFromCaller.Cost);

                    if (lineItemFromRepository.Core != lineItemFromCaller.Core)
                        lineItemFromRepository.SetCore(lineItemFromCaller.Core);

                    if (lineItemFromRepository.PONumber != lineItemFromCaller.PONumber)
                        lineItemFromRepository.SetPONumber(lineItemFromCaller.PONumber);

                    if (lineItemFromRepository.TransactionDate is not null)
                        if (lineItemFromRepository.TransactionDate != lineItemFromCaller.TransactionDate)
                            lineItemFromRepository.SetTransactionDate(lineItemFromCaller.TransactionDate);
                }
                // TODO: Deleted
            }
            // Payments
            foreach (var paymentFromCaller in invoiceFromCaller?.Payments)
            {
                // Added
                if (paymentFromCaller.Id == 0)
                    invoiceFromRepository.AddPayment(
                        VendorInvoicePayment.Create(
                            await paymentMethodRepository.GetPaymentMethodEntityAsync(
                                paymentFromCaller.PaymentMethod.Id), paymentFromCaller.Amount)
                        .Value);
                // Updated
                if (paymentFromCaller.Id != 0)
                {
                    var paymentFromRepository = invoiceFromRepository?.Payments.FirstOrDefault(
                        paymentFromRepository =>
                        paymentFromRepository.Id == paymentFromCaller.Id);

                    if (paymentFromRepository.PaymentMethod.Id != paymentFromCaller.PaymentMethod.Id)
                        paymentFromRepository.SetPaymentMethod(
                            await paymentMethodRepository.GetPaymentMethodEntityAsync(paymentFromCaller.PaymentMethod.Id));

                    if (paymentFromRepository.Amount != paymentFromCaller.Amount)
                        paymentFromRepository.SetAmount(paymentFromCaller.Amount);
                }
                // TODO: Deleted
            }
            // Taxes
            foreach (var taxFromCaller in invoiceFromCaller?.Taxes)
            {
                // Don't save new, zero amount taxes sent by client
                if (taxFromCaller.Amount == 0 && taxFromCaller.Id == 0)
                    continue;

                // Delete existing tax with edited amount == 0 sent by client
                if (taxFromCaller.Amount == 0 && taxFromCaller.Id != 0)
                {
                    var taxToRemove = invoiceFromRepository?.Taxes.FirstOrDefault(
                        taxFromRepository =>
                        taxFromRepository.Id == taxFromCaller.Id);

                    invoiceFromRepository.RemoveTax(taxToRemove);
                    continue;
                }

                // Added
                if (taxFromCaller.Id == 0)
                    invoiceFromRepository.AddTax(
                        VendorInvoiceTax.Create(
                            await salesTaxRepository.GetSalesTaxEntityAsync(taxFromCaller.SalesTax.Id), taxFromCaller.Amount).Value);
                // Updated
                if (taxFromCaller.Id != 0)
                {
                    var taxFromRepository = invoiceFromRepository?.Taxes.FirstOrDefault(
                        taxFromRepository =>
                        taxFromRepository.Id == taxFromCaller.Id);

                    if (taxFromRepository.SalesTax.Id != taxFromCaller.SalesTax.Id)
                        taxFromRepository.SetSalesTax(await salesTaxRepository.GetSalesTaxEntityAsync(taxFromCaller.SalesTax.Id));

                    if (taxFromRepository.Amount != taxFromCaller.Amount)
                        taxFromRepository.SetAmount(taxFromCaller.Amount);
                }
                // TODO: Deleted
            }

            await repository.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/vendorinvoices
        [HttpPost]
        public async Task<ActionResult> AddInvoiceAsync(VendorInvoiceToWrite invoiceToAdd)
        {
            // TODO: How do we return the domain class error message?
            // TODO: Settle on error handling/messaging/logging
            var vendor = await vendorRepository.GetVendorEntityAsync(invoiceToAdd.Vendor.Id);

            if (vendor is null)
                return NotFound($"Could not add new Invoice, Number: {invoiceToAdd.InvoiceNumber}. Vendor '{invoiceToAdd.Vendor.Name}' not found.");

            var invoiceOrError = VendorInvoice.Create(
                vendor,
                invoiceToAdd.Status,
                invoiceToAdd.DocumentType,
                invoiceToAdd.Total,
                await repository.GetVendorInvoiceNumbers(vendor.Id),
                invoiceToAdd.InvoiceNumber,
                invoiceToAdd.Date,
                invoiceToAdd.DatePosted);

            if (invoiceOrError.IsFailure)
                return BadRequest($"Could not add new Invoice Number '{invoiceToAdd.InvoiceNumber}': {invoiceOrError.Error}.");

            VendorInvoice invoice = invoiceOrError.Value;
            VendorInvoiceItem vendorInvoiceItem;

            if (invoiceToAdd?.LineItems.Count > 0)
                foreach (var item in invoiceToAdd.LineItems)
                {
                    vendorInvoiceItem = VendorInvoiceItem.Create(
                                item.Item.PartNumber,
                                item.Item.Description,
                                item.Item.Manufacturer is null
                                    ? null
                                    : await manufacturerRepository.GetManufacturerEntityAsync(item.Item.Manufacturer.Id),
                                item.Item.SaleCode is null
                                    ? null
                                    : await saleCodeRepository.GetSaleCodeEntityAsync(item.Item.SaleCode.Id))
                            .Value;

                    invoice.AddLineItem(
                        VendorInvoiceLineItem.Create(
                            item.Type,
                            vendorInvoiceItem,
                            item.Quantity,
                            item.Cost,
                            item.Core,
                            item.PONumber,
                            item.TransactionDate)
                        .Value);
                };

            if (invoiceToAdd?.Payments.Count > 0)
                foreach (var payment in invoiceToAdd.Payments)
                    invoice.AddPayment(
                        VendorInvoicePayment.Create(
                            await paymentMethodRepository.GetPaymentMethodEntityAsync(payment.PaymentMethod.Id),
                            payment.Amount).Value);

            if (invoiceToAdd?.Taxes.Count > 0)
                foreach (var tax in invoiceToAdd.Taxes)
                {
                    // Don't save new, zero amount taxes sent by client
                    if (tax.Amount == 0)
                        continue;

                    invoice.AddTax(
                        VendorInvoiceTax.Create(
                            await salesTaxRepository.GetSalesTaxEntityAsync(tax.SalesTax.Id),
                            tax.Amount).Value);
                }

            repository.AddInvoice(invoice);
            await repository.SaveChangesAsync();

            return Created(
                new Uri($"{BasePath}/{invoice.Id}",
                UriKind.Relative),
                new
                {
                    invoice.Id
                });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteInvoiceAsync(long id)
        {
            var invoiceFromRepository = await repository.GetInvoiceEntityAsync(id);

            if (invoiceFromRepository is null)
                return NotFound($"Could not find Vendor Invoice in the database to delete with Id: {id}.");

            repository.DeleteInvoice(invoiceFromRepository);

            await repository.SaveChangesAsync();

            return NoContent();
        }

        private async Task<IReadOnlyList<Manufacturer>> GetManufacturersInInvoice(VendorInvoiceToWrite invoice)
        {
            return await manufacturerRepository.GetManufacturerEntitiesAsync(
                invoice.LineItems?.Select(
                    lineItem => lineItem.Item.Manufacturer.Id)
                .ToList());
        }

        private async Task<IReadOnlyList<SaleCode>> GetSaleCodesInInvoice(VendorInvoiceToWrite invoice)
        {
            return await saleCodeRepository.GetSaleCodeEntitiesAsync(
                invoice.LineItems?.Select(
                    lineItem => lineItem.Item.SaleCode.Id)
                .ToList());
        }

        private async Task<IReadOnlyList<Vendor>> GetVendorsInInvoice(VendorInvoiceToWrite invoice)
        {
            var result = new List<Vendor>
            {
                await vendorRepository.GetVendorEntityAsync(invoice.Vendor.Id)
            };

            List<long> ids = new();

            foreach (var payment in invoice.Payments)
            {
                if (payment.PaymentMethod.ReconcilingVendor is not null)
                    ids.Add(payment.PaymentMethod.ReconcilingVendor.Id);
            }

            result.AddRange(
                await vendorRepository.GetVendorEntitiesAsync(ids));

            return result;
        }

        private static bool ItemHasEdits(VendorInvoiceLineItemToWrite lineItemFromCaller, VendorInvoiceLineItem lineItemFromRepository)
        {
            return
                  lineItemFromRepository.Item.Description != lineItemFromCaller.Item.Description
                || lineItemFromRepository.Item.Manufacturer.Id != lineItemFromCaller.Item.Manufacturer.Id
                || lineItemFromRepository.Item.PartNumber != lineItemFromCaller.Item.PartNumber
                || lineItemFromRepository.Item.SaleCode.Id != lineItemFromCaller.Item.SaleCode.Id;
        }
    }
}

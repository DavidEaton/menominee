﻿using CustomerVehicleManagement.Domain.Entities.Payables;
using CustomerVehicleManagement.Shared.Models.Payables.Invoices;
using CustomerVehicleManagement.Shared.Models.Payables.Vendors;
using FluentAssertions;
using Menominee.Client.Components.Payables;
using Menominee.Common.Enums;
using System;
using System.Collections.Generic;
using Xunit;
using static CustomerVehicleManagement.Tests.Unit.Helpers.VendorInvoiceHelper;
namespace CustomerVehicleManagement.Tests.Unit.Entities
{
    public class InvoiceTotalsShould
    {
        [Fact]
        public void Initialize_Values_To_Zero()
        {
            var invoiceTotals = new InvoiceTotals();

            invoiceTotals.Purchases.Should().Be(0);
            invoiceTotals.Returns.Should().Be(0);
            invoiceTotals.CoreReturns.Should().Be(0);
            invoiceTotals.Defectives.Should().Be(0);
            invoiceTotals.Warranties.Should().Be(0);
            invoiceTotals.MiscellaneousDebits.Should().Be(0);
            invoiceTotals.MiscellaneousCredits.Should().Be(0);
            invoiceTotals.BalanceForwards.Should().Be(0);
            invoiceTotals.Taxes.Should().Be(0);
            invoiceTotals.Total.Should().Be(0);
            invoiceTotals.Payments.Should().Be(0);
            invoiceTotals.TaxableTotal.Should().Be(0);
        }

        [Fact]
        public void Clear_Results_To_Zero()
        {
            var invoiceTotals = new InvoiceTotals();
            var type = VendorInvoiceLineItemType.Purchase;
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var paymentLineCount = 5;
            var paymentLineAmount = 10.01;
            var taxLineCount = 5;
            var taxLineAmount = .1;
            var invoice = CreateVendorInvoiceToWrite(
                type,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity,
                paymentLineCount: paymentLineCount,
                paymentLineAmount: paymentLineAmount,
                taxLineCount: taxLineCount,
                taxLineAmount: taxLineAmount);
            var invoiceTotalsOrError = invoiceTotals.CalculateInvoiceTotals(invoice);
            invoiceTotalsOrError.Value.Total.Should().BeGreaterThan(0);
            invoiceTotalsOrError.IsFailure.Should().BeFalse();

            invoiceTotals.Clear();

            invoiceTotals.Purchases.Should().Be(0);
            invoiceTotals.Returns.Should().Be(0);
            invoiceTotals.CoreReturns.Should().Be(0);
            invoiceTotals.Defectives.Should().Be(0);
            invoiceTotals.Warranties.Should().Be(0);
            invoiceTotals.MiscellaneousDebits.Should().Be(0);
            invoiceTotals.MiscellaneousCredits.Should().Be(0);
            invoiceTotals.BalanceForwards.Should().Be(0);
            invoiceTotals.Taxes.Should().Be(0);
            invoiceTotals.Total.Should().Be(0);
            invoiceTotals.Payments.Should().Be(0);
            invoiceTotals.TaxableTotal.Should().Be(0);
        }

        [Fact]
        public void CalculateInvoiceTotal()
        {
            var invoiceTotals = new InvoiceTotals();
            var type = VendorInvoiceLineItemType.Purchase;
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var paymentLineCount = 5;
            var paymentLineAmount = 10.01;
            var expectedPayments = paymentLineCount * paymentLineAmount;
            var taxLineCount = 5;
            var taxLineAmount = .1;
            var expectedTaxes = taxLineCount * taxLineAmount;
            var expectedPurchasesAmount = type == VendorInvoiceLineItemType.Purchase
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedReturnsAmount = type == VendorInvoiceLineItemType.Return
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedCoreReturnsAmount = type == VendorInvoiceLineItemType.CoreReturn
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedDefectivesAmount = type == VendorInvoiceLineItemType.Defective
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedWarrantiesAmount = type == VendorInvoiceLineItemType.Warranty
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedMiscellaneousDebitsAmount = type == VendorInvoiceLineItemType.MiscDebit
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedMiscellaneousCreditsAmount = type == VendorInvoiceLineItemType.MiscCredit
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedBalanceForwardsAmount = type == VendorInvoiceLineItemType.BalanceForward
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedTotal =
                  expectedPurchasesAmount
                + expectedReturnsAmount
                + expectedCoreReturnsAmount
                + expectedDefectivesAmount
                + expectedWarrantiesAmount
                + expectedMiscellaneousDebitsAmount
                + expectedMiscellaneousCreditsAmount
                + expectedBalanceForwardsAmount
                + expectedTaxes;

            var invoice = CreateVendorInvoiceToWrite(
                type,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity,
                paymentLineCount: paymentLineCount,
                paymentLineAmount: paymentLineAmount,
                taxLineCount: taxLineCount,
                taxLineAmount: taxLineAmount);
            var invoiceTotalsOrError = invoiceTotals.CalculateInvoiceTotals(invoice);

            invoiceTotalsOrError.IsFailure.Should().BeFalse();
            invoiceTotalsOrError.Value.Payments.Should().Be(expectedPayments);
            invoiceTotalsOrError.Value.Taxes.Should().Be(expectedTaxes);
            invoiceTotalsOrError.Value.Purchases.Should().Be(expectedPurchasesAmount);
            invoiceTotalsOrError.Value.Returns.Should().Be(expectedReturnsAmount);
            invoiceTotalsOrError.Value.CoreReturns.Should().Be(expectedCoreReturnsAmount);
            invoiceTotalsOrError.Value.Defectives.Should().Be(expectedDefectivesAmount);
            invoiceTotalsOrError.Value.Warranties.Should().Be(expectedWarrantiesAmount);
            invoiceTotalsOrError.Value.MiscellaneousDebits.Should().Be(expectedMiscellaneousDebitsAmount);
            invoiceTotalsOrError.Value.MiscellaneousCredits.Should().Be(expectedMiscellaneousCreditsAmount);
            invoiceTotalsOrError.Value.BalanceForwards.Should().Be(expectedBalanceForwardsAmount);
            invoiceTotalsOrError.Value.Total.Should().Be(expectedTotal);
        }

        [Theory]
        [MemberData(nameof(TestData.Data), MemberType = typeof(TestData))]
        public void CalculateInvoiceTotals_For_Each_LineItemType(VendorInvoiceLineItemType lineItemType)
        {
            var invoiceTotals = new InvoiceTotals();
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var paymentLineCount = 5;
            var paymentLineAmount = 10.01;
            var expectedPayments = paymentLineCount * paymentLineAmount;
            var taxLineCount = 5;
            var taxLineAmount = .1;
            var expectedTaxes = taxLineCount * taxLineAmount;
            var expectedPurchasesAmount = lineItemType == VendorInvoiceLineItemType.Purchase
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedReturnsAmount = lineItemType == VendorInvoiceLineItemType.Return
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedCoreReturnsAmount = lineItemType == VendorInvoiceLineItemType.CoreReturn
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedDefectivesAmount = lineItemType == VendorInvoiceLineItemType.Defective
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedWarrantiesAmount = lineItemType == VendorInvoiceLineItemType.Warranty
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedMiscellaneousDebitsAmount = lineItemType == VendorInvoiceLineItemType.MiscDebit
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedMiscellaneousCreditsAmount = lineItemType == VendorInvoiceLineItemType.MiscCredit
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedBalanceForwardsAmount = lineItemType == VendorInvoiceLineItemType.BalanceForward
                ? (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount
                : 0;
            var expectedTotal =
                  expectedPurchasesAmount
                + expectedReturnsAmount
                + expectedCoreReturnsAmount
                + expectedDefectivesAmount
                + expectedWarrantiesAmount
                + expectedMiscellaneousDebitsAmount
                + expectedMiscellaneousCreditsAmount
                + expectedBalanceForwardsAmount
                + expectedTaxes;

            var invoice = CreateVendorInvoiceToWrite(
                lineItemType,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity,
                paymentLineCount: paymentLineCount,
                paymentLineAmount: paymentLineAmount,
                taxLineCount: taxLineCount,
                taxLineAmount: taxLineAmount);
            var invoiceTotalsOrError = invoiceTotals.CalculateInvoiceTotals(invoice);

            invoiceTotalsOrError.IsFailure.Should().BeFalse();
            invoiceTotalsOrError.Value.Payments.Should().Be(expectedPayments);
            invoiceTotalsOrError.Value.Taxes.Should().Be(expectedTaxes);
            invoiceTotalsOrError.Value.Purchases.Should().Be(expectedPurchasesAmount);
            invoiceTotalsOrError.Value.Returns.Should().Be(expectedReturnsAmount);
            invoiceTotalsOrError.Value.CoreReturns.Should().Be(expectedCoreReturnsAmount);
            invoiceTotalsOrError.Value.Defectives.Should().Be(expectedDefectivesAmount);
            invoiceTotalsOrError.Value.Warranties.Should().Be(expectedWarrantiesAmount);
            invoiceTotalsOrError.Value.MiscellaneousDebits.Should().Be(expectedMiscellaneousDebitsAmount);
            invoiceTotalsOrError.Value.MiscellaneousCredits.Should().Be(expectedMiscellaneousCreditsAmount);
            invoiceTotalsOrError.Value.BalanceForwards.Should().Be(expectedBalanceForwardsAmount);
            invoiceTotalsOrError.Value.Total.Should().Be(expectedTotal);
        }

        [Fact]
        public void CalculateInvoicePayments()
        {
            var invoiceTotals = new InvoiceTotals();
            var type = VendorInvoiceLineItemType.Purchase;
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var paymentLineCount = 5;
            var paymentLineAmount = 10.01;
            var expectedPayments = paymentLineCount * paymentLineAmount;
            var taxLineCount = 5;
            var taxLineAmount = .1;
            var invoice = CreateVendorInvoiceToWrite(
                type,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity,
                paymentLineCount: paymentLineCount,
                paymentLineAmount: paymentLineAmount,
                taxLineCount: taxLineCount,
                taxLineAmount: taxLineAmount);

            var invoicePaymentsOrError = invoiceTotals.CalculateInvoicePayments(invoice);

            invoicePaymentsOrError.Value.Should().Be(expectedPayments);
            invoicePaymentsOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoiceTaxes()
        {
            var invoiceTotals = new InvoiceTotals();
            var type = VendorInvoiceLineItemType.Purchase;
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var taxLineCount = 5;
            var taxLineAmount = 10.01;
            var expectedTaxes = taxLineCount * taxLineAmount;
            var invoice = CreateVendorInvoiceToWrite(
                type,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity,
                0, 0, // payment
                taxLineCount: taxLineCount,
                taxLineAmount: taxLineAmount);

            var invoiceTaxesOrError = invoiceTotals.CalculateInvoiceTaxes(invoice);

            invoiceTaxesOrError.Value.Should().Be(expectedTaxes);
            invoiceTaxesOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoicePurchases()
        {
            var invoiceTotals = new InvoiceTotals();
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var invoice = CreateVendorInvoiceToWrite(
                VendorInvoiceLineItemType.Purchase,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity, 0, 0, 0, 0);
            var expectedPurchaseAmount = (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount;

            var invoicePurchasesOrError = invoiceTotals.CalculateInvoicePurchases(invoice);

            invoicePurchasesOrError.Value.Should().Be(expectedPurchaseAmount);
            invoicePurchasesOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoiceReturns()
        {
            var invoiceTotals = new InvoiceTotals();
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var invoice = CreateVendorInvoiceToWrite(
                VendorInvoiceLineItemType.Return,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity, 0, 0, 0, 0);
            var expectedReturnAmount = (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount;

            var invoiceReturnsOrError = invoiceTotals.CalculateInvoiceReturns(invoice);

            invoiceReturnsOrError.Value.Should().Be(expectedReturnAmount);
            invoiceReturnsOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoiceCoreReturns()
        {
            var invoiceTotals = new InvoiceTotals();
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var invoice = CreateVendorInvoiceToWrite(
                VendorInvoiceLineItemType.CoreReturn,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity, 0, 0, 0, 0);
            var expectedCoreReturnAmount = (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount;

            var invoiceCoreReturnsOrError = invoiceTotals.CalculateInvoiceCoreReturns(invoice);

            invoiceCoreReturnsOrError.Value.Should().Be(expectedCoreReturnAmount);
            invoiceCoreReturnsOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoiceDefectives()
        {
            var invoiceTotals = new InvoiceTotals();
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var invoice = CreateVendorInvoiceToWrite(
                VendorInvoiceLineItemType.Defective,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity, 0, 0, 0, 0);
            var expectedDefectiveAmount = (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount;

            var invoiceDefectivesOrError = invoiceTotals.CalculateInvoiceDefectives(invoice);

            invoiceDefectivesOrError.Value.Should().Be(expectedDefectiveAmount);
            invoiceDefectivesOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoiceWarranties()
        {
            var invoiceTotals = new InvoiceTotals();
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var invoice = CreateVendorInvoiceToWrite(
                VendorInvoiceLineItemType.Warranty,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity, 0, 0, 0, 0);
            var expectedWarrantyAmount = (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount;

            var invoiceWarrantiesOrError = invoiceTotals.CalculateInvoiceWarranties(invoice);

            invoiceWarrantiesOrError.Value.Should().Be(expectedWarrantyAmount);
            invoiceWarrantiesOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoiceMiscellaneousDebits()
        {
            var invoiceTotals = new InvoiceTotals();
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var invoice = CreateVendorInvoiceToWrite(
                VendorInvoiceLineItemType.MiscDebit,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity, 0, 0, 0, 0);
            var expectedMiscellaneousDebitAmount = (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount;

            var invoiceWarrantiesOrError = invoiceTotals.CalculateInvoiceMiscellaneousDebits(invoice);

            invoiceWarrantiesOrError.Value.Should().Be(expectedMiscellaneousDebitAmount);
            invoiceWarrantiesOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoiceMiscellaneousCredits()
        {
            var invoiceTotals = new InvoiceTotals();
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var invoice = CreateVendorInvoiceToWrite(
                VendorInvoiceLineItemType.MiscCredit,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity, 0, 0, 0, 0);
            var expectedMiscellaneousCreditAmount = (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount;

            var invoiceWarrantiesOrError = invoiceTotals.CalculateInvoiceMiscellaneousCredits(invoice);

            invoiceWarrantiesOrError.Value.Should().Be(expectedMiscellaneousCreditAmount);
            invoiceWarrantiesOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoiceBalanceForwards()
        {
            var invoiceTotals = new InvoiceTotals();
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var invoice = CreateVendorInvoiceToWrite(
                VendorInvoiceLineItemType.BalanceForward,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity, 0, 0, 0, 0);
            var expectedBalanceForwardAmount = (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount;

            var invoiceWarrantiesOrError = invoiceTotals.CalculateInvoiceBalanceForwards(invoice);

            invoiceWarrantiesOrError.Value.Should().Be(expectedBalanceForwardAmount);
            invoiceWarrantiesOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void CalculateInvoiceTaxableTotal()
        {
            var invoiceTotals = new InvoiceTotals();
            // Purchase LineItems: 100
            var lineItemCount = 5;
            var lineItemCore = 1.0;
            var lineItemCost = 1.0;
            var lineItemQuantity = 10.0;
            var lineItemType = VendorInvoiceLineItemType.Purchase;
            var expectedPurchases = (lineItemCost + lineItemCore) * lineItemQuantity * lineItemCount;
            // Payments: 50.05
            var paymentLineCount = 5;
            var paymentLineAmount = 10.01;
            var expectedPayments = paymentLineCount * paymentLineAmount;
            // Taxes: .5
            var taxLineCount = 5;
            var taxLineAmount = .1;
            var expectedTaxes = taxLineCount * taxLineAmount;
            // BalanceForwards: 4
            var balanceForwardLineItemCount = 2;
            var balanceForwardLineItemCore = 1.0;
            var balanceForwardLineItemCost = 1.0;
            var balanceForwardLineItemQuantity = 1.0;
            var expectedBalanceForwardAmount =
                (balanceForwardLineItemCost + balanceForwardLineItemCore) * balanceForwardLineItemQuantity * balanceForwardLineItemCount;
            // Total: 104.5
            var expectedTotal =
                // Purchase LineItems: 100
                (lineItemCost + lineItemCore)
                * lineItemQuantity
                * lineItemCount
                // Taxes: .5
                + (taxLineCount * taxLineAmount)
                // BalanceForwards: 4
                + (balanceForwardLineItemCost + balanceForwardLineItemCore) * balanceForwardLineItemQuantity * balanceForwardLineItemCount;

            var balanceForwardLineItems = CreateLineItemsToWrite(VendorInvoiceLineItemType.BalanceForward, balanceForwardLineItemCount, balanceForwardLineItemCore, balanceForwardLineItemCost, balanceForwardLineItemQuantity);

            var invoice = CreateVendorInvoiceToWrite(
                lineItemType: lineItemType,
                lineItemCount: lineItemCount,
                lineItemCore: lineItemCore,
                lineItemCost: lineItemCost,
                lineItemQuantity: lineItemQuantity,
                paymentLineCount: paymentLineCount,
                paymentLineAmount: paymentLineAmount,
                taxLineCount: taxLineCount,
                taxLineAmount: taxLineAmount);

            // add the balanceForwardLineItems to the invoice
            foreach (var item in balanceForwardLineItems)
                invoice.LineItems.Add(item);
            // Recalculate invoice values
            var invoiceTaxableTotalOrError = invoiceTotals.CalculateInvoiceTaxableTotal(invoice);

            // TaxableTotal = Total - BalanceForwards - Taxes;
            //  100                           104.5           4                              .5
            var expectedInvoiceTaxableTotal = expectedTotal - expectedBalanceForwardAmount - expectedTaxes;

            invoiceTotals.Taxes.Should().Be(expectedTaxes); //.5
            invoiceTotals.Payments.Should().Be(expectedPayments);//50.05
            invoiceTotals.Purchases.Should().Be(expectedPurchases);//100
            invoiceTotals.BalanceForwards.Should().Be(expectedBalanceForwardAmount); //100
            invoiceTaxableTotalOrError.Value.Should().Be(expectedInvoiceTaxableTotal); //100
            invoiceTaxableTotalOrError.IsFailure.Should().BeFalse();
        }

        private static VendorInvoiceToWrite CreateVendorInvoiceToWrite(
            VendorInvoiceLineItemType lineItemType,
            int lineItemCount,
            double lineItemCore,
            double lineItemCost,
            double lineItemQuantity,
            int paymentLineCount,
            double paymentLineAmount,
            int taxLineCount,
            double taxLineAmount) => new()
            {
                Date = DateTime.Now,
                InvoiceNumber = "123",
                Vendor = CreateVendorToReadInList(),
                Status = VendorInvoiceStatus.Open,
                Total = 0,
                LineItems = CreateLineItemsToWrite(lineItemType, lineItemCount, lineItemCore, lineItemCost, lineItemQuantity),
                Payments = CreatePaymentsToWrite(paymentLineCount, paymentLineAmount),
                Taxes = CreateTaxesToWrite(taxLineCount, taxLineAmount)
            };

        private static VendorToReadInList CreateVendorToReadInList()
        {
            return new VendorToReadInList()
            {
                Id = 1,
                IsActive = true,
                Name = Utilities.RandomCharacters(Vendor.MinimumLength) + 1,
                VendorCode = Utilities.RandomCharacters(Vendor.MinimumLength + 1)
            };
        }

        internal class TestData
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { VendorInvoiceLineItemType.BalanceForward };
                    yield return new object[] { VendorInvoiceLineItemType.Defective };
                }
            }
        }

    }
}

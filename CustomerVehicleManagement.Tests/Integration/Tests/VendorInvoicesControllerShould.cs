﻿using CustomerVehicleManagement.Api.Manufacturers;
using CustomerVehicleManagement.Api.Payables.Invoices;
using CustomerVehicleManagement.Api.Payables.PaymentMethods;
using CustomerVehicleManagement.Api.Payables.Vendors;
using CustomerVehicleManagement.Api.SaleCodes;
using CustomerVehicleManagement.Api.Taxes;
using CustomerVehicleManagement.Shared.Models.Payables.Invoices;
using CustomerVehicleManagement.Shared.Models.Payables.Vendors;
using FluentAssertions;
using Menominee.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CustomerVehicleManagement.Tests.Integration.Tests
{
    public class VendorInvoicesControllerShould : IntegrationTestBase
    {
        [Fact]
        public async Task AddInvoiceAsync()
        {
            // Arrange
            VendorInvoiceToWrite invoiceToAdd = new()
            {
                Date = DateTime.Today,
                DocumentType = VendorInvoiceDocumentType.Invoice,
                Status = VendorInvoiceStatus.Open,
                Total = 10.0,
                Vendor = new VendorToReadInList()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Vendor One",
                    VendorCode = "V01"
                }
            };

            // Act
            using (var context = Helpers.CreateTestContext())
            {
                var controller = new VendorInvoicesController(
                    new VendorInvoiceRepository(context),
                    new VendorRepository(context),
                    new VendorInvoicePaymentMethodRepository(context),
                    new SalesTaxRepository(context),
                    new ManufacturerRepository(context),
                    new SaleCodeRepository(context)
                    );
                var addInvoiceResult = await controller.AddInvoiceAsync(invoiceToAdd);
                var addInvoiceResponse = JsonSerializer.Deserialize<CreatedResultResponse>(addInvoiceResult.ToJson());
                var getInvoiceActionResult = await controller.GetInvoiceAsync(addInvoiceResponse.Value.Id);
                var actionResultValue = (OkObjectResult)getInvoiceActionResult.Result;
                var vendorInvoice = (VendorInvoiceToRead)actionResultValue.Value;

                // Assert
                vendorInvoice.Should().BeOfType<VendorInvoiceToRead>();
                vendorInvoice.Status.Should().Be(VendorInvoiceStatus.Open);
                vendorInvoice.Date.Should().Be(DateTime.Today);
                vendorInvoice.DatePosted.Should().Be(null);
            }
        }

        [Fact]
        public async Task UpdateInvoiceAsync()
        {
            // Arrange
            VendorInvoiceToWrite invoiceToAdd = new()
            {
                Date = DateTime.Today,
                DocumentType = VendorInvoiceDocumentType.Invoice,
                Status = VendorInvoiceStatus.Open,
                Total = 10.0,
                Vendor = new VendorToReadInList()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Vendor One",
                    VendorCode = "V01"
                }
            };

            // Act
            using (var context = Helpers.CreateTestContext())
            {
                var controller = new VendorInvoicesController(
                    new VendorInvoiceRepository(context),
                    new VendorRepository(context),
                    new VendorInvoicePaymentMethodRepository(context),
                    new SalesTaxRepository(context),
                    new ManufacturerRepository(context),
                    new SaleCodeRepository(context)
                    );
                var addInvoiceResult = await controller.AddInvoiceAsync(invoiceToAdd);
                var addInvoiceResponse = JsonSerializer.Deserialize<CreatedResultResponse>(addInvoiceResult.ToJson());
                var getInvoiceActionResult = await controller.GetInvoiceAsync(addInvoiceResponse.Value.Id);
                var actionResultValue = (OkObjectResult)getInvoiceActionResult.Result;
                var vendorInvoice = (VendorInvoiceToRead)actionResultValue.Value;

                // Assert
                vendorInvoice.Should().BeOfType<VendorInvoiceToRead>();
                vendorInvoice.Status.Should().Be(VendorInvoiceStatus.Open);
                vendorInvoice.Date.Should().Be(DateTime.Today);
                vendorInvoice.DatePosted.Should().Be(null);
                vendorInvoice.LineItems.Count.Should().Be(0);
                vendorInvoice.Payments.Count.Should().Be(0);
                vendorInvoice.Taxes.Count.Should().Be(0);
            }
        }

    }
}

﻿using FluentValidation;
using Menominee.Domain.Entities;
using Menominee.Domain.Entities.Inventory;
using Menominee.Domain.Entities.Payables;
using Menominee.Shared.Models.Payables.Invoices.LineItems;
using System.Collections.Generic;

namespace Menominee.Api.Features.Payables.Invoices
{
    public class VendorInvoiceLineItemsValidator : AbstractValidator<IList<VendorInvoiceLineItemToWrite>>
    {
        private const string notEmptyMessage = "Line Item must not be empty.";

        public VendorInvoiceLineItemsValidator()
        {
            // TODO: Replace empty shopSupplies with injected repository call to real SaleCodeShopSupplies from database
            var shopSupplies = SaleCodeShopSupplies.Create(0, 0, 0, 0, true, true).Value;
            // TODO: figure out solution for manufacturer create
            RuleFor(lineItems => lineItems)
                .NotEmpty()
                .ForEach(lineItem =>
                {
                    lineItem.NotEmpty().WithMessage(notEmptyMessage);
                    lineItem.MustBeEntity(lineItem =>
                        VendorInvoiceLineItem.Create(
                            lineItem.Type,
                            VendorInvoiceItem.Create(
                                lineItem.Item.PartNumber,
                                lineItem.Item.Description,
                                lineItem?.Item?.Manufacturer is null
                                ? null
                                : Manufacturer.Create(
                                    lineItem.Item.Manufacturer.Id,
                                    lineItem?.Item?.Manufacturer.Name,
                                    lineItem?.Item?.Manufacturer.Prefix,
                                    new List<string>(),
                                    new List<long>())
                                .Value,
                                lineItem?.Item?.SaleCode is null
                                ? null
                                : SaleCode.Create(
                                    lineItem?.Item?.SaleCode.Name,
                                    lineItem?.Item?.SaleCode.Code,
                                    (double)(lineItem?.Item?.SaleCode.LaborRate),
                                    (double)(lineItem?.Item?.SaleCode.DesiredMargin),
                                    shopSupplies,
                                    new List<string>())
                                .Value)
                            .Value,
                            lineItem.Quantity,
                            lineItem.Cost,
                            lineItem.Core,
                            lineItem?.PONumber,
                            lineItem?.TransactionDate
                        ));
                });
        }
    }
}

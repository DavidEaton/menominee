﻿using CSharpFunctionalExtensions;
using CustomerVehicleManagement.Domain.Entities.Inventory;
using Menominee.Common.ValueObjects;
using System;
using System.Collections.Generic;

namespace CustomerVehicleManagement.Domain.Entities.Payables
{
    public class VendorInvoiceItem : AppValueObject
    {
        public static readonly string RequiredMessage = $"Please include all required items.";
        public static readonly int MaximumLength = 255;
        public static readonly string MaximumLengthMessage = $"Cannot be over {MaximumLength} character(s) in length.";
        public static readonly int MinimumLength = 1;
        public static readonly string MinimumLengthMessage = $"Cannot be under {MinimumLength} character(s) in length.";

        public string PartNumber { get; private set; } // required, 1..255 or maybe less. Really just a string?
        public string Description { get; private set; } // required, 1.255
        public Manufacturer Manufacturer { get; private set; } // not required for every item type
        public SaleCode SaleCode { get; private set; } // not required

        private VendorInvoiceItem(
            string partNumber,
            string description,
            Manufacturer manufacturer = null,
            SaleCode saleCode = null)
        {
            partNumber = (partNumber ?? string.Empty).Trim();
            description = (description ?? string.Empty).Trim();

            if (partNumber.Length > MaximumLength || partNumber.Length < MinimumLength)
                throw new ArgumentOutOfRangeException(MaximumLengthMessage);

            if (description.Length < MinimumLength || description.Length > MaximumLength)
                throw new ArgumentOutOfRangeException(MaximumLengthMessage);

            PartNumber = partNumber;
            Description = description;

            if (manufacturer is not null)
                Manufacturer = manufacturer;

            if (saleCode is not null)
                SaleCode = saleCode;
        }

        public static Result<VendorInvoiceItem> Create(
            string partNumber,
            string description,
            Manufacturer manufacturer = null,
            SaleCode saleCode = null)
        {
            partNumber = (partNumber ?? string.Empty).Trim();
            description = (description ?? string.Empty).Trim();

            if (partNumber.Length > MaximumLength || description.Length > MaximumLength)
                return Result.Failure<VendorInvoiceItem>(MaximumLengthMessage);

            if (partNumber.Length < MinimumLength || description.Length < MinimumLength)
                return Result.Failure<VendorInvoiceItem>(MinimumLengthMessage);

            return Result.Success(new VendorInvoiceItem(
                partNumber, description, manufacturer, saleCode));
        }

        public void SetPartNumber(string partNumber)
        {
            partNumber = (partNumber ?? string.Empty).Trim();

            if (partNumber.Length > MaximumLength || partNumber.Length < MinimumLength)
                throw new ArgumentOutOfRangeException(MaximumLengthMessage);

            PartNumber = partNumber;
        }

        public void SetDescription(string description)
        {
            description = (description ?? string.Empty).Trim();

            if (description.Length < MinimumLength || description.Length > MaximumLength)
                throw new ArgumentOutOfRangeException(MaximumLengthMessage);

            Description = description;
        }

        public void SetManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer is null)
                throw new ArgumentOutOfRangeException(RequiredMessage);

            Manufacturer = manufacturer;
        }

        public void ClearManufacturer() => Manufacturer = null;

        public void SetSaleCode(SaleCode saleCode)
        {
            if (saleCode is null)
                throw new ArgumentOutOfRangeException(RequiredMessage);

            SaleCode = saleCode;
        }

        public void ClearSaleCode() => SaleCode = null;

        #region ORM

        // EF requires an empty constructor
        public VendorInvoiceItem() { }

        #endregion

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PartNumber;
            yield return Manufacturer;
            yield return Description;
            yield return SaleCode;
        }
    }
}

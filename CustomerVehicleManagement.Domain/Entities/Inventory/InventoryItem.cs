﻿using CSharpFunctionalExtensions;
using Menominee.Common.Enums;
using System;
using Entity = Menominee.Common.Entity;

namespace CustomerVehicleManagement.Domain.Entities.Inventory
{
    public class InventoryItem : Entity
    {
        public static readonly int MinimumLength = 1;
        public static readonly int MaximumLength = 255;
        public static readonly string InvalidMessage = $"Item Number, Description must be between {MinimumLength} and {MaximumLength} character(s) in length.";
        public static readonly string RequiredMessage = $"Please include all required items.";

        public Manufacturer Manufacturer { get; private set; }
        public string ItemNumber { get; private set; }
        public string Description { get; private set; }
        public ProductCode ProductCode { get; private set; }
        public InventoryItemType ItemType { get; private set; }
        public InventoryItemPart Part { get; private set; }
        public InventoryItemLabor Labor { get; private set; }
        public InventoryItemTire Tire { get; private set; }
        public InventoryItemPackage Package { get; private set; }
        public InventoryItemInspection Inspection { get; private set; }
        public InventoryItemWarranty Warranty { get; private set; }

        private InventoryItem(Manufacturer manufacturer, string itemNumber, string description, ProductCode productCode, InventoryItemType itemType, InventoryItemPart part, InventoryItemLabor labor, InventoryItemTire tire, InventoryItemPackage package, InventoryItemInspection inspection, InventoryItemWarranty warranty)
        {
            if (manufacturer is null || productCode is null)
                throw new ArgumentOutOfRangeException(RequiredMessage);

            itemNumber = (itemNumber ?? string.Empty).Trim();
            description = (description ?? string.Empty).Trim();

            if (itemNumber.Length < MinimumLength || itemNumber.Length > MaximumLength)
                throw new ArgumentOutOfRangeException($"{InvalidMessage} You entered {itemNumber.Length} character(s).");

            if (description.Length < MinimumLength || description.Length > MaximumLength)
                throw new ArgumentOutOfRangeException($"{InvalidMessage} You entered {description.Length} character(s).");

            if (!Enum.IsDefined(typeof(InventoryItemType), itemType))
                throw new ArgumentOutOfRangeException(RequiredMessage);

            Manufacturer = manufacturer;
            ItemNumber = itemNumber;
            Description = description;
            ProductCode = productCode;
            ItemType = itemType;
            Part = part;
            Labor = labor;
            Tire = tire;
            Package = package;
            Inspection = inspection;
            Warranty = warranty;
        }

        public static Result<InventoryItem> Create(
            Manufacturer manufacturer,
            string itemNumber,
            string description,
            ProductCode productCode,
            InventoryItemType itemType,
            InventoryItemPart part,
            InventoryItemLabor labor,
            InventoryItemTire tire,
            InventoryItemPackage package,
            InventoryItemInspection inspection,
            InventoryItemWarranty warranty)
        {
            if (manufacturer is null || productCode is null)
                return Result.Failure<InventoryItem>(RequiredMessage);

            itemNumber = (itemNumber ?? string.Empty).Trim();
            description = (description ?? string.Empty).Trim();

            if (itemNumber.Length < MinimumLength || itemNumber.Length > MaximumLength)
                return Result.Failure<InventoryItem>($"{InvalidMessage} You entered {itemNumber.Length} character(s).");

            if (description.Length < MinimumLength || description.Length > MaximumLength)
                return Result.Failure<InventoryItem>($"{InvalidMessage} You entered {description.Length} character(s).");

            if (!Enum.IsDefined(typeof(InventoryItemType), itemType))
                return Result.Failure<InventoryItem>(RequiredMessage);





            return Result.Success(new InventoryItem(manufacturer, itemNumber, description, productCode, itemType, part, labor, tire, package, inspection, warranty));
        }

        public Result<Manufacturer> SetManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer is null)
                return Result.Failure<Manufacturer>(RequiredMessage);

            return Result.Success(Manufacturer = manufacturer);
        }

        public void ClearManufacturer() => Manufacturer = null;

        private void SetItemType(InventoryItemType itemType)
        {
            if (!Enum.IsDefined(typeof(InventoryItemType), itemType))
                throw new ArgumentOutOfRangeException(nameof(itemType), "itemType is null");

            ItemType = itemType;
        }

        public Result<InventoryItemPart> SetPart(InventoryItemPart part)
        {
            if (part is null)
                return Result.Failure<InventoryItemPart>(RequiredMessage);

            SetItemType(InventoryItemType.Part);

            return Result.Success(Part = part);
        }

        public Result<InventoryItemLabor> SetLabor(InventoryItemLabor labor)
        {
            if (labor is null)
                return Result.Failure<InventoryItemLabor>(RequiredMessage);

            SetItemType(InventoryItemType.Part);

            return Result.Success(Labor = labor);
        }

        public Result<InventoryItemTire> SetTire(InventoryItemTire tire)
        {
            if (tire is null)
                return Result.Failure<InventoryItemTire>(RequiredMessage);

            SetItemType(InventoryItemType.Tire);

            return Result.Success(Tire = tire);
        }

        public Result<InventoryItemPackage> SetPackage(InventoryItemPackage package)
        {
            if (package is null)
                return Result.Failure<InventoryItemPackage>(RequiredMessage);

            SetItemType(InventoryItemType.Package);

            return Result.Success(Package = package);
        }

        public Result<InventoryItemInspection> SetInspection(InventoryItemInspection inspection)
        {
            if (inspection is null)
                return Result.Failure<InventoryItemInspection>(RequiredMessage);

            SetItemType(InventoryItemType.Inspection);

            return Result.Success(Inspection = inspection);
        }

        public Result<InventoryItemWarranty> SetWarranty(InventoryItemWarranty warranty)
        {
            if (warranty is null)
                return Result.Failure<InventoryItemWarranty>(RequiredMessage);

            SetItemType(InventoryItemType.Warranty);

            return Result.Success(Warranty = warranty);
        }

        #region ORM

        // EF requires a parameterless constructor
        protected InventoryItem() { }

        #endregion
    }
}

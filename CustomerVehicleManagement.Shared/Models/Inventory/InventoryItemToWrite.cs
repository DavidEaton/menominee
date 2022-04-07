﻿using CustomerVehicleManagement.Domain.Entities.Inventory;
using Menominee.Common.Enums;

namespace CustomerVehicleManagement.Shared.Models.Inventory
{
    public class InventoryItemToWrite
    {
        public long Id { get; set; }
        //public virtual Manufacturer Manufacturer { get; set; }
        public long ManufacturerId { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        //public virtual ProductCode ProductCode { get; set; }
        public long ProductCodeId { get; set; }
        public InventoryItemType ItemType { get; set; }
        public long DetailId { get; set; }

        public InventoryPartToWrite Part { get; set; }
        public InventoryLaborToWrite Labor { get; set; }
        public InventoryTireToWrite Tire { get; set; }
    }
}

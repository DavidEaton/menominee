﻿using Menominee.Common;
using Menominee.Common.Enums;

namespace CustomerVehicleManagement.Domain.Entities.Inventory
{
    public class InventoryItemLabor : Entity
    {
        public InventoryItem InventoryItem { get; private set; }
        public ItemLaborType LaborType { get; private set; }
        public double LaborAmount { get; private set; }
        public ItemLaborType TechPayType { get; private set; }
        public double TechPayAmount { get; private set; }
        public SkillLevel SkillLevel { get; private set; }

        #region ORM

        // EF requires a parameterless constructor
        protected InventoryItemLabor() { }

        #endregion
    }
}

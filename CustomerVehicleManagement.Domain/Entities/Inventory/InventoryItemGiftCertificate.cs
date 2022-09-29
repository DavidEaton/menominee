﻿using Menominee.Common;

namespace CustomerVehicleManagement.Domain.Entities.Inventory
{
    // TODO: No detail for this entitiy.  Can we avoid using this class?
    public class InventoryItemGiftCertificate : Entity
    {
        public InventoryItem InventoryItem { get; private set; }

        #region ORM

        // EF requires a parameterless constructor
        protected InventoryItemGiftCertificate() { }

        #endregion  
    }
}

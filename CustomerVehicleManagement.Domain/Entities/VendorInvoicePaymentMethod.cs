﻿using Menominee.Common;

namespace CustomerVehicleManagement.Domain.Entities
{
    public class VendorInvoicePaymentMethod : Entity
    {
        public string PaymentName { get; set; }

        #region ORM

        // EF requires an empty constructor
        public VendorInvoicePaymentMethod() { }

        #endregion
    }
}

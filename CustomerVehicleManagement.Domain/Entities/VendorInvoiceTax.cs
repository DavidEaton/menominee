﻿using Menominee.Common;

namespace CustomerVehicleManagement.Domain.Entities
{
    public class VendorInvoiceTax : Entity
    {
        public long InvoiceId { get; set; }
        public int Order { get; set; }
        public int TaxId { get; set; }
        public string TaxName { get; set; }
        public double Amount { get; set; }
    }
}

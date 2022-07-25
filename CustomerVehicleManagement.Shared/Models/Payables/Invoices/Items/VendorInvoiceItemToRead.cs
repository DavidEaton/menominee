﻿using CustomerVehicleManagement.Domain.Entities.Inventory;
using CustomerVehicleManagement.Shared.Models.Manufacturers;
using Menominee.Common.Enums;
using System;

namespace CustomerVehicleManagement.Shared.Models.Payables.Invoices.Items
{
    public class VendorInvoiceItemToRead
    {
        public long Id { get; set; }
        public long VendorInvoiceId { get; set; }
        public VendorInvoiceItemType Type { get; set; }
        public string PartNumber { get; set; }
        //public string MfrId { get; set; }
        public ManufacturerToRead Manufacturer { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public double Cost { get; set; }
        public double Core { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}

﻿namespace CustomerVehicleManagement.Shared.Models.Payables.Vendors
{
    public class VendorToEdit
    {
        public long Id { get; set; }
        public string VendorCode { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}

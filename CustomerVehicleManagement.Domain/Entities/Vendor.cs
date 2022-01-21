﻿using Menominee.Common;

namespace CustomerVehicleManagement.Domain.Entities
{
    public class Vendor : Entity
    {
        public string VendorCode { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}

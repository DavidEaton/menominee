﻿namespace CustomerVehicleManagement.Api.Data.Models
{
    public class PhoneReadDto
    {
        //public int Id { get; set; } PHONE IS NOT AN AGGREGATE ROOT
        public string Number { get; set; }
        public string PhoneType { get; set; }
        public bool Primary { get; set; }

    }
}

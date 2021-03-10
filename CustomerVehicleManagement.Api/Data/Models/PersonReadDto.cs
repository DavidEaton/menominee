﻿using SharedKernel.Enums;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace CustomerVehicleManagement.Api.Data.Models
{
    public class PersonReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public DriversLicense DriversLicense { get; set; }
        public Address Address { get; set; }
        public IEnumerable<PhoneReadDto> Phones { get; set; } = new List<PhoneReadDto>();
        public IEnumerable<EmailReadDto> Emails { get; set; } = new List<EmailReadDto>();
    }
}

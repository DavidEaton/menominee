﻿using Menominee.Domain.Enums;
using System;

namespace Menominee.Shared.Models.Persons.DriversLicenses
{
    public class DriversLicenseToRead
    {
        public string Number { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expiry { get; set; }
        public State State { get; set; }
    }
}

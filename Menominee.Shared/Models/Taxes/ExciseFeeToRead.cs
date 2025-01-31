﻿using Menominee.Domain.Enums;

namespace Menominee.Shared.Models.Taxes
{
    public class ExciseFeeToRead
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public ExciseFeeType FeeType { get; set; }
        public double Amount { get; set; }
    }
}

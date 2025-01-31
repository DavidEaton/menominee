﻿using Menominee.Domain.Enums;
using System.Collections.Generic;

namespace Menominee.Shared.Models.Taxes
{
    public class SalesTaxToWrite
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public SalesTaxType TaxType { get; set; }
        public int Order { get; set; }
        public bool? IsAppliedByDefault { get; set; }
        public bool? IsTaxable { get; set; }
        public string TaxIdNumber { get; set; }
        public double PartTaxRate { get; set; }
        public double LaborTaxRate { get; set; }
        public List<ExciseFeeToWrite> ExciseFees { get; set; } = new List<ExciseFeeToWrite>();
    }
}
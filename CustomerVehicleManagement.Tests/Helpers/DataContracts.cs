﻿using CustomerVehicleManagement.Domain.Entities.Inventory;
using CustomerVehicleManagement.Domain.Entities.Taxes;
using Menominee.Common.Enums;
using System.Collections.Generic;

namespace CustomerVehicleManagement.Tests.Helpers
{
    public class DataContracts
    {
        public static IReadOnlyList<ProductCode> CreateProductCodes(int count, Manufacturer manufacturer, IReadOnlyList<string> manufacturerCodes)
        {
            var list = new List<ProductCode>();

            for (int i = 0; i < count; i++)
            {
                list.Add(ProductCode.Create(
                    manufacturer,
                    code: $"{i}Code",
                    name: $"{i}Name",
                    manufacturerCodes: manufacturerCodes
                    ).Value);
            }

            return list;
        }


        public static List<SalesTax> CreateSalesTaxes(int count)
        {
            var list = new List<SalesTax>();

            for (int i = 0; i < count; i++)
            {
                list.Add(SalesTax.Create(
                    description: $"{i} description",
                    taxType: SalesTaxType.Normal,
                    order: i,
                    taxIdNumber: $"EIN-000{i}",
                    partTaxRate: i * .1,
                    laborTaxRate: i * .2).Value);
            }

            return list;
        }


    }
}

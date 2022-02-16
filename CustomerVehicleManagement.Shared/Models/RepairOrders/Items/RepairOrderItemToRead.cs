﻿using CustomerVehicleManagement.Domain.Entities;
using CustomerVehicleManagement.Domain.Entities.RepairOrders;
using CustomerVehicleManagement.Shared.Models.RepairOrders.SerialNumbers;
using CustomerVehicleManagement.Shared.Models.RepairOrders.Taxes;
using CustomerVehicleManagement.Shared.Models.RepairOrders.Warranties;
using Menominee.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace CustomerVehicleManagement.Shared.Models.RepairOrders.Items
{
    public class RepairOrderItemToRead
    {
        public long Id { get; set; }
        public long RepairOrderServiceId { get; set; }
        public int SequenceNumber { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public SaleCode SaleCode { get; set; }
        public ProductCode ProductCode { get; set; }
        public SaleType SaleType { get; set; }
        public PartType PartType { get; set; }
        public bool IsDeclined { get; set; }
        public bool IsCounterSale { get; set; }
        public double QuantitySold { get; set; }
        public double SellingPrice { get; set; }
        public ItemLaborType LaborType { get; set; } = ItemLaborType.None;
        public double LaborEach { get; set; }
        public ItemDiscountType DiscountType { get; set; } = ItemDiscountType.None;
        public double DiscountEach { get; set; } = 0.0;
        public double Cost { get; set; }
        public double Core { get; set; }
        public double Total { get; set; }

        public IReadOnlyList<RepairOrderSerialNumberToRead> SerialNumbers { get; set; } = new List<RepairOrderSerialNumberToRead>();
        public IReadOnlyList<RepairOrderWarrantyToRead> Warranties { get; set; } = new List<RepairOrderWarrantyToRead>();
        public IReadOnlyList<RepairOrderItemTaxToRead> Taxes { get; set; } = new List<RepairOrderItemTaxToRead>();

        public static IReadOnlyList<RepairOrderItemToRead> ConvertToDto(IList<RepairOrderItem> items)
        {
            return items
                .Select(item =>
                        ConvertToDto(item))
                .ToList();
        }

        private static RepairOrderItemToRead ConvertToDto(RepairOrderItem item)
        {
            if (item != null)
            {
                return new RepairOrderItemToRead()
                {
                    Id = item.Id,
                    RepairOrderServiceId = item.RepairOrderServiceId,
                    SequenceNumber = item.SequenceNumber,
                    Manufacturer = item.Manufacturer,
                    PartNumber = item.PartNumber,
                    Description = item.Description,
                    SaleCode = item.SaleCode,
                    ProductCode = item.ProductCode,
                    SaleType = item.SaleType,
                    PartType = item.PartType,
                    IsDeclined = item.IsDeclined,
                    IsCounterSale = item.IsCounterSale,
                    QuantitySold = item.QuantitySold,
                    SellingPrice = item.SellingPrice,
                    LaborType = item.LaborType,
                    LaborEach = item.LaborEach,
                    DiscountType = item.DiscountType,
                    DiscountEach = item.DiscountEach,
                    Cost = item.Cost,
                    Core = item.Core,
                    Total = item.Total,
                    SerialNumbers = RepairOrderSerialNumberToRead.ConvertToDto(item.SerialNumbers),
                    Warranties = RepairOrderWarrantyToRead.ConvertToDto(item.Warranties),
                    Taxes = RepairOrderItemTaxToRead.ConvertToDto(item.Taxes)
                };
            }

            return null;
        }
    }
}

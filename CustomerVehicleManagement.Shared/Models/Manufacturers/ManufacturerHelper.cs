﻿using CustomerVehicleManagement.Domain.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerVehicleManagement.Shared.Models.Manufacturers
{
    public class ManufacturerHelper
    {
        public static ManufacturerToWrite ConvertReadToWriteDto(ManufacturerToRead manufacturer)
        {
            if (manufacturer is null)
                return new ManufacturerToWrite();

            return new ManufacturerToWrite()
            {
                Id = manufacturer.Id,
                Code = manufacturer.Code,
                Prefix = manufacturer.Prefix,
                Name = manufacturer.Name
            };
        }

        public static Manufacturer ConvertWriteDtoToEntity(ManufacturerToWrite manufacturer)
        {
            if (manufacturer is null)
                return new Manufacturer();

            return new()
            {
                Code = manufacturer.Code,
                Prefix = manufacturer.Prefix,
                Name = manufacturer.Name
            };
        }

        public static ManufacturerToRead ConvertEntityToReadDto(Manufacturer manufacturer)
        {
            if (manufacturer is null)
                return null;

            return new()
            {
                Id = manufacturer.Id,
                Code = manufacturer.Code,
                Prefix = manufacturer.Prefix,
                Name = manufacturer.Name
            };
        }

        public static ManufacturerToReadInList ConvertEntityToReadInListDto(Manufacturer manufacturer)
        {
            if (manufacturer is null)
                return null;

            return new()
            {
                Id = manufacturer.Id,
                Code = manufacturer.Code,
                Prefix = manufacturer.Prefix,
                Name = manufacturer.Name
            };
        }
    }
}

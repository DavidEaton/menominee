﻿

using CustomerVehicleManagement.Domain.Entities.Inventory;

namespace CustomerVehicleManagement.Shared.Models.Manufacturers
{
    public class ManufacturerToRead
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Prefix { get; set; }
        public string Name { get; set; }
        //public xxx Country { get; set; }
        //public xxx Franchise { get; set; }

        public static ManufacturerToRead ConvertToDto(Manufacturer mfr)
        {
            if (mfr != null)
            {
                return new ManufacturerToRead()
                {
                    Id = mfr.Id,
                    Code = mfr.Code,
                    Prefix = mfr.Prefix,
                    Name = mfr.Name
                };
            }

            return null;
        }

    }
}

﻿using Menominee.Domain.Enums;

namespace Menominee.Shared.Models.Vehicles
{
    public class VehicleToWrite
    {
        public long Id { get; set; }
        public string VIN { get; set; }
        public int? Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Plate { get; set; }
        public State? PlateStateProvince { get; set; }
        public string UnitNumber { get; set; }
        public string Color { get; set; }
        public bool Active { get; set; } = true;
        public bool NonTraditionalVehicle { get; set; } = false;
        public bool IsEmpty() =>
            string.IsNullOrWhiteSpace(VIN)
            && string.IsNullOrWhiteSpace(Make)
            && string.IsNullOrWhiteSpace(Model)
            && string.IsNullOrWhiteSpace(Plate)
            && string.IsNullOrWhiteSpace(UnitNumber)
            && string.IsNullOrWhiteSpace(Color);

        public override string ToString()
        {
            return $"{Year} {Make} {Model}";
        }
    }
}

﻿using CustomerVehicleManagement.Api.Data;
using CustomerVehicleManagement.Domain.Entities;
using CustomerVehicleManagement.Shared.Models.Manufacturers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerVehicleManagement.Api.Manufacturers
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly ApplicationDbContext context;

        public ManufacturerRepository(ApplicationDbContext context)
        {
            this.context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task AddManufacturerAsync(Manufacturer manufacturer)
        {
            if (manufacturer != null)
                await context.AddAsync(manufacturer);
        }

        public async Task DeleteManufacturerAsync(string code)
        {
            var manufacturerFromContext = await context.Manufacturers.FindAsync(code);
            if (manufacturerFromContext != null)
                context.Remove(manufacturerFromContext);
        }

        public void FixTrackingState()
        {
            context.FixState();
        }

        public async Task<ManufacturerToRead> GetManufacturerAsync(string code)
        {
            var manufacturerFromContext = await context.Manufacturers
                .AsNoTracking()
                .FirstOrDefaultAsync(manufacturer => manufacturer.Code == code);

            return ManufacturerHelper.ConvertToReadDto(manufacturerFromContext);
        }

        public async Task<Manufacturer> GetManufacturerEntityAsync(string code)
        {
            var manufacturerFromContext = await context.Manufacturers
                .FirstOrDefaultAsync(manufacturer => manufacturer.Code == code);

            return manufacturerFromContext;
        }

        public async Task<IReadOnlyList<ManufacturerToReadInList>> GetManufacturerListAsync()
        {
            IReadOnlyList<Manufacturer> manufacturers = await context.Manufacturers
                .AsNoTracking()
                .ToListAsync();

            return manufacturers.
                Select(manufacturer => ManufacturerHelper.ConvertToReadInListDto(manufacturer))
                .ToList();
        }

        public async Task<bool> ManufacturerExistsAsync(string code)
        {
            return await context.Manufacturers.AnyAsync(manufacturer => manufacturer.Code == code);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void UpdateManufacturerAsync(Manufacturer manufacturer)
        {
            // No code in this implementation
        }
    }
}

﻿using CustomerVehicleManagement.Api.Data;
using CustomerVehicleManagement.Domain.Entities;
using CustomerVehicleManagement.Domain.Entities.Payables;
using CustomerVehicleManagement.Shared.Models.Payables.Vendors;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerVehicleManagement.Api.Payables.Vendors
{
    public class VendorRepository : IVendorRepository
    {
        private readonly ApplicationDbContext context;

        public VendorRepository(ApplicationDbContext context)
        {
            this.context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<Vendor> GetVendorEntityAsync(long id)
        {
            return await context.Vendors.FirstOrDefaultAsync(vendor => vendor.Id == id);
        }

        public async Task<IReadOnlyList<Vendor>> GetVendorEntitiesAsync(List<long> ids)
        {
            return await context.Vendors
                .Where(vendor => ids.Contains(vendor.Id))
                .ToListAsync();
        }

        public async Task AddVendorAsync(Vendor vendor)
        {
            if (vendor is not null)
                await context.AddAsync(vendor);
        }

        public async Task<VendorToRead> GetVendorAsync(long id)
        {
            var vendorFromContext = await context.Vendors
                //.Include(organization => organization.Phones)
                //.Include(organization => organization.Emails)
                //.Include(organization => organization.Contact)
                //    .ThenInclude(contact => contact.Phones)
                //.Include(organization => organization.Contact)
                //    .ThenInclude(contact => contact.Emails)
                .FirstOrDefaultAsync(vendor => vendor.Id == id);

            return vendorFromContext is not null
                ? new VendorToRead()
                {
                    Id = vendorFromContext.Id,
                    VendorCode = vendorFromContext.VendorCode,
                    Name = vendorFromContext.Name,
                    IsActive = vendorFromContext.IsActive
                }
                : null;
        }

        public async Task<IReadOnlyList<VendorToRead>> GetVendorsAsync()
        {
            IReadOnlyList<Vendor> vendorsFromContext = await context.Vendors
                .AsNoTracking()
                .ToListAsync();

            return vendorsFromContext
                .Select(vendor => new VendorToRead()
                {
                    Id = vendor.Id,
                    VendorCode = vendor.VendorCode,
                    Name = vendor.Name,
                    IsActive = vendor.IsActive
                }).ToList();
        }

        public async Task<IReadOnlyList<VendorToRead>> GetVendorsListAsync()
        {
            IReadOnlyList<Vendor> vendors = await context.Vendors
                .AsNoTracking()
                .ToListAsync();

            return vendors.Select(vendor => new VendorToRead
            {
                Id = vendor.Id,
                VendorCode = vendor.VendorCode,
                Name = vendor.Name,
                IsActive = vendor.IsActive
            }).ToList();
        }

        public void DeleteVendor(Vendor vendor)
        {
            context.Remove(vendor);
        }

        public async Task<bool> VendorExistsAsync(long id)
        {
            return await context.Vendors.AnyAsync(vendor => vendor.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}

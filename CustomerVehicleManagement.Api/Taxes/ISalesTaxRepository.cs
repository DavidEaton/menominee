﻿using CustomerVehicleManagement.Domain.Entities.Taxes;
using CustomerVehicleManagement.Shared.Models.Taxes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerVehicleManagement.Api.Taxes
{
    public interface ISalesTaxRepository
    {
        Task AddSalesTaxAsync(SalesTax salesTax);
        Task<SalesTax> GetSalesTaxEntityAsync(long id);
        Task<SalesTaxToRead> GetSalesTaxAsync(long id);
        Task<IReadOnlyList<SalesTaxToReadInList>> GetSalesTaxListAsync();
        Task<SalesTax> UpdateSalesTaxAsync(SalesTax salesTax);
        Task DeleteSalesTaxAsync(long id);
        Task<bool> SalesTaxExistsAsync(long id);
        Task<bool> SaveChangesAsync();
        void FixTrackingState();
        Task<IReadOnlyList<SalesTax>> GetSalesTaxEntities();
    }
}

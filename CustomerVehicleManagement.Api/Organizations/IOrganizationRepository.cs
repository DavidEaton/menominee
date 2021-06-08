﻿using CustomerVehicleManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerVehicleManagement.Api.Organizations
{
    public interface IOrganizationRepository
    {
        Task AddOrganizationAsync(Organization entity);
        Task<IEnumerable<OrganizationReadDto>> GetOrganizationsAsync();
        Task<OrganizationReadDto> GetOrganizationAsync(int id);
        Task<IEnumerable<OrganizationsInListDto>> GetOrganizationsListAsync();
        void UpdateOrganizationAsync(Organization entity);
        Task DeleteOrganizationAsync(int id);
        void FixTrackingState();
        Task<bool> OrganizationExistsAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<Organization> GetOrganizationEntityAsync(int id);
    }
}

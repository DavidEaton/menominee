﻿using CustomerVehicleManagement.Shared.Models;
using CustomerVehicleManagement.Shared.Models.Organizations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menominee.Client.Services
{
    public interface IOrganizationDataService
    {
        Task<IReadOnlyList<OrganizationToReadInList>> GetAllOrganizations();
        Task<OrganizationToRead> GetOrganization(long id);
        Task<OrganizationToRead> AddOrganization(OrganizationToWrite organization);
        Task UpdateOrganization(OrganizationToWrite organization, long id);
    }
}

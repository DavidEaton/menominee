﻿using Menominee.Common.Entities;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Menominee.Client.Services
{
    public class TenantDataService : ITenantDataService
    {
        private readonly HttpClient httpClient;

        public TenantDataService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Tenant> GetTenantAsync()
        {
            return await JsonSerializer.DeserializeAsync<Tenant>
                (await httpClient.GetStreamAsync($"tenants/"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}

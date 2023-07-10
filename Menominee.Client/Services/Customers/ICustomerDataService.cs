﻿using Menominee.Shared.Models.Customers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menominee.Client.Services.Customers
{
    public interface ICustomerDataService
    {
        Task<IReadOnlyList<CustomerToReadInList>> GetAllCustomers();
        Task<CustomerToRead> GetCustomer(long id);
        Task<CustomerToRead> AddCustomer(CustomerToWrite customer);
        Task UpdateCustomer(CustomerToWrite customer, long id);
        Task DeleteCustomer(long id);
    }
}

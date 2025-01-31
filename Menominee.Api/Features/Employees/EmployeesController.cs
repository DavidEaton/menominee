﻿using Menominee.Domain.Enums;
using Menominee.Shared;
using Menominee.Shared.Models.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menominee.Api.Features.Employees
{
    [Authorize(Policies.CanManageHumanResources)]
    public class EmployeesController : BaseApplicationController<EmployeesController>
    {
        public EmployeesController(ILogger<EmployeesController> logger) : base(logger) { }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<EmployeeToRead>>> GetAsync()
        {
            var list = new List<EmployeeToRead>()
            {
                { new EmployeeToRead() { Id = 1, Name="Hard-Coded list...", Hired=DateTime.Today.AddYears(-2), ShopRole = ShopRole.Admin.ToString() } },
                { new EmployeeToRead() { Id = 2, Name="...from EmployeesController", Hired=DateTime.Today.AddYears(-3), ShopRole = ShopRole.Advisor.ToString() } },
                { new EmployeeToRead() { Id = 3, Name="Lane L. Lones", Hired=DateTime.Today.AddYears(-1), ShopRole = ShopRole.Employee.ToString() } },
                { new EmployeeToRead() { Id = 4, Name="Hane H. Hones", Hired=DateTime.Today.AddYears(-4), ShopRole = ShopRole.Owner.ToString() } },
                { new EmployeeToRead() { Id = 4, Name="Bane B. Bones", Hired=DateTime.Today.AddYears(-4), ShopRole = ShopRole.Technician.ToString() } }
            };

            return await Task.FromResult(list);
        }
    }
}

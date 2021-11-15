﻿using CustomerVehicleManagement.Api.Data;
using CustomerVehicleManagement.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerVehicleManagement.Api.Users
{
    //[Authorize(Policies.CanManageUsers)]
    public class UsersController : ApplicationController
    {
        private readonly IConfiguration Configuration;
        private readonly UserContext UserContext;
        public UsersController(IConfiguration configuration,
                               UserContext userContext)
        {
            Configuration = configuration;
            UserContext = userContext;
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<UserToReadInList>> GetUsers()
        {
            var tenantId = GetTenantId();

            var users = new List<UserToReadInList>();

            using SqlConnection connection = new(Configuration[$"IDPSettings:Connection"]);
            try
            {
                connection.Open();
                string query = $"SELECT [Id], [UserName], [Email], [ShopRole] FROM [dbo].[AspNetUsers];";
                //string query = $"SELECT [Id], [UserName], [Email], [ShopRole] FROM [dbo].[AspNetUsers] WHERE [TenantId] = '{tenantId}';";
                using SqlCommand command = new(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        users.Add(
                        new UserToReadInList()
                        {
                            Id = (string)reader["Id"],
                            Username = reader["Username"].ToString(),
                            Name = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            ShopRole = (string)reader["ShopRole"]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.LogError($"Exception message from GetUsersAsync(): {ex.Message}");
                return null;
            }

            return Ok(users);
        }

        public Guid GetTenantId()
        {
            if (UserContext == null)
                return new Guid();

            var claims = UserContext.Claims;
            Guid tenantId;

            try
            {
                tenantId = Guid.Parse(claims.First(claim => claim.Type == "tenantId").Value);
            }
            catch (Exception ex)
            {
                //Logger.LogError($"Exception message from GetTenantId(): {ex.Message}");
                return new Guid();
            }

            return tenantId;
        }
    }
}

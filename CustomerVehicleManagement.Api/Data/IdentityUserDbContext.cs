﻿using Janco.Idp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Menominee.Common.Entities;

namespace CustomerVehicleManagement.Data
{
    public class IdentityUserDbContext : IdentityUserContext<ApplicationUser>
    {
        public IdentityUserDbContext(DbContextOptions<IdentityUserDbContext> options)
            : base(options)
        { }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
    }
}

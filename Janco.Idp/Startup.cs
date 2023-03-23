﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Services;
using Janco.Idp.Areas.Identity.Data;
using Janco.Idp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Janco.Idp
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        private readonly string CorsPolicyProduction = "CorsPolicyProduction";
        private readonly string CorsPolicyDevelopment = "CorsPolicyDevelopment";
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<IProfileService, LocalUserProfileService>();

            var builder = services.AddIdentityServer(options =>
            {
                // additional startup in Areas/Identity/IdentityHostingStartup.cs
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Configuration.GetSection("ApplicationSettings:Clients"))

                // Configure IdentityServer to use AspNetCore Identity membership
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<LocalUserProfileService>();


            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
                services.AddTransient<IEmailSender, DummyEmailSender>();
            }

            if (Environment.IsProduction())
                // not recommended for production - you need to store your key material somewhere secure
                builder.AddDeveloperSigningCredential();


            ConfigureCors(services);
        }

        private void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyProduction,
                builder =>
                {
                    builder.WithOrigins(
                           "https://menominee.azurewebsites.net",
                           "https://menominee-test.azurewebsites.net",
                           "https://menominee-stage.azurewebsites.net")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });

                if (Environment.IsDevelopment())
                {
                    options.AddPolicy(CorsPolicyDevelopment,
                                      builder =>
                                      {
                                          builder.WithOrigins()
                                                 .AllowAnyOrigin()
                                                 .AllowAnyMethod()
                                                 .AllowAnyHeader();
                                      });
                }
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(CorsPolicyDevelopment);
            }

            if (Environment.IsProduction())
                app.UseCors(CorsPolicyProduction);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}

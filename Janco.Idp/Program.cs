﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace Janco.Idp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // uncomment to write to Azure diagnostics stream
                //.WriteTo.File(
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.File(new JsonFormatter(), @"janco-idp-log.json", rollingInterval: RollingInterval.Day)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            try
            {
                Log.Information("Starting host...");
                var host = CreateHostBuilder(args).Build();

                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Build the current set of configuration to load values from
                    // JSON files and environment variables, including VaultName.
                    var builtConfig = config.Build();
                    string envName = context.HostingEnvironment.EnvironmentName;
                    config.AddJsonFile("idp.appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"idp.appsettings.{envName}.json", optional: true, reloadOnChange: true);
                    // Use VaultName from the configuration to create the full vault URI.
                    var vaultName = builtConfig["VaultName"];
                    if (!string.IsNullOrEmpty(vaultName))
                    {
                        Uri vaultUri = new Uri($"https://{vaultName}.vault.azure.net/");

                        // Load all secrets from the vault into configuration. This will automatically
                        // authenticate to the vault using a managed identity. If a managed identity
                        // is not available, it will check if Visual Studio and/or the Azure CLI are
                        // installed locally and see if they are configured with credentials that can
                        // access the vault.
                        config.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
                    }
                });
    }
}
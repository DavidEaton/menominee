using Blazored.Toast;
using CustomerVehicleManagement.Shared;
using Menominee.Client.Components.RepairOrders;
using Menominee.Client.MessageHandlers;
using Menominee.Client.Services;
using Menominee.Client.Services.CreditCards;
using Menominee.Client.Services.Customers;
using Menominee.Client.Services.Inventory;
using Menominee.Client.Services.Manufacturers;
using Menominee.Client.Services.Payables.Invoices;
using Menominee.Client.Services.Payables.PaymentMethods;
using Menominee.Client.Services.Payables.Vendors;
using Menominee.Client.Services.ProductCodes;
using Menominee.Client.Services.SaleCodes;
using Menominee.Client.Services.Taxes;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using System;
using System.Threading.Tasks;

namespace Menominee.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Add your Syncfusion license key for Blazor platform with corresponding Syncfusion NuGet version referred in project. For more information about license key see https://help.syncfusion.com/common/essential-studio/licensing/license-key.
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTQ3MzAyQDMxMzkyZTMzMmUzMGF5MU1kSEI2RnZMQWMxR3dqSlM4T2MvVFBWTFdBbEhzckF2TVJwSVlJVTQ9");
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTg1MzU1QDMxMzkyZTM0MmUzMGVFZWRZcnBURWU0L3NnaE1qdzlJT1h3NEx2N3ZOSmJ1RWx3aXh5SGlrVnc9");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddBlazoredToast();
            builder.Services.AddTelerikBlazor();

            builder.Services.AddScoped<LocalStorage>();

            builder.Services.AddTransient<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("OidcConfiguration", options.ProviderOptions);
                builder.Configuration.Bind("UserOptions", options.UserOptions);
            });

            builder.Services.AddAuthorizationCore(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    Policies.AdminOnly,
                    Policies.AdminPolicy());

                authorizationOptions.AddPolicy(
                    Policies.RequireAuthenticatedUser,
                    Policies.RequireAuthenticatedUserPolicy());

                authorizationOptions.AddPolicy(
                    Policies.CanManageHumanResources,
                    Policies.CanManageHumanResourcesPolicy());

                authorizationOptions.AddPolicy(
                    Policies.CanManageUsers,
                    Policies.CanManageUsersPolicy());

                authorizationOptions.AddPolicy(
                    Policies.FreeUser,
                    Policies.FreeUserPolicy());

                authorizationOptions.AddPolicy(
                    Policies.OwnerOnly,
                    Policies.OwnerPolicy());

                authorizationOptions.AddPolicy(
                    Policies.PaidUser,
                    Policies.PaidUserPolicy());

                authorizationOptions.AddPolicy(
                    Policies.TechniciansUser,
                    Policies.TechnicianUserPolicy());
            });

            var baseAddress = new Uri(builder.Configuration.GetValue<string>("ApiBaseUrl"));

            builder.Services.AddHttpClient<IUserDataService, UserDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IPersonDataService, PersonDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<ICustomerDataService, CustomerDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IOrganizationDataService, OrganizationDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IEmployeeDataService, EmployeeDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IVendorDataService, VendorDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IVendorInvoiceDataService, VendorInvoiceDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IVendorInvoicePaymentMethodDataService, VendorInvoicePaymentMethodDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IRepairOrderDataService, RepairOrderDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IManufacturerDataService, ManufacturerDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<ISaleCodeDataService, SaleCodeDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IProductCodeDataService, ProductCodeDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IInventoryItemDataService, InventoryItemDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<ICreditCardDataService, CreditCardDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<IExciseFeeDataService, ExciseFeeDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<ISalesTaxDataService, SalesTaxDataService>(
                client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<MenonineeApiAuthorizationMessageHandler>();

            await builder.Build().RunAsync();

        }
    }
}

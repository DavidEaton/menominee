﻿using CustomerVehicleManagement.Shared.Models.Manufacturers;
using CustomerVehicleManagement.Shared.Models.Payables.Invoices.Items;
using Menominee.Client.Services.Manufacturers;
using Menominee.Client.Services.ProductCodes;
using Menominee.Common.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Menominee.Client.Components.Payables
{
    public partial class VendorInvoiceItemEditor
    {
        [Inject]
        public IManufacturerDataService manufacturerDataService { get; set; }

        [Inject]
        public IProductCodeDataService productCodeDataService { get; set; }

        [Parameter]
        public VendorInvoiceItemToWrite Item { get; set; }

        [Parameter]
        public bool DialogVisible { get; set; }

        [Parameter]
        public FormMode Mode
        {
            get => formMode;
            set
            {
                formMode = value;
                if (formMode == FormMode.Add)
                    Title = "Add";
                else if (formMode == FormMode.Edit)
                    Title = "Edit";
                else
                    Title = "View";
                Title += " Item";
            }
        }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        private FormMode formMode;
        private IReadOnlyList<ManufacturerToReadInList> Manufacturers = null;
        //private IReadOnlyList<ProductCodeToReadInList> ProductCodes = new List<ProductCodeToReadInList>();
        private string SaleCode = string.Empty;
        private bool parametersSet = false;
        private long manufacturerId = 0;
        private long productCodeId = 0;

        public string Title { get; set; }
        
        private List<InvoiceItemType> itemTypes { get; set; } = new List<InvoiceItemType>();

        protected override async Task OnInitializedAsync()
        {
            Manufacturers = (await manufacturerDataService.GetAllManufacturersAsync())
                                                          .Where(mfr => mfr.Prefix?.Length > 0
                                                                     && mfr.Code != StaticManufacturerCodes.Package)
                                                          .OrderBy(mfr => mfr.Prefix)
                                                          .ToList();
            //Manufacturers.Insert(0, new ManufacturerToReadInList
            //{
            //    Id = 0,
            //    Prefix = "*N/A*",
            //    Name = "Non-Inventory Item"
            //});

            //manufacturerId = (await manufacturerDataService.GetManufacturerAsync(StaticManufacturerCodes.Miscellaneous)).Id;
            //ProductCodes = (await productCodeDataService.GetAllProductCodesAsync(manufacturerId)).ToList();
            //ProductCodes.OrderBy(pc => pc.Code);

            foreach (VendorInvoiceItemType itemType in Enum.GetValues(typeof(VendorInvoiceItemType)))
            {
                itemTypes.Add(new InvoiceItemType { Text = EnumExtensions.GetDisplayName(itemType), Value = itemType });
            }

            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (parametersSet)
                return;
            parametersSet = true;

            //if (Item?.MfrId .Manufacturer != null)
            if (Item?.Manufacturer != null)
            {
                manufacturerId = Item.Manufacturer.Id;
            }
            //if (Item?.ProductCode != null)
            //{
            //    productCodeId = Item.ProductCode.Id;
            //}

            //if (Item?.Part == null)
            //{
            //    Item.Part = new();
            //    Item.ItemType = InventoryItemType.Part;

            //    Title = "Add Part";
            //}

            await OnManufacturerChangeAsync();
        }

        private async Task OnManufacturerChangeAsync()
        {
            if (manufacturerId > 0 && Item.Manufacturer?.Id != manufacturerId)
            {
                Item.Manufacturer = ManufacturerHelper.ConvertReadToWriteDto(await manufacturerDataService.GetManufacturerAsync(manufacturerId));
            }

            //if (Item?.Manufacturer is not null)
            //{
            //    long savedProductCodeId = productCodeId;
            //    ProductCodes = (await productCodeDataService.GetAllProductCodesAsync(manufacturerId)).ToList();
            //    if (savedProductCodeId > 0 && Item.ProductCode?.Id == 0 && ProductCodes.Any(pc => pc.Id == savedProductCodeId) == true)
            //        Item.ProductCode = ProductCodeHelper.ConvertReadToWriteDto(await productCodeDataService.GetProductCodeAsync(savedProductCodeId));
            //    productCodeId = savedProductCodeId;
            //}
            //else
            //{
            //    productCodeId = 0;
            //    ProductCodes = new List<ProductCodeToReadInList>();
            //    Item.ProductCode = new();
            //}

            //await OnProductCodeChangeAsync();
        }

        public class InvoiceItemType
        {
            public string Text { get; set; }
            public VendorInvoiceItemType Value { get; set; }
        }
    }
}

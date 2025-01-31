﻿using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using System.Collections.Generic;

namespace Menominee.Client.Components.Inventory
{
    public partial class InventoryMenu
    {
        [Inject]
        public NavigationManager navigationManager { get; set; }

        private static string ModuleUrl = "/inventory";
        public int menuWidth { get; set; } = 229;

        public void OnItemSelected(MenuItem selectedItem)
        {
        }

        private List<MenuItem> menuItems = new()
        {
#pragma warning disable BL0005
            new MenuItem
            {
                Text = "Items",
                Id = "-1",
                Items = new List<MenuItem>
                {
                    new MenuItem { Text="Item List", Url=$"{ModuleUrl}/items/listing", Id=((int)InventoryMenuId.ItemList).ToString() },
                    new MenuItem { Text="Maintenance Items", Url=$"{ModuleUrl}/maintenanceitems", Id=((int)InventoryMenuId.MaintenanceItems).ToString() }
                },
                Url = ""
            },
            new MenuItem
            {
                Text = "Orders",
                Id = ((int)InventoryMenuId.Orders).ToString(),
                Url = $"{ModuleUrl}/orders"
            },
            new MenuItem
            {
                Text = "Reports",
                Id = "-1",//((int)InventoryMenuId.Reports).ToString(),
                Items = new List<MenuItem>
                {
                    new MenuItem { Text="Total Value Of Inventory", Url=$"{ModuleUrl}", Id=((int)InventoryMenuId.TotalValue).ToString() },
                    new MenuItem { Text="Top Sellers", Url=$"{ModuleUrl}", Id=((int)InventoryMenuId.TopSellers).ToString() }
                },
                Url = ""
            }
         };
#pragma warning restore BL0005
    }
}
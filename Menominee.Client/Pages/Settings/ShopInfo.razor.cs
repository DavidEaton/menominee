﻿using Microsoft.AspNetCore.Components;
using System.Net.Http;

namespace Menominee.Client.Pages.Settings
{
    public partial class ShopInfo
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        private NavigationManager navigationManager { get; set; }

        //[Parameter]
        //public int SelectedContent { get; set; }

        //[Parameter]
        //public EventCallback<int> ResetContent { get; set; }

        //private async Task SaveChanges()
        //{
        //    //SelectedContent = 1;
        //    await ResetContent.InvokeAsync(1);
        //}

        //private async Task DiscardChanges()
        //{
        //    //SelectedContent = 0;
        //    await ResetContent.InvokeAsync(0);
        //}

        private void SaveChanges()
        {
            navigationManager.NavigateTo("/settings");
        }

        private void DiscardChanges()
        {
            navigationManager.NavigateTo("/settings");
        }

        //public CompanyInformation Company { get; set; }
        //public State[] States { get; set; }

        //protected override async Task OnInitializedAsync()
        //{
        //    States = await HttpClient.GetFromJsonAsync<State[]>("sample-data/states.json");
        //    Company = await HttpClient.GetFromJsonAsync<CompanyInformation>("sample-data/companyInfo.json");
        //}
    }
}

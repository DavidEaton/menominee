﻿using CustomerVehicleManagement.Shared.Models.Payables.Vendors;
using Menominee.Client.Services.Payables.Vendors;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Blazor.Components;

namespace Menominee.Client.Components.Payables.Pages
{
    public partial class VendorListPage : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IVendorDataService VendorDataService { get; set; }

        //[Inject]
        //public ILogger<VendorList> Logger { get; set; }

        [Parameter]
        public long VendorToSelect { get; set; } = 0;

        public IReadOnlyList<VendorToRead> VendorsList;
        public IEnumerable<VendorToRead> SelectedList { get; set; } = Enumerable.Empty<VendorToRead>();
        public VendorToRead SelectedItem { get; set; }

        private bool showInactive { get; set; } = false;

        public TelerikGrid<VendorToRead> Grid { get; set; }

        private bool CanEdit { get; set; } = false;
        private bool CanDelete { get; set; } = false;

        private long selectedId;
        public long SelectedId
        {
            get => selectedId;
            set
            {
                selectedId = value;
                CanEdit = selectedId > 0;
                CanDelete = selectedId > 0;
            }
        }

        //[CascadingParameter]
        //IModalService ModalService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            VendorsList = (await VendorDataService.GetAllVendorsAsync()).ToList();

            if (VendorsList.Count > 0)
            {
                if (VendorToSelect == 0)
                {
                    SelectedItem = VendorsList.FirstOrDefault();
                }
                else
                {
                    SelectedItem = VendorsList.Where(x => x.Id == VendorToSelect).FirstOrDefault();
                }
                SelectedId = SelectedItem.Id;
                SelectedList = new List<VendorToRead> { SelectedItem };
            }
        }

        private void OnAdd()
        {
            NavigationManager.NavigateTo("payables/vendors/0");
        }

        //private async Task OnEdit()
        private void OnEdit()
        {
            //if (SelectedId == 0)
            //{
            //    var parameters = new ModalParameters();
            //    parameters.Add(nameof(ModalMessage.Message), "Please select a vendor to edit.");
            //    var modalMessage = ModalService.Show<ModalMessage>("Edit Vendor", parameters);
            //    await modalMessage.Result;
            //    return;
            //}

            NavigationManager.NavigateTo($"payables/vendors/{SelectedId}");
        }

        private void OnDelete()
        {
            //NavigationManager.NavigateTo($"payables/vendors/{SelectedId}");
        }

        private void OnDone()
        {
            NavigationManager.NavigateTo("payables");
        }

        //public void OnRowSelected(RowSelectEventArgs<VendorToRead> args)
        //{
        //    SelectedId = args.Data.Id;
        //}

        protected void OnSelect(IEnumerable<VendorToRead> vendors)
        {
            SelectedItem = vendors.FirstOrDefault();
            SelectedList = new List<VendorToRead> { SelectedItem };
        }

        private void OnRowSelected(GridRowClickEventArgs args)
        {
            SelectedId = (args.Item as VendorToRead).Id;
        }
    }
}

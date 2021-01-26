﻿using Client.Models;
using Client.Services;
using CustomerVehicleManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using SharedKernel;
using SharedKernel.Enums;
using System;
using System.Threading.Tasks;

namespace Client.Components
{
    public partial class AddPersonDialog
    {
        private PersonAddProperties PersonAdd { get; set; }
        private PersonAddDto Person { get; set; }
        public string Message { get; set; }
        public EntityType EntityType { get; set; }

        protected PersonNameForm PersonNameForm { get; set; }
        protected AddressForm AddressForm { get; set; }

        [Inject]
        public IPersonDataService PersonDataService { get; set; }
        public bool ShowDialog { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public void Show()
        {
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            PersonAdd = new PersonAddProperties();
        }

        protected async Task HandleValidSubmit()
        {
            Message = string.Empty;

            if (FormIsValid())
            {
                Person = new PersonAddDto(PersonAdd.Name, PersonAdd.Gender, PersonAdd.Birthday, PersonAdd.Address);
                await PersonDataService.AddPerson(Person);
                ShowDialog = false;

                await CloseEventCallback.InvokeAsync(true);
                StateHasChanged();
            }

            else
            {
                // TODO: Alert user that form is invalid
                Message = "Please complete all required items";
            }
        }

        private bool FormIsValid()
        {
            if (PersonAdd.Name != null)
                return true;

            return false;
        }

        public void PersonNameForm_OnPersonNameChanged()
        {
            PersonAdd.Name = PersonNameForm.PersonName;
            //StateHasChanged();
        }

        public void AddressForm_OnAddressChanged()
        {
            PersonAdd.Address = AddressForm.EntityAddress;
        }

        private class PersonAddProperties
        {
            internal PersonName Name { get; set; }
            public Gender Gender { get; set; }
            public DateTime? Birthday { get; set; }
            internal Address Address { get; set; }
        }
    }
}

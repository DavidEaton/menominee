﻿using Microsoft.AspNetCore.Components;
using SharedKernel.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Client.Components
{
    public partial class PersonNameForm : ComponentBase
    {
        PersonNameProperties Name { get; set; } = new PersonNameProperties();
        public PersonName PersonName { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        protected async Task Validate()
        {
            if (Name.IsValid)
            {
                PersonName = new PersonName(Name.LastName, Name.FirstName, Name.MiddleName);
                await CloseEventCallback.InvokeAsync(true);
                StateHasChanged();
            }
            else
            {
                // TODO: display invalid message(s)

            }
        }

        private class PersonNameProperties
        {
            [Required(ErrorMessage = "Last Name is required")]
            [StringLength(100, ErrorMessage = "Please limit Last Name to 100 characters")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "First Name is required")]
            [StringLength(100, ErrorMessage = "Please limit First Name to 100 characters")]
            public string FirstName { get; set; }

            public string MiddleName { get; set; }

            public bool IsValid { get => !string.IsNullOrWhiteSpace(LastName) && !string.IsNullOrWhiteSpace(FirstName); }

        }
    }


}

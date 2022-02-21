﻿using Blazored.Toast.Services;
using CustomerVehicleManagement.Shared.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Menominee.Client.Services
{
    public class PersonDataService : IPersonDataService
    {
        private readonly HttpClient httpClient;
        private readonly IToastService toastService;
        private const string MediaType = "application/json";
        private const string UriSegment = "api/persons";

        public PersonDataService(HttpClient httpClient,
                                 IToastService toastService)
        {
            this.httpClient = httpClient;
            this.toastService = toastService;
        }

        public async Task<PersonToRead> AddPerson(PersonToWrite person)
        {
            var content = new StringContent(JsonSerializer.Serialize(person), Encoding.UTF8, MediaType);
            var response = await httpClient.PostAsync(UriSegment, content);

            if (response.IsSuccessStatusCode)
            {
                toastService.ShowSuccess($"{person.Name.LastFirstMiddle} added successfully", "Added");
                return await JsonSerializer.DeserializeAsync<PersonToRead>(await response.Content.ReadAsStreamAsync());
            }

            toastService.ShowError($"{person.Name.LastFirstMiddle} failed to add. {response.ReasonPhrase}.", "Add Failed");
            return null;
        }

        public async Task<IReadOnlyList<PersonToReadInList>> GetAllPersons()
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IReadOnlyList<PersonToReadInList>>($"{UriSegment}/list");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message :{ex.Message}");
            }

            return null;
        }

        public async Task<PersonToRead> GetPersonDetails(long id)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<PersonToRead>(UriSegment + $"/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message :{ex.Message}");
            }
            return null;
        }

        public async Task DeletePerson(long id)
        {
            try
            {
                await httpClient.DeleteAsync($"{UriSegment}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message :{ex.Message}");
            }
        }

        public async Task UpdatePerson(PersonToWrite person, long id)
        {
            var content = new StringContent(JsonSerializer.Serialize(person), Encoding.UTF8, MediaType);
            var response = await httpClient.PutAsync(UriSegment + $"/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                toastService.ShowSuccess($"{person.Name.LastFirstMiddle} updated successfully", "Saved");
                return;
            }

            toastService.ShowError($"{person.Name.LastFirstMiddle} failed to update", "Save Failed");
        }

        public async Task<PersonToRead> GetPerson(long id)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<PersonToRead>(UriSegment + $"/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message :{ex.Message}");
            }
            return null;
        }


    }

}

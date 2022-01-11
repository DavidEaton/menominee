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
    public class UserDataService : IUserDataService
    {
        private readonly HttpClient httpClient;
        private const string UriSegment = "users";
        private const string MediaType = "application/json";
        private readonly IToastService toastService;

        public UserDataService(HttpClient httpClient, IToastService toastService)
        {
            this.httpClient = httpClient;
            this.toastService = toastService;
        }

        public async Task<IReadOnlyList<UserToRead>> GetAll()
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IReadOnlyList<UserToRead>>($"{UriSegment}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message :{ex.Message}");
            }

            return null;
        }
        public async Task<RegisterUserResult> Register(RegisterUser registerModel)
        {
            var content = new StringContent(JsonSerializer.Serialize(registerModel), Encoding.UTF8, MediaType);
            var response = await httpClient.PostAsync(UriSegment, content);

            if (response.IsSuccessStatusCode)
            {
                toastService.ShowSuccess($"{registerModel.Email} added successfully", "Added");
                return await JsonSerializer.DeserializeAsync<RegisterUserResult>(await response.Content.ReadAsStreamAsync());
            }

            toastService.ShowError($"{registerModel.Email} failed to add. {response.ReasonPhrase}.", "Add Failed");
            return null;
        }

        public async Task UpdateUser(RegisterUser registerUser, long id)
        {
            var content = new StringContent(JsonSerializer.Serialize(registerUser), Encoding.UTF8, MediaType);
            var response = await httpClient.PutAsync(UriSegment + $"/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                toastService.ShowSuccess($"{registerUser.Email} updated successfully", "Saved");
                return;
            }

            toastService.ShowError($"{registerUser.Email} failed to update", "Save Failed");
        }
    }
}

﻿using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;

namespace Menominee.Client.Services
{
    public class LocalStorage
    {
        // Used by Telerik Grid to store and retrieve user GridState
        protected IJSRuntime JSRuntimeInstance { get; set; }

        public LocalStorage(IJSRuntime jsRuntime)
        {
            JSRuntimeInstance = jsRuntime;
        }

        public ValueTask SetItem(string key, object data)
        {
            return JSRuntimeInstance.InvokeVoidAsync(
                "localStorage.setItem",
                new object[] {
                key,
                JsonSerializer.Serialize(data)
                });
        }

        public async Task<T> GetItem<T>(string key)
        {
            var data = await JSRuntimeInstance.InvokeAsync<string>("localStorage.getItem", key);
            if (!string.IsNullOrEmpty(data))
            {
                return JsonSerializer.Deserialize<T>(data);
            }

            return default;
        }

        public ValueTask RemoveItem(string key)
        {
            return JSRuntimeInstance.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}

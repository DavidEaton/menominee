﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Threading.Tasks;

namespace Menominee.Client.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        SignOutSessionStateManager SignOutManager { get; set; }

        [CascadingParameter(Name = "MainLayout")]
        MainLayout MainLayout { get; set; }

        private bool displayIsLarge = false;

        private string GetBreakpoint()
        {
            if (MainLayout != null)
            {
                Console.WriteLine(MainLayout.DrawerExpanded ? "(min-width: 900px)" : "(min-width: 770px)");
                return MainLayout.DrawerExpanded ? "(min-width: 900px)" : "(min-width: 770px)";
            }
            else
            {
                Console.WriteLine("MainLayout null");
                return "";
            }
        }

        public void MediaQueryChange(bool matchesMediaQuery)
        {
            displayIsLarge = matchesMediaQuery;
            Console.WriteLine(matchesMediaQuery);
        }

        private async Task BeginSignOut(MouseEventArgs args)
        {
            await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("authentication/logout");
        }

        private void BeginSignIn(MouseEventArgs args)
        {
            Navigation.NavigateTo("authentication/login");
        }
    }
}

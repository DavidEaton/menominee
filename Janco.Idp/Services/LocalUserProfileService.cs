﻿using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Janco.Idp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Janco.Idp.Services
{
    public class LocalUserProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public LocalUserProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);
            var claims = principal.Claims.ToList();

            // Add user claims
            if (user.TenantId != Guid.Empty)
                claims.Add(new Claim("tenantId", user.TenantId.ToString()));

            if (user.TenantName != string.Empty)
                claims.Add(new Claim("tenantName", user.TenantName));

            if (user.Role != null)
                claims.Add(new Claim("role", user.Role.ToString()));

            claims.Add(new Claim("ShopRole", user.ShopRole.ToString()));

            //if (user.SubscriptionLevel != string.Empty)
            //claims.Add(new Claim("subscriptionLevel", user.SubscriptionLevel.ToString()));

            //if (user.SubscribedProducts != string.Empty)
            //claims.Add(new Claim("subscribedProducts", user.SubscribedProducts.ToString()));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }

}

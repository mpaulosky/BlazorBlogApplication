// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IdentitySeeder.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shared.Entities;

namespace Web.Services;

/// <summary>
/// Helper for seeding identity roles and an initial user.
/// </summary>
public static class IdentitySeeder
{
    /// <summary>
    /// Ensure the roles defined in <see cref="RoleNames"/> exist.
    /// </summary>
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(RoleNames.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleNames.Admin));
        }

        if (!await roleManager.RoleExistsAsync(RoleNames.Author))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleNames.Author));
        }
    }

    /// <summary>
    /// Create a default user if one does not exist and assign the default role (Author).
    /// </summary>
    public static async Task SeedDefaultUserAsync(IServiceProvider services, string email, string password)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = "Default User"
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Default role for newly created users is Author
                await userManager.AddToRoleAsync(user, RoleNames.Author);
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(user, RoleNames.Author))
            {
                await userManager.AddToRoleAsync(user, RoleNames.Author);
            }
        }
    }
}

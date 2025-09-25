// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IdentitySeeder.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

using Microsoft.AspNetCore.Identity;

namespace Web.Services;

/// <summary>
///   Helper for seeding identity roles and an initial user.
/// </summary>
public static class IdentitySeeder
{

	/// <summary>
	///   Ensure the roles defined in <see cref="RoleNames" /> exist.
	/// </summary>
	public static async Task SeedRolesAsync(IServiceProvider services)
	{
		using IServiceScope scope = services.CreateScope();
		RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

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
	///   Create a default user if one does not exist and assign the default role (Author).
	/// </summary>
	public static async Task SeedDefaultUserAsync(IServiceProvider services, string email, string password)
	{
		using IServiceScope scope = services.CreateScope();
		UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		ApplicationUser? user = await userManager.FindByEmailAsync(email);

		if (user is null)
		{
			user = new ApplicationUser
			{
					UserName = email, Email = email, EmailConfirmed = true, DisplayName = "Default User"
			};

			IdentityResult result = await userManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				// The default role for newly created users is Author
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
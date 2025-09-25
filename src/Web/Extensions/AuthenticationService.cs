// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AuthenticationService.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

// Auth0 removed - using Microsoft Identity instead

using Microsoft.AspNetCore.Identity;

namespace Web.Extensions;

/// <summary>
///   IServiceCollectionExtensions
/// </summary>
public static partial class ServiceCollectionExtensions
{

	/// <summary>
	///   Add Authentication Services
	/// </summary>
	/// <param name="services">IServiceCollection</param>
	/// <param name="config">ConfigurationManager</param>
	public static void AddAuthenticationService(
			this IServiceCollection services,
			ConfigurationManager config)
	{

		// Use Microsoft Identity with default cookie authentication for Blazor Server
		services.AddCascadingAuthenticationState();

		// Let AddIdentity register the cookie scheme for Identity. Avoid adding
		// the cookie scheme here to prevent duplicate scheme registration when
		// Identity also configures it.
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
			options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
		});

		// Add Identity stores using EF Core Identity setup already present in ApplicationDbContext
		services.AddIdentity<ApplicationUser, IdentityRole>(options =>
				{
					// Configure password/lockout settings conservatively for security; override in appsettings if needed
					options.Password.RequireDigit = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequiredLength = 6;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddSignInManager();

	}

}
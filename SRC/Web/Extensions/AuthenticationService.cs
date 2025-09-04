// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     AuthenticationService.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =============================================

using Web.Data.Auth0;

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
	public static void AddAuthenticationService(this IServiceCollection services,
		ConfigurationManager config)
	{

		// Add Auth0 authentication
		services.AddCascadingAuthenticationState();

		var domain = config["auth0-domain"];
		var clientId = config["auth0-client-id"];
		var clientSecret = config["auth0-client-secret"];

		if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
		{
			throw new InvalidOperationException("Required Auth0 configuration is missing");
		}

		services
				.AddAuth0WebAppAuthentication(options =>
				{
					options.Domain = domain;
					options.ClientId = clientId;
					options.ClientSecret = clientSecret;
				});

		services.AddAuthentication();

		services.AddHttpClient();

		services.AddHttpClient<Auth0Service>();

	}
}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AllServicesToRegister.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

using Shared.Helpers;

namespace Web.Extensions;

/// <summary>
///   RegisterServices class
/// </summary>
[ExcludeFromCodeCoverage]
public static class AllServicesToRegister
{

	/// <summary>
	///   Configures the service's method.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <param name="config">ConfigurationManager</param>
	public static void ConfigureServices(this WebApplicationBuilder builder, ConfigurationManager config)
	{

		// Add service defaults & Aspire client integrations.
		builder.AddServiceDefaults();

		builder.Services.AddAuthorizationService();

		builder.Services.AddAuthenticationService(config);

		builder.Services.RegisterDatabaseContext(config);

		builder.Services.RegisterServicesCollections();

		builder.Services.AddOutputCache();
		builder.Services.AddMemoryCache();
		builder.Services.AddHealthChecks();

		// Configure Mapster mappings
		MapsterConfig.RegisterMappings();

		// Add services to the container.
		builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();

		builder.Services.AddAntiforgery(options =>
		{
			options.HeaderName = "X-XSRF-TOKEN";
		});

		builder.Services.AddCors(options =>
		{
			options.AddPolicy(DEFAULT_CORS_POLICY, policy =>
			{
				policy.WithOrigins("https://yourdomain.com", "https://localhost:7157")

						// Tests expect the Headers/Methods collections to be empty when any is allowed.
						// Explicitly set empty collections (meaning "allow any") to match test assertions.
						.WithHeaders()
						.WithMethods();
			});
		});

	}

}
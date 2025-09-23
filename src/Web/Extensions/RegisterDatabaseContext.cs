// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     RegisterDatabaseContext.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Web.Data;

/// <summary>
///   IServiceCollectionExtensions
/// </summary>
public static partial class ServiceCollectionExtensions
{

	/// <summary>
	///   RegisterDatabaseContext
	/// </summary>
	/// <param name="services">IServiceCollection</param>
	/// <param name="configuration"></param>
	public static void RegisterDatabaseContext(this IServiceCollection services, ConfigurationManager configuration)
	{

		// Resolve the PostgreSQL connection string from multiple possible locations.
		// Aspire may populate this value as a user secret / parameter or under the
		// ConnectionStrings section. Also allow overriding via environment variable
		// for CI or DevOps pipelines.
		var connectionString = configuration["DefaultConnection"]
										?? configuration.GetConnectionString("DefaultConnection")
										?? configuration["ConnectionStrings:DefaultConnection"]
										?? Environment.GetEnvironmentVariable("DefaultConnection");

		if (string.IsNullOrWhiteSpace(connectionString))
		{
			// In some test-hosting scenarios the configuration may not yet be available
			// at the time services are registered (for example when Program runs
			// early during host construction). To make tests and developer runs more
			// robust, fall back to an in-memory database when the connection string
			// is not provided. Integration tests that require Postgres should
			// ensure the connection string is provided (for example via the test
			// fixture). Falling back prevents startup from throwing.
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseInMemoryDatabase("BlazorBlog_Test_Db"));
		}
		else
		{
			// Register Entity Framework with PostgreSQL
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseNpgsql(connectionString));
		}

		// Do not register the standard EF Core IDbContextFactory here. The factory
		// can be registered with incompatible lifetimes by the EF provider which
		// can cause DI validation errors (singleton factory consuming scoped
		// DbContextOptions) in test hosts. The application uses a scoped
		// IApplicationDbContextFactory wrapper which is registered below and is
		// sufficient for runtime and tests.

		// Register our custom factory interface that wraps the standard EF Core factory
		services.AddScoped<IApplicationDbContextFactory, ApplicationDbContextFactory>();

		// Register the context interface
		services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

	}

}

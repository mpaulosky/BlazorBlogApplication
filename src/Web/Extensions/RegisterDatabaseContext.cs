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
			throw new InvalidOperationException("Required configuration 'DefaultConnection' is missing");
		}

		// Register Entity Framework with PostgreSQL
		services.AddDbContext<ArticleDbContext>(options =>
			options.UseNpgsql(connectionString));

		// Register the standard EF Core factory
		services.AddDbContextFactory<ArticleDbContext>(options =>
			options.UseNpgsql(connectionString));

		// Register our custom factory interface that wraps the standard EF Core factory
		services.AddScoped<IArticleDbContextFactory, ArticleDbContextFactory>();

		// Register the context interface
		services.AddScoped<IArticleDbContext>(sp => sp.GetRequiredService<ArticleDbContext>());

	}

}

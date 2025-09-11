// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     RegisterDatabaseContext.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Extensions;

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

		// Resolve the MongoDB connection string from multiple possible locations.
		// Aspire may populate this value as a user secret / parameter or under the
		// ConnectionStrings section. Also allow overriding via environment variable
		// for CI or DevOps pipelines.
		var mongoConn = configuration["mongoDb-connection"]
										?? configuration.GetConnectionString("mongoDb-connection")
										?? configuration["ConnectionStrings:mongoDb-connection"]
										?? Environment.GetEnvironmentVariable("mongoDb-connection");

		if (string.IsNullOrWhiteSpace(mongoConn))
		{
			throw new InvalidOperationException("Required configuration 'mongoDb-connection' is missing");
		}

		// Ensure SCRAM-SHA-256 is used for authentication
		if (!mongoConn.Contains("authMechanism=SCRAM-SHA-256"))
		{
			mongoConn += mongoConn.Contains("?") ? "&authMechanism=SCRAM-SHA-256" : "?authMechanism=SCRAM-SHA-256";
		}

		services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));

		// Register the MongoDB context factory
		services.AddSingleton<IMyBlogContextFactory, MyBlogContextFactory>();

		// Register the MongoDB context as scoped using the factory to ensure per-request lifetime
		// The factory exposes a synchronous CreateContext() convenience method that wraps
		// the async CreateContext(CancellationToken) implementation. Use the sync overload
		// here because DI doesn't support async factories.
		services.AddScoped<IMyBlogContext>(sp =>
		{
			var factory = sp.GetRequiredService<IMyBlogContextFactory>();

			return factory.CreateContext();
		});

		// Register the MongoDB context as scoped using the factory to ensure per-request lifetime
		// services.AddScoped<IMyBlogContext>(sp =>
		// {
		// 	var factory = sp.GetRequiredService<IMyBlogContextFactory>();
		// 	// Block synchronously here since DI does not support async factories
		// 	return factory.CreateAsync().GetAwaiter().GetResult();
		// });

	}

}

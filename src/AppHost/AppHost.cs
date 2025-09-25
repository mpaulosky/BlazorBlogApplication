// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppHost.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  AppHost
// =======================================================

using System.Diagnostics.CodeAnalysis;

using Projects;

using static Shared.Services;

namespace AppHost;

/// <summary>
///   Entry point for the distributed application host. Configures and runs the Aspire projects.
/// </summary>
[ExcludeFromCodeCoverage]
public static class AppHostEntryPoint
{

	/// <summary>
	///   Configures and runs the distributed application.
	/// </summary>
	/// <param name="args">Command-line arguments.</param>
	public static void Main(string[] args)
	{
		IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

		IResourceBuilder<RedisResource> cache = builder.AddRedis(CACHE)
				.WithLifetime(ContainerLifetime.Persistent)
				.WithRedisInsight();

		IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres(SERVER)
				.WithLifetime(ContainerLifetime.Persistent)
				.WithPgAdmin()
				.WithDataVolume(isReadOnly: false);

		IResourceBuilder<PostgresDatabaseResource> postgresDb = postgres.AddDatabase(ARTICLE_DATABASE);

		builder.AddProject<Web>(WEBSITE)
				.WithReference(postgresDb)
				.WithReference(cache)
				.WaitFor(postgres)
				.WithExternalHttpEndpoints();

		builder.Build().Run();
	}

}
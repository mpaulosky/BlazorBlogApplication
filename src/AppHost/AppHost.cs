// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppHost.cs
// Company :       mpaulosky
// Author :        Matthew
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
		var builder = DistributedApplication.CreateBuilder(args);

		var cache = builder.AddRedis(Cache)
				.WithLifetime(ContainerLifetime.Persistent)
				.WithRedisInsight();

		var postgres = builder.AddPostgres(Server)
				.WithLifetime(ContainerLifetime.Persistent)
				.WithPgAdmin()
				.WithDataVolume(isReadOnly: false);

		var postgresDb = postgres.AddDatabase(ArticleDatabase);

		builder.AddProject<Web>(Website)
				.WithReference(postgresDb)
				.WithReference(cache)
				.WaitFor(postgres)
				.WithExternalHttpEndpoints();

		builder.Build().Run();
	}

}

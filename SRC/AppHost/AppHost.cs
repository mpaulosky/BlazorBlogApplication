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

		// Define constants for resources
		var auth0Domain = builder.AddParameter("auth0-domain", true)
				.WithDescription("The Auth0 domain for authentication.");

		var auth0Client = builder.AddParameter("auth0-client-id", true)
				.WithDescription("The Auth0 client ID for authentication.");

		var auth0ClientSecret = builder.AddParameter("auth0-client-secret", true)
				.WithDescription("The Auth0 client secret for authentication.");

		var mongoDbConnection = builder.AddParameter("mongoDb-connection", true)
				.WithDescription("The MongoDB connection string.");

		var database = builder.AddMongoDB(SERVER)
				.WithLifetime(ContainerLifetime.Persistent)
				.WithDataVolume($"{SERVER}-data")
				.WithMongoExpress()
				.AddDatabase(DATABASE);

		builder.AddProject<Web>(WEBSITE)
				.WithExternalHttpEndpoints()
				.WithHttpHealthCheck("/health")
				.WithReference(database)
				.WaitFor(database)
				.WithEnvironment("mongoDb-connection", mongoDbConnection)
				.WithEnvironment("auth0-domain", auth0Domain)
				.WithEnvironment("auth0-client-id", auth0Client)
				.WithEnvironment("auth0-client-secret", auth0ClientSecret);

		builder.Build().Run();
	}

}

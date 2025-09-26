// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationDbContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

using Microsoft.EntityFrameworkCore.Design;

namespace Web.Data;

/// <summary>
///   A design-time factory for <see cref="ApplicationDbContext" /> so tools (migrations, EF CLI)
///   can create the DbContext when building.
/// </summary>
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>,
		IApplicationDbContextFactory
{

	public ApplicationDbContext CreateDbContext()
	{
		// Build a minimal configuration that mirrors how the app resolves the connection string.
		IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", true)
				.AddJsonFile("appsettings.Development.json", true)
				.AddEnvironmentVariables();

		IConfigurationRoot configuration = builder.Build();

		string? connectionString = configuration["DefaultConnection"]
															?? configuration.GetConnectionString("DefaultConnection")
															?? configuration["ConnectionStrings:DefaultConnection"]
															?? Environment.GetEnvironmentVariable("DefaultConnection");

		DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new ();

		if (string.IsNullOrWhiteSpace(connectionString))
		{
			// For design-time tools (like EF migrations), we need a real database provider
			// instead of in-memory. Use a development PostgreSQL connection string.
			// In production, the connection string should be properly configured.
			connectionString = "Host=localhost;Database=BlazorBlogDev;Username=dev;Password=dev";
		}

		optionsBuilder.UseNpgsql(connectionString, options =>
		{
			// Specify the migrations directory relative to the project root
			options.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
		});

		return new ApplicationDbContext(optionsBuilder.Options);
	}

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		// For design-time tools (like EF migrations), delegate to the parameterless method
		// since we don't typically use the args parameter for simple scenarios
		return CreateDbContext();
	}

}
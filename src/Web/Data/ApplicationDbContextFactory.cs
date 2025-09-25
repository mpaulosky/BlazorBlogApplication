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
			// When running unit tests or design-time tools in environments without a
			// configured DefaultConnection, fall back to an in-memory provider so
			// tests that instantiate this factory succeed. This keeps design-time
			// behavior convenient while still allowing production to require a real
			// connection string via RegisterDatabaseContext.
			optionsBuilder.UseInMemoryDatabase("DesignTimeFallback");

			return new ApplicationDbContext(optionsBuilder.Options);
		}

		optionsBuilder.UseNpgsql(connectionString);

		return new ApplicationDbContext(optionsBuilder.Options);
	}

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		throw new NotImplementedException();
	}

}
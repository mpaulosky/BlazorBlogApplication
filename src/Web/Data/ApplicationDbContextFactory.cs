// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationDbContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

/// <summary>
/// A design-time factory for <see cref="ApplicationDbContext"/> so tools (migrations, EF CLI)
/// can create the DbContext when building.
/// </summary>
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>,
		IApplicationDbContextFactory
{

	public ApplicationDbContext CreateDbContext()
	{
		// Build a minimal configuration that mirrors how the app resolves the connection string.
		var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true)
				.AddJsonFile("appsettings.Development.json", optional: true)
				.AddEnvironmentVariables();

		var configuration = builder.Build();

		var connectionString = configuration["DefaultConnection"]
													?? configuration.GetConnectionString("DefaultConnection")
													?? configuration["ConnectionStrings:DefaultConnection"]
													?? Environment.GetEnvironmentVariable("DefaultConnection");

		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

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
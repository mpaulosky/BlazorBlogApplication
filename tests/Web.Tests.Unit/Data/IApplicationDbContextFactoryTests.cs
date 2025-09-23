// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IApplicationDbContextFactoryTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Data;

using Microsoft.EntityFrameworkCore;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(IApplicationDbContextFactory))]
public class IApplicationDbContextFactoryTests
{
	[Fact]
	public void CreateDbContext_WithValidOptions_ReturnsInitializedContext()
	{
		// Arrange
		var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

		// Ensure a fallback connection string is available for the factory lookup
		Environment.SetEnvironmentVariable("DefaultConnection", "Host=localhost;Database=Test;Username=Test;Password=Test");
		var factory = new Web.Data.ApplicationDbContextFactory();
		var result = factory.CreateDbContext();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApplicationDbContext>();
	}

	[Fact]
	public void Constructor_Default_Is_Constructible()
	{
		// Act
		var action = () => new Web.Data.ApplicationDbContextFactory();

		// Assert - factory should be constructible without throwing
		action.Should().NotThrow();
	}
}
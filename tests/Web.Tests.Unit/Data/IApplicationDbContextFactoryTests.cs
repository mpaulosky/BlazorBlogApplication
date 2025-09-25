// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IApplicationDbContextFactoryTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Data;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(IApplicationDbContextFactory))]
public class IApplicationDbContextFactoryTests
{

	[Fact]
	public void CreateDbContext_WithValidOptions_ReturnsInitializedContext()
	{
		// Arrange
		DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

		// Ensure a fallback connection string is available for the factory lookup
		Environment.SetEnvironmentVariable("DefaultConnection", "Host=localhost;Database=Test;Username=Test;Password=Test");
		ApplicationDbContextFactory factory = new ();
		ApplicationDbContext result = factory.CreateDbContext();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApplicationDbContext>();
	}

	[Fact]
	public void Constructor_Default_Is_Constructible()
	{
		// Act
		Func<ApplicationDbContextFactory> action = () => new ApplicationDbContextFactory();

		// Assert - factory should be constructible without throwing
		action.Should().NotThrow();
	}

}
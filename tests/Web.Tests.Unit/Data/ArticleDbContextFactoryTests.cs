// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleDbContextFactoryTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using System.Threading.Tasks;

namespace Web.Data;

using Microsoft.EntityFrameworkCore;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(ArticleDbContextFactory))]
public class ArticleDbContextFactoryTests
{
	[Fact]
	public void CreateDbContext_WithValidOptions_ReturnsInitializedContext()
	{
		// Arrange
		var options = new DbContextOptionsBuilder<ArticleDbContext>().Options;
		var factory = new ArticleDbContextFactory(options);

		// Act
		var result = factory.CreateDbContext();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ArticleDbContext>();
	}

	[Fact]
	public void Constructor_WithNullOptions_ThrowsArgumentNullException()
	{
		// Act
		var action = () => new ArticleDbContextFactory(null!);

		// Assert
		action.Should().Throw<ArgumentNullException>()
				.WithParameterName("options");
	}
}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetArticleHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Articles.ArticleDetails;

namespace Web.Features.Articles;

/// <summary>
/// Integration tests for the GetArticle.Handler class.
/// Tests the complete workflow of getting articles from the database with edge cases.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetArticle.Handler))]
[Collection("Test Collection")]
public class GetArticleHandlerTests
{
private readonly WebTestFactory _factory;
private readonly ILogger<GetArticle.Handler> _logger;
private readonly IApplicationDbContextFactory _contextFactory;
private string _testUserId = string.Empty;
private Guid _testCategoryId;

public GetArticleHandlerTests(WebTestFactory factory)
{
_factory = factory;
using var scope = _factory.Services.CreateScope();
_logger = scope.ServiceProvider.GetRequiredService<ILogger<GetArticle.Handler>>();
_contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
}

[Fact]
public async Task HandleAsync_WithEmptyGuid_ReturnsFailureResult()
{
// Arrange  
await _factory.ResetDatabaseAsync();
var handler = new GetArticle.Handler(_contextFactory, _logger);

// Act
var result = await handler.HandleAsync(Guid.Empty);

// Assert
result.Should().NotBeNull();
result.Success.Should().BeFalse();
result.Error.Should().NotBeNullOrEmpty();
}

[Fact]
public async Task HandleAsync_WithNonExistentId_ReturnsFailureResult()
{
// Arrange  
await _factory.ResetDatabaseAsync();
var handler = new GetArticle.Handler(_contextFactory, _logger);
var nonExistentId = Guid.NewGuid();

// Act
var result = await handler.HandleAsync(nonExistentId);

// Assert
result.Should().NotBeNull();
result.Success.Should().BeFalse();
result.Error.Should().NotBeNullOrEmpty();
}

[Fact]
public async Task HandleAsync_WithValidExistingArticle_ReturnsSuccessWithCompleteDetails()
{
// Arrange
var articleId = await CreateValidArticleAsync();
var handler = new GetArticle.Handler(_contextFactory, _logger);

// Act
var result = await handler.HandleAsync(articleId);

// Assert
result.Should().NotBeNull();
result.Success.Should().BeTrue();
result.Error.Should().BeNullOrEmpty();
result.Value.Should().NotBeNull();

// Verify article details
result.Value.Id.Should().Be(articleId);
result.Value.Title.Should().Be("Test Article Title");
result.Value.Introduction.Should().Be("Test Introduction");
result.Value.Content.Should().Be("Test Content for the article");
result.Value.IsPublished.Should().BeTrue();
result.Value.PublishedOn.Should().NotBeNull();

// Verify relationships were loaded via Include()
result.Value.Author.Should().NotBeNull();
result.Value.Author.DisplayName.Should().Be("Test User");
result.Value.Author.Email.Should().Be("testuser@example.com");

result.Value.Category.Should().NotBeNull();
result.Value.Category.CategoryName.Should().Be("Test Category");
}

/// <summary>
/// Creates a test article with all required foreign key relationships.
/// </summary>
private async Task<Guid> CreateValidArticleAsync(bool resetDatabase = true)
{
if (resetDatabase)
{
await _factory.ResetDatabaseAsync();
}

using var context = _contextFactory.CreateDbContext();

// Check if we already have test data, if not create it
if (string.IsNullOrEmpty(_testUserId) || _testCategoryId == Guid.Empty)
{
// Create test category
var testCategory = new Category("Test Category");
context.Categories.Add(testCategory);

// Create test user
_testUserId = Guid.NewGuid().ToString();
var testUser = new ApplicationUser
{
Id = _testUserId,
UserName = "testuser@example.com",
Email = "testuser@example.com",
DisplayName = "Test User",
EmailConfirmed = true
};
context.Users.Add(testUser);

await context.SaveChangesAsync();
_testCategoryId = testCategory.Id;
}

		// Create a published article
		var article = new Article(
			title: "Test Article Title",
			introduction: "Test Introduction",
			content: "Test Content for the article",
			coverImageUrl: "https://example.com/image.jpg",
			urlSlug: "test-article-title",
			authorId: _testUserId,
			categoryId: _testCategoryId,
			isPublished: true,
			publishedOn: DateTime.UtcNow
		);

		// EF will automatically load navigation properties via foreign keys when using Include()
		context.Articles.Add(article);
await context.SaveChangesAsync();

return article.Id;
}
}

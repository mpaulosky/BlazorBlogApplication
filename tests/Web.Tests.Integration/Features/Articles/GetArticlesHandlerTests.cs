// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetArticlesHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Articles.ArticlesList;

namespace Web.Features.Articles;

/// <summary>
/// Integration tests for the <see cref="GetArticles.Handler"/> class.
/// Tests the complete workflow of retrieving multiple articles from the database with various filtering scenarios.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetArticles.Handler))]
[Collection("Test Collection")]
public class GetArticlesHandlerTests
{
private readonly WebTestFactory _factory;
private readonly ILogger<GetArticles.Handler> _logger;
private readonly IApplicationDbContextFactory _contextFactory;
private string _testUserId = string.Empty;
private Guid _testCategoryId;

public GetArticlesHandlerTests(WebTestFactory factory)
{
_factory = factory;
using var scope = _factory.Services.CreateScope();
_logger = scope.ServiceProvider.GetRequiredService<ILogger<GetArticles.Handler>>();
_contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
}

[Fact]
public async Task HandleAsync_WithMultipleArticles_ReturnsAllArticles()
{
// Arrange
await CreateMultipleTestArticlesAsync();
var handler = new GetArticles.Handler(_contextFactory, _logger);

// Act
var result = await handler.HandleAsync(excludeArchived: false);

// Assert
result.Should().NotBeNull();
result.Success.Should().BeTrue("Retrieving articles should succeed when articles exist");
result.Error.Should().BeNull();
result.Value.Should().NotBeNull();
result.Value!.Should().HaveCount(3, "Should return all 3 created articles");

// Verify navigation properties are loaded correctly
var articles = result.Value!.ToList();
articles.Should().AllSatisfy(article =>
{
article.Author.Should().NotBeNull("Author navigation property should be loaded");
article.Author.Id.Should().Be(_testUserId);
article.Author.DisplayName.Should().Be("Test User");

article.Category.Should().NotBeNull("Category navigation property should be loaded");
article.Category.Id.Should().Be(_testCategoryId);
article.Category.CategoryName.Should().Be("Test Category");
});
}

[Fact]
public async Task HandleAsync_WithExcludeArchivedTrue_ReturnsOnlyNonArchivedArticles()
{
// Arrange
await CreateMultipleTestArticlesAsync(); // Creates 2 non-archived, 1 archived
var handler = new GetArticles.Handler(_contextFactory, _logger);

// Act
var result = await handler.HandleAsync(excludeArchived: true);

// Assert
result.Should().NotBeNull();
result.Success.Should().BeTrue("Retrieving non-archived articles should succeed");
result.Error.Should().BeNull();
result.Value.Should().NotBeNull();
result.Value!.Should().HaveCount(2, "Should return only the 2 non-archived articles");

// Verify all returned articles are not archived
var articles = result.Value!.ToList();
articles.Should().AllSatisfy(article =>
{
article.IsArchived.Should().BeFalse("All returned articles should be non-archived");
});

// Verify specific articles are included/excluded
articles.Should().Contain(a => a.Title == "Published Article");
articles.Should().Contain(a => a.Title == "Draft Article");
articles.Should().NotContain(a => a.Title == "Archived Article");
}

[Fact]
public async Task HandleAsync_WithExcludeArchivedFalse_ReturnsAllArticlesIncludingArchived()
{
// Arrange
await CreateMultipleTestArticlesAsync(); // Creates 2 non-archived, 1 archived
var handler = new GetArticles.Handler(_contextFactory, _logger);

// Act
var result = await handler.HandleAsync(excludeArchived: false);

// Assert
result.Should().NotBeNull();
result.Success.Should().BeTrue("Retrieving all articles should succeed");
result.Error.Should().BeNull();
result.Value.Should().NotBeNull();
result.Value!.Should().HaveCount(3, "Should return all 3 articles including archived");

// Verify mix of archived and non-archived articles
var articles = result.Value!.ToList();
articles.Should().Contain(a => a.IsArchived == true, "Should include archived articles");
articles.Should().Contain(a => a.IsArchived == false, "Should include non-archived articles");

// Verify all expected articles are included
articles.Should().Contain(a => a.Title == "Published Article");
articles.Should().Contain(a => a.Title == "Draft Article");
articles.Should().Contain(a => a.Title == "Archived Article");
}

[Fact]
public async Task HandleAsync_WithEmptyDatabase_ReturnsFailureResult()
{
// Arrange
await _factory.ResetDatabaseAsync(); // Ensure empty database
var handler = new GetArticles.Handler(_contextFactory, _logger);

// Act
var result = await handler.HandleAsync();

// Assert
result.Should().NotBeNull();
result.Success.Should().BeFalse("Should fail when no articles exist");
result.Error.Should().Be("No articles found.");
result.Value.Should().BeNull();
}

[Fact]
public async Task HandleAsync_WithOnlyArchivedArticlesAndExcludeTrue_ReturnsFailure()
{
// Arrange
await CreateOnlyArchivedArticlesAsync();
var handler = new GetArticles.Handler(_contextFactory, _logger);

// Act
var result = await handler.HandleAsync(excludeArchived: true);

// Assert
result.Should().NotBeNull();
result.Success.Should().BeFalse("Should fail when only archived articles exist and they're excluded");
result.Error.Should().Be("No articles found.");
result.Value.Should().BeNull();
}

[Fact]
public async Task HandleAsync_VerifiesNavigationPropertiesAreProperlyLoaded()
{
// Arrange
await CreateSingleTestArticleAsync();
var handler = new GetArticles.Handler(_contextFactory, _logger);

// Act
var result = await handler.HandleAsync();

// Assert
result.Should().NotBeNull();
result.Success.Should().BeTrue();

var article = result.Value!.First();

// Verify Author navigation property details
article.Author.Should().NotBeNull();
article.Author.Id.Should().Be(_testUserId);
article.Author.UserName.Should().Be("testuser@example.com");
article.Author.Email.Should().Be("testuser@example.com");
article.Author.DisplayName.Should().Be("Test User");
article.Author.EmailConfirmed.Should().BeTrue();

// Verify Category navigation property details
article.Category.Should().NotBeNull();
article.Category.Id.Should().Be(_testCategoryId);
article.Category.CategoryName.Should().Be("Test Category");
article.Category.IsArchived.Should().BeFalse();
}

[Fact]
public async Task HandleAsync_WithVariousArticleStatuses_ReturnsCorrectlyMappedDtos()
{
// Arrange
await CreateMultipleTestArticlesAsync();
var handler = new GetArticles.Handler(_contextFactory, _logger);

// Act
var result = await handler.HandleAsync(excludeArchived: false);

// Assert
result.Should().NotBeNull();
result.Success.Should().BeTrue();

var articles = result.Value!.ToList();

// Verify published article
var publishedArticle = articles.First(a => a.Title == "Published Article");
publishedArticle.IsPublished.Should().BeTrue();
publishedArticle.PublishedOn.Should().NotBeNull();
publishedArticle.IsArchived.Should().BeFalse();

// Verify draft article
var draftArticle = articles.First(a => a.Title == "Draft Article");
draftArticle.IsPublished.Should().BeFalse();
draftArticle.PublishedOn.Should().BeNull();
draftArticle.IsArchived.Should().BeFalse();

// Verify archived article
var archivedArticle = articles.First(a => a.Title == "Archived Article");
archivedArticle.IsArchived.Should().BeTrue();
archivedArticle.IsPublished.Should().BeFalse();
}

/// <summary>
/// Helper method to create multiple test articles with different statuses for comprehensive testing.
/// </summary>
private async Task CreateMultipleTestArticlesAsync(bool resetDatabase = true)
{
if (resetDatabase)
{
await _factory.ResetDatabaseAsync();
}

using (var context = _contextFactory.CreateDbContext())
{
// Create test user and category if not exist
await EnsureTestDataExistsAsync(context);

// Create a published article
var publishedArticle = new Article(
title: "Published Article",
introduction: "Published Introduction",
content: "Published Content",
coverImageUrl: "https://example.com/published.jpg",
urlSlug: "published-article",
authorId: _testUserId,
categoryId: _testCategoryId,
isPublished: true,
publishedOn: DateTime.UtcNow
);

// Create a draft article
var draftArticle = new Article(
title: "Draft Article",
introduction: "Draft Introduction",
content: "Draft Content",
coverImageUrl: "https://example.com/draft.jpg",
urlSlug: "draft-article",
authorId: _testUserId,
categoryId: _testCategoryId,
isPublished: false,
publishedOn: null
);

// Create an archived article
var archivedArticle = new Article(
title: "Archived Article",
introduction: "Archived Introduction",
content: "Archived Content",
coverImageUrl: "https://example.com/archived.jpg",
urlSlug: "archived-article",
authorId: _testUserId,
categoryId: _testCategoryId,
isPublished: false,
publishedOn: null
);
archivedArticle.IsArchived = true;

context.Articles.AddRange(publishedArticle, draftArticle, archivedArticle);
await context.SaveChangesAsync();
}
}

/// <summary>
/// Helper method to create only archived articles for edge case testing.
/// </summary>
private async Task CreateOnlyArchivedArticlesAsync()
{
await _factory.ResetDatabaseAsync();

using (var context = _contextFactory.CreateDbContext())
{
await EnsureTestDataExistsAsync(context);

var archivedArticle = new Article(
title: "Only Archived Article",
introduction: "Archived Introduction",
content: "Archived Content",
coverImageUrl: "https://example.com/archived.jpg",
urlSlug: "only-archived-article",
authorId: _testUserId,
categoryId: _testCategoryId,
isPublished: false,
publishedOn: null
);
archivedArticle.IsArchived = true;

context.Articles.Add(archivedArticle);
await context.SaveChangesAsync();
}
}

/// <summary>
/// Helper method to create a single test article for basic testing.
/// </summary>
private async Task CreateSingleTestArticleAsync()
{
await _factory.ResetDatabaseAsync();

using (var context = _contextFactory.CreateDbContext())
{
await EnsureTestDataExistsAsync(context);

var article = new Article(
title: "Single Test Article",
introduction: "Test Introduction",
content: "Test Content",
coverImageUrl: "https://example.com/test.jpg",
urlSlug: "single-test-article",
authorId: _testUserId,
categoryId: _testCategoryId,
isPublished: true,
publishedOn: DateTime.UtcNow
);

context.Articles.Add(article);
await context.SaveChangesAsync();
}
}

/// <summary>
/// Helper method to ensure test user and category exist in the database.
/// </summary>
private async Task EnsureTestDataExistsAsync(ApplicationDbContext context)
{
// Create test user if not exists
if (string.IsNullOrEmpty(_testUserId))
{
var testUser = new ApplicationUser
{
Id = Guid.CreateVersion7().ToString(),
UserName = "testuser@example.com",
Email = "testuser@example.com",
DisplayName = "Test User",
EmailConfirmed = true
};
context.Users.Add(testUser);
_testUserId = testUser.Id;
}

// Create test category if not exists
if (_testCategoryId == Guid.Empty)
{
var testCategory = new Category("Test Category");
context.Categories.Add(testCategory);
_testCategoryId = testCategory.Id;
}

await context.SaveChangesAsync();
}
}

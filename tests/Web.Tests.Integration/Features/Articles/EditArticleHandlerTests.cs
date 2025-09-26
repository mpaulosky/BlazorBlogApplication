// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditArticleHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Articles.ArticleEdit;

namespace Web.Features.Articles;

/// <summary>
/// Integration tests for the <see cref="EditArticle.Handler"/> class.
/// Tests the complete workflow of editing articles in the database with various scenarios.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(EditArticle.Handler))]
[Collection("Test Collection")]
public class EditArticleHandlerTests
{
	private readonly WebTestFactory _factory;
	private readonly ILogger<EditArticle.Handler> _logger;
	private readonly IApplicationDbContextFactory _contextFactory;
	private string _testUserId = string.Empty;
	private Guid _testCategoryId;

	public EditArticleHandlerTests(WebTestFactory factory)
	{
		_factory = factory;
		using var scope = _factory.Services.CreateScope();
		_logger = scope.ServiceProvider.GetRequiredService<ILogger<EditArticle.Handler>>();
		_contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
	}

	/// <summary>
	/// Seeds the database with test data and creates a valid article for testing updates.
	/// </summary>
	private async Task<Guid> CreateValidArticleAsync()
	{
		await _factory.ResetDatabaseAsync();
		
		using var context = _contextFactory.CreateDbContext();
		
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

		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
		_testCategoryId = testCategory.Id;

		// Create test article using parameterless constructor and setting properties
		var article = new Article();
		article.Title = "Original Title";
		article.Introduction = "Original introduction";
		article.Content = "Original content";
		article.CoverImageUrl = "https://example.com/original.jpg";
		article.UrlSlug = "original-title";
		article.AuthorId = _testUserId;
		article.CategoryId = _testCategoryId;
		article.IsPublished = false;
		article.PublishedOn = null;
		article.IsArchived = false;

		context.Articles.Add(article);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		return article.Id;
	}

	/// <summary>
	/// Creates a valid ArticleDto for testing article updates.
	/// </summary>
	private ArticleDto CreateValidArticleDto(Guid articleId)
	{
		return new ArticleDto
		{
			Id = articleId,
			Title = "Updated Article Title",
			Introduction = "Updated introduction",
			Content = "Updated content for the article",
			CoverImageUrl = "https://example.com/updated-cover.jpg",
			UrlSlug = "updated-article-title",
			Author = new ApplicationUserDto
			{
				Id = _testUserId,
				UserName = "testuser@example.com",
				Email = "testuser@example.com",
				DisplayName = "Test User",
				EmailConfirmed = true
			},
			Category = new CategoryDto
			{
				Id = _testCategoryId,
				CategoryName = "Test Category"
			},
			IsPublished = true,
			PublishedOn = DateTime.UtcNow.AddDays(-1),
			IsArchived = false
		};
	}

	[Fact]
	public async Task HandleAsync_WithValidArticleUpdate_ReturnsSuccess()
	{
		// Arrange
		var articleId = await CreateValidArticleAsync();
		var handler = new EditArticle.Handler(_contextFactory, _logger);

		var updatedDto = CreateValidArticleDto(articleId);

		// Act
		var result = await handler.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was updated in the database
		using var context = _contextFactory.CreateDbContext();
		var updatedArticle = await context.Articles.FirstOrDefaultAsync(a => a.Id == articleId, TestContext.Current.CancellationToken);
		
		updatedArticle.Should().NotBeNull();
		updatedArticle!.Title.Should().Be("Updated Article Title");
		updatedArticle.Introduction.Should().Be("Updated introduction");
		updatedArticle.Content.Should().Be("Updated content for the article");
		updatedArticle.ModifiedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public async Task HandleAsync_WithNullRequest_ReturnsFailure()
	{
		// Arrange
		var handler = new EditArticle.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(null);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("The request is null.");
	}

	[Fact]
	public async Task HandleAsync_WithNonExistentArticle_ReturnsFailure()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new EditArticle.Handler(_contextFactory, _logger);

		var nonExistentId = Guid.NewGuid();
		var articleDto = CreateValidArticleDto(nonExistentId);

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("Article not found.");
	}

	[Fact]
	public async Task HandleAsync_UpdatesModifiedOnTimestamp()
	{
		// Arrange
		var articleId = await CreateValidArticleAsync();
		var handler = new EditArticle.Handler(_contextFactory, _logger);

		// Get original article data
		using var contextBefore = _contextFactory.CreateDbContext();
		var originalArticle = await contextBefore.Articles.FirstOrDefaultAsync(a => a.Id == articleId, TestContext.Current.CancellationToken);
		var originalModifiedOn = originalArticle!.ModifiedOn;

		// Wait a bit to ensure timestamp difference
		await Task.Delay(10, TestContext.Current.CancellationToken);

		var updatedDto = CreateValidArticleDto(articleId);

		// Act
		var result = await handler.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify ModifiedOn timestamp was updated
		using var contextAfter = _contextFactory.CreateDbContext();
		var updatedArticle = await contextAfter.Articles.FirstOrDefaultAsync(a => a.Id == articleId, TestContext.Current.CancellationToken);
		
		updatedArticle.Should().NotBeNull();
		updatedArticle!.ModifiedOn.Should().BeAfter(originalModifiedOn.GetValueOrDefault());
		updatedArticle.ModifiedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public async Task HandleAsync_UpdatesAllArticleProperties()
	{
		// Arrange
		var articleId = await CreateValidArticleAsync();
		var handler = new EditArticle.Handler(_contextFactory, _logger);

		var updatedDto = CreateValidArticleDto(articleId);
		updatedDto.Title = "Completely New Title";
		updatedDto.Introduction = "Completely new introduction";
		updatedDto.Content = "Completely new content body";
		updatedDto.CoverImageUrl = "https://newsite.com/new-image.png";
		updatedDto.UrlSlug = "completely-new-title";
		updatedDto.IsPublished = false;
		updatedDto.PublishedOn = DateTime.UtcNow.AddDays(-10);
		updatedDto.IsArchived = true;

		// Act
		var result = await handler.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify all properties were updated
		using var context = _contextFactory.CreateDbContext();
		var updatedArticle = await context.Articles.FirstOrDefaultAsync(a => a.Id == articleId, TestContext.Current.CancellationToken);
		
		updatedArticle.Should().NotBeNull();
		updatedArticle!.Title.Should().Be("Completely New Title");
		updatedArticle.Introduction.Should().Be("Completely new introduction");
		updatedArticle.Content.Should().Be("Completely new content body");
		updatedArticle.CoverImageUrl.Should().Be("https://newsite.com/new-image.png");
		updatedArticle.UrlSlug.Should().Be("completely-new-title");
		updatedArticle.IsPublished.Should().BeFalse();
		updatedArticle.PublishedOn.Should().BeCloseTo(DateTime.UtcNow.AddDays(-10), TimeSpan.FromSeconds(5));
		updatedArticle.IsArchived.Should().BeTrue();
		updatedArticle.AuthorId.Should().Be(_testUserId);
		updatedArticle.CategoryId.Should().Be(_testCategoryId);
	}

	[Fact]
	public async Task HandleAsync_PreservesCreatedOnTimestamp()
	{
		// Arrange
		var articleId = await CreateValidArticleAsync();
		var handler = new EditArticle.Handler(_contextFactory, _logger);

		// Get original article data
		using var contextBefore = _contextFactory.CreateDbContext();
		var originalArticle = await contextBefore.Articles.FirstOrDefaultAsync(a => a.Id == articleId, TestContext.Current.CancellationToken);
		var originalCreatedOn = originalArticle!.CreatedOn;

		var updatedDto = CreateValidArticleDto(articleId);

		// Act
		var result = await handler.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify CreatedOn timestamp was preserved
		using var contextAfter = _contextFactory.CreateDbContext();
		var updatedArticle = await contextAfter.Articles.FirstOrDefaultAsync(a => a.Id == articleId, TestContext.Current.CancellationToken);
		
		updatedArticle.Should().NotBeNull();
		updatedArticle!.CreatedOn.Should().Be(originalCreatedOn);
	}

	[Fact]
	public async Task HandleAsync_WithEmptyTitle_AllowsUpdate()
	{
		// Arrange
		var articleId = await CreateValidArticleAsync();
		var handler = new EditArticle.Handler(_contextFactory, _logger);

		var updatedDto = CreateValidArticleDto(articleId);
		updatedDto.Title = string.Empty;

		// Act
		var result = await handler.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the empty title was saved
		using var context = _contextFactory.CreateDbContext();
		var updatedArticle = await context.Articles.FirstOrDefaultAsync(a => a.Id == articleId, TestContext.Current.CancellationToken);
		updatedArticle.Should().NotBeNull();
		updatedArticle!.Title.Should().Be(string.Empty);
	}

	[Fact]
	public async Task HandleAsync_LogsInformationOnSuccess()
	{
		// Arrange
		var articleId = await CreateValidArticleAsync();
		var handler = new EditArticle.Handler(_contextFactory, _logger);

		var updatedDto = CreateValidArticleDto(articleId);
		updatedDto.Title = "Logged Article Title";

		// Act
		var result = await handler.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		// Note: In a real scenario, you might want to use a test logger to verify log messages
		// For now, we're just ensuring the operation succeeds without throwing
	}

	[Fact]
	public async Task HandleAsync_WithDatabaseException_ReturnsFailure()
	{
		// Arrange
		var articleId = await CreateValidArticleAsync();
		var handler = new EditArticle.Handler(_contextFactory, _logger);

		// Create an invalid DTO that will cause a database constraint violation
		var invalidDto = CreateValidArticleDto(articleId);
		// Set non-existent foreign key references
		invalidDto.Author = new ApplicationUserDto
		{
			Id = Guid.NewGuid().ToString(),
			UserName = "nonexistent@example.com",
			Email = "nonexistent@example.com",
			DisplayName = "Non-existent User",
			EmailConfirmed = true
		};
		invalidDto.Category = new CategoryDto
		{
			Id = Guid.NewGuid(),
			CategoryName = "Non-existent Category"
		};

		// Act
		var result = await handler.HandleAsync(invalidDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
	}
}
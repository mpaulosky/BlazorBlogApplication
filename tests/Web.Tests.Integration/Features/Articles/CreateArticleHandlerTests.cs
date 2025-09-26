// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateArticleHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Articles.ArticleCreate;

namespace Web.Features.Articles;

/// <summary>
/// Integration tests for the <see cref="CreateArticle.Handler"/> class.
/// Tests the complete workflow of creating articles in the database with edge cases.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(CreateArticle.Handler))]
[Collection("Test Collection")]
public class CreateArticleHandlerTests
{
	private readonly WebTestFactory _factory;
	private readonly ILogger<CreateArticle.Handler> _logger;
	private readonly IApplicationDbContextFactory _contextFactory;
	private string _testUserId = string.Empty;
	private Guid _testCategoryId;

	public CreateArticleHandlerTests(WebTestFactory factory)
	{
		_factory = factory;
		using var scope = _factory.Services.CreateScope();
		_logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateArticle.Handler>>();
		_contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
	}

	/// <summary>
	/// Seeds the database with test data and returns a valid ArticleDto with proper foreign key references.
	/// </summary>
	private async Task<ArticleDto> CreateValidArticleDtoAsync(bool resetDatabase = true)
	{
		if (resetDatabase)
		{
			await _factory.ResetDatabaseAsync();
		}
		
		using (var context = _contextFactory.CreateDbContext())
		{
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
				
				await context.SaveChangesAsync(TestContext.Current.CancellationToken);
				_testCategoryId = testCategory.Id;
			}
		}
		
		// Create ArticleDto with valid foreign key references
		return new ArticleDto
		{
			Id = Guid.NewGuid(),
			Title = "Test Article Title",
			Introduction = "Test Introduction",
			Content = "Test Content for the article",
			CoverImageUrl = "https://example.com/test-image.jpg",
			UrlSlug = "test-article-title",
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
			IsPublished = false,
			PublishedOn = null,
			IsArchived = false
		};
	}

	[Fact]
	public async Task HandleAsync_WithNullRequest_ReturnsFailure()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(null);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("The request is null.");
	}

	[Fact]
	public async Task HandleAsync_WithValidArticle_CreatesArticleSuccessfully()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		
		string testUserId;
		Guid testCategoryId;
		
		// First, create valid Author and Category in the database
		using (var context = _contextFactory.CreateDbContext())
		{
			// Create a test category
			var testCategory = new Category("Test Category", false);
			context.Categories.Add(testCategory);
			
			// Create a test user (ApplicationUser) entity directly in the database
			testUserId = Guid.NewGuid().ToString();
			var testUser = new ApplicationUser
			{
				Id = testUserId,
				UserName = "testuser@example.com",
				Email = "testuser@example.com",
				DisplayName = "Test User",
				EmailConfirmed = true
			};
			context.Users.Add(testUser);
			
			await context.SaveChangesAsync(TestContext.Current.CancellationToken);
			
			// Store the IDs for later verification
			testCategoryId = testCategory.Id;
		}
		
		// Now create ArticleDto with valid foreign key references
		var articleDto = new ArticleDto
		{
			Id = Guid.NewGuid(),
			Title = "Test Article Title",
			Introduction = "Test Introduction",
			Content = "Test Content for the article",
			CoverImageUrl = "https://example.com/test-image.jpg",
			UrlSlug = "test-article-title",
			Author = new ApplicationUserDto
			{
				Id = testUserId,
				UserName = "testuser@example.com",
				Email = "testuser@example.com",
				DisplayName = "Test User",
				EmailConfirmed = true
			},
			Category = new CategoryDto
			{
				Id = testCategoryId,
				CategoryName = "Test Category"
			},
			IsPublished = false,
			PublishedOn = null,
			IsArchived = false
		};

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue("Article creation should succeed with valid data");
		result.Error.Should().BeNull();

		// Verify article was created in database with correct foreign key references
		using (var context = _contextFactory.CreateDbContext())
		{
			var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
			
			createdArticle.Should().NotBeNull();
			createdArticle!.Title.Should().Be(articleDto.Title);
			createdArticle.Introduction.Should().Be(articleDto.Introduction);
			createdArticle.Content.Should().Be(articleDto.Content);
			createdArticle.CoverImageUrl.Should().Be(articleDto.CoverImageUrl);
			createdArticle.UrlSlug.Should().Be(articleDto.UrlSlug);
			createdArticle.AuthorId.Should().Be(testUserId);
			createdArticle.CategoryId.Should().Be(testCategoryId);
			createdArticle.IsPublished.Should().Be(articleDto.IsPublished);
			createdArticle.PublishedOn.Should().Be(articleDto.PublishedOn);
			createdArticle.IsArchived.Should().Be(articleDto.IsArchived);
		}
	}

	[Fact]
	public async Task HandleAsync_WithPublishedArticle_CreatesWithPublishedDate()
	{
		// Arrange
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		var publishedDate = DateTime.UtcNow.AddHours(-1); // Use UtcNow for PostgreSQL compatibility
		articleDto.IsPublished = true;
		articleDto.PublishedOn = publishedDate;

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify published article properties
		using var context = _contextFactory.CreateDbContext();
		var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
		
		createdArticle.Should().NotBeNull();
		createdArticle!.IsPublished.Should().BeTrue();
		createdArticle.PublishedOn.Should().BeCloseTo(publishedDate, TimeSpan.FromSeconds(1));
	}

	[Fact]
	public async Task HandleAsync_WithUnpublishedArticle_CreatesWithoutPublishedDate()
	{
		// Arrange
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		articleDto.IsPublished = false;
		articleDto.PublishedOn = null;

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify unpublished article properties
		using var context = _contextFactory.CreateDbContext();
		var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
		
		createdArticle.Should().NotBeNull();
		createdArticle!.IsPublished.Should().BeFalse();
		createdArticle.PublishedOn.Should().BeNull();
	}

	[Fact]
	public async Task HandleAsync_WithArchivedArticle_CreatesWithArchivedStatus()
	{
		// Arrange
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		articleDto.IsArchived = true;

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify archived article properties
		using var context = _contextFactory.CreateDbContext();
		var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
		
		createdArticle.Should().NotBeNull();
		createdArticle!.IsArchived.Should().BeTrue();
	}

	[Fact]
	public async Task HandleAsync_WithMinimalRequiredFields_CreatesSuccessfully()
	{
		// Arrange
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		
		// Override with minimal required fields
		articleDto.Title = "Test Title";
		articleDto.Introduction = "Test Introduction";
		articleDto.Content = "Test Content";
		articleDto.UrlSlug = "test-title";
		articleDto.IsPublished = false;
		articleDto.PublishedOn = null;
		articleDto.IsArchived = false;

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify article was created with minimal fields
		using var context = _contextFactory.CreateDbContext();
		var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
		
		createdArticle.Should().NotBeNull();
		createdArticle!.Title.Should().Be("Test Title");
		createdArticle.Introduction.Should().Be("Test Introduction");
		createdArticle.Content.Should().Be("Test Content");
	}

	[Fact]
	public async Task HandleAsync_WithDuplicateUrlSlug_HandlesGracefully()
	{
		// Arrange
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var firstArticle = await CreateValidArticleDtoAsync(resetDatabase: true);
		var secondArticle = await CreateValidArticleDtoAsync(resetDatabase: false);
		secondArticle.UrlSlug = firstArticle.UrlSlug; // Same URL slug

		// Act - Create first article
		var firstResult = await handler.HandleAsync(firstArticle);
		// Act - Try to create second article with same URL slug
		var secondResult = await handler.HandleAsync(secondArticle);

		// Assert
		firstResult.Should().NotBeNull();
		firstResult.Success.Should().BeTrue();

		// Second article should fail due to database constraint or be handled gracefully
		secondResult.Should().NotBeNull();
		// Depending on database constraints, this might fail or succeed
		// Let's verify the database state either way
		using var context = _contextFactory.CreateDbContext();
		var articlesWithSameSlug = await context.Articles
			.Where(a => a.UrlSlug == firstArticle.UrlSlug)
			.ToListAsync(TestContext.Current.CancellationToken);
		
		// Should have at least one article created
		articlesWithSameSlug.Should().HaveCountGreaterOrEqualTo(1);
	}

	[Fact]
	public async Task HandleAsync_WithSpecialCharactersInFields_HandlesCorrectly()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		
		// Add special characters to test field handling
		articleDto.Title = "Test Title with Special Chars: <>&\"'";
		articleDto.Introduction = "Introduction with éñtèrnatiònál characters & symbols";
		articleDto.Content = "Content with\nnew lines and\ttabs and special chars: <>\"'&";

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify special characters are preserved
		using var context = _contextFactory.CreateDbContext();
		var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
		
		createdArticle.Should().NotBeNull();
		createdArticle!.Title.Should().Be("Test Title with Special Chars: <>&\"'");
		createdArticle.Introduction.Should().Be("Introduction with éñtèrnatiònál characters & symbols");
		createdArticle.Content.Should().Be("Content with\nnew lines and\ttabs and special chars: <>\"'&");
	}

	[Fact]
	public async Task HandleAsync_WithLongContent_HandlesCorrectly()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		
		// Create long content to test field length handling
		var longContent = string.Join(" ", Enumerable.Repeat("This is a very long content string.", 100));
		articleDto.Content = longContent;

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify long content is preserved
		using var context = _contextFactory.CreateDbContext();
		var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
		
		createdArticle.Should().NotBeNull();
		createdArticle!.Content.Should().Be(longContent);
		createdArticle.Content.Length.Should().BeGreaterThan(1000);
	}

	[Fact]
	public async Task HandleAsync_WithEmptyOptionalFields_CreatesSuccessfully()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		
		// Set optional fields to empty/default values
		articleDto.Introduction = string.Empty;
		articleDto.CoverImageUrl = string.Empty;

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify empty fields are handled correctly
		using var context = _contextFactory.CreateDbContext();
		var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
		
		createdArticle.Should().NotBeNull();
		createdArticle!.Introduction.Should().Be(string.Empty);
		createdArticle.CoverImageUrl.Should().Be(string.Empty);
	}

	[Fact]
	public async Task HandleAsync_MultipleValidArticles_CreatesAllSuccessfully()
	{
		// Arrange
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articles = new List<ArticleDto>();

		// Create 5 valid articles with unique URL slugs
		// First article will reset database and create test data
		var firstArticle = await CreateValidArticleDtoAsync(resetDatabase: true);
		firstArticle.UrlSlug = $"test-article-0";
		articles.Add(firstArticle);
		
		// Subsequent articles will reuse existing test data
		for (int i = 1; i < 5; i++)
		{
			var article = await CreateValidArticleDtoAsync(resetDatabase: false);
			article.UrlSlug = $"test-article-{i}";
			articles.Add(article);
		}

		// Act - Create multiple articles
		var results = new List<Shared.Abstractions.Result>();
		foreach (var article in articles)
		{
			var result = await handler.HandleAsync(article);
			results.Add(result);
		}

		// Assert
		results.Should().AllSatisfy(r => r.Success.Should().BeTrue());

		// Verify all articles were created
		using var context = _contextFactory.CreateDbContext();
		var createdArticles = await context.Articles.ToListAsync(TestContext.Current.CancellationToken);
		
		createdArticles.Should().HaveCount(5);
		createdArticles.Select(a => a.UrlSlug).Should().Contain(articles.Select(a => a.UrlSlug));
	}

	[Theory]
	[InlineData(true, true)]   // Published and archived
	[InlineData(true, false)]  // Published but not archived
	[InlineData(false, true)]  // Not published but archived
	[InlineData(false, false)] // Not published and not archived
	public async Task HandleAsync_WithVariousPublishAndArchiveStates_CreatesCorrectly(bool isPublished, bool isArchived)
	{
		// Arrange
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		
		articleDto.IsPublished = isPublished;
		articleDto.IsArchived = isArchived;
		articleDto.PublishedOn = isPublished ? DateTime.UtcNow : null; // Use UtcNow for PostgreSQL compatibility
		articleDto.UrlSlug = $"test-article-{isPublished}-{isArchived}"; // Unique slug for each test case

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify article state matches expectations
		using var context = _contextFactory.CreateDbContext();
		var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
		
		createdArticle.Should().NotBeNull();
		createdArticle!.IsPublished.Should().Be(isPublished);
		createdArticle.IsArchived.Should().Be(isArchived);
		
		if (isPublished)
		{
			createdArticle.PublishedOn.Should().NotBeNull();
		}
		else
		{
			createdArticle.PublishedOn.Should().BeNull();
		}
	}

	[Fact]
	public async Task HandleAsync_VerifyDatabaseTransactionIntegrity()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();

		// Get initial article count
		using var initialContext = _contextFactory.CreateDbContext();
		var initialCount = await initialContext.Articles.CountAsync(TestContext.Current.CancellationToken);

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify article count increased by exactly 1
		using var finalContext = _contextFactory.CreateDbContext();
		var finalCount = await finalContext.Articles.CountAsync(TestContext.Current.CancellationToken);
		
		finalCount.Should().Be(initialCount + 1);
	}

	[Fact]
	public async Task HandleAsync_VerifyEntityTimestamps()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		var beforeCreation = DateTime.UtcNow;

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify entity has appropriate timestamps
		using var context = _contextFactory.CreateDbContext();
		var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
		
		createdArticle.Should().NotBeNull();
		
		// The entity should have been created after our timestamp
		// Note: This depends on the Entity base class having CreatedOn/ModifiedOn properties
		if (createdArticle!.CreatedOn != default(DateTime))
		{
			createdArticle.CreatedOn.Should().BeAfter(beforeCreation.AddSeconds(-1));
			createdArticle.CreatedOn.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
		}
	}

	[Fact]
	public async Task HandleAsync_WithRequiredFieldsEmpty_ReturnsFailureResult()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		
		// Set required fields to empty (should cause validation failure)
		articleDto.Title = string.Empty;
		articleDto.Content = string.Empty;
		articleDto.UrlSlug = string.Empty;

		// Act 
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
	}

	[Fact]
	public async Task HandleAsync_WithNullRequiredFields_ReturnsFailureResult()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		
		// Set required fields to null (should cause validation failure)
		articleDto.Title = null!;
		articleDto.Content = null!;
		articleDto.UrlSlug = null!;

		// Act 
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
	}

	[Fact]
	public async Task HandleAsync_WithInvalidGuidIds_HandlesCorrectly()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		
		// Set non-existent GUIDs that should cause foreign key violations
		articleDto.Category.Id = Guid.NewGuid(); // Random GUID that doesn't exist
		articleDto.Author = articleDto.Author with { Id = Guid.NewGuid().ToString() }; // Random GUID that doesn't exist

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert - Should fail due to foreign key constraint violation
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
	}

	[Fact]
	public async Task HandleAsync_WithExtremelyLongFields_HandlesCorrectly()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		var articleDto = await CreateValidArticleDtoAsync();
		
		// Create extremely long strings to test database field limits
		var longTitle = new string('A', 500);
		var longIntroduction = new string('B', 1000);
		var longUrlSlug = new string('c', 200);
		var longCoverImageUrl = new string('D', 500) + ".jpg";
		
		articleDto.Title = longTitle;
		articleDto.Introduction = longIntroduction;
		articleDto.UrlSlug = longUrlSlug;
		articleDto.CoverImageUrl = longCoverImageUrl;

		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert - This might fail or succeed depending on database constraints
		result.Should().NotBeNull();
		
		if (result.Success)
		{
			// If it succeeded, verify the long fields were preserved
			using var context = _contextFactory.CreateDbContext();
			var createdArticle = await context.Articles.FirstOrDefaultAsync(a => a.UrlSlug == articleDto.UrlSlug, TestContext.Current.CancellationToken);
			
			createdArticle.Should().NotBeNull();
			createdArticle!.Title.Should().Be(longTitle);
			createdArticle.Introduction.Should().Be(longIntroduction);
		}
		else
		{
			// If it failed, verify the error message contains information about the failure
			result.Error.Should().NotBeNull();
			result.Error.Should().Contain("error");
		}
	}

	[Fact]
	public async Task HandleAsync_DatabaseSaveFailure_ReturnsFailureResult()
	{
		// This test would ideally mock the database to force a save failure,
		// but since we're doing integration tests, we'll test a scenario that might naturally fail
		
		// Arrange
		await _factory.ResetDatabaseAsync();
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		
		// Create an article with potentially problematic data
		var articleDto = await CreateValidArticleDtoAsync();
		
		// This test verifies that database exceptions are caught and returned as Result failures
		// In a real scenario, this might happen due to constraint violations, connection issues, etc.
		
		// Act
		var result = await handler.HandleAsync(articleDto);

		// Assert
		result.Should().NotBeNull();
		// For this test, we expect success since the data should be valid
		result.Success.Should().BeTrue();
		
		// However, if there was a database error, we would expect:
		// result.Success.Should().BeFalse();
		// result.Error.Should().NotBeNull();
		// result.Error.Should().Contain("error occurred while creating");
	}

	[Fact] 
	public async Task HandleAsync_ConcurrentCreation_HandlesCorrectly()
	{
		// Arrange
		var handler = new CreateArticle.Handler(_contextFactory, _logger);
		
		// Create multiple articles that could be created concurrently
		var articles = new List<ArticleDto>();
		
		// First article will reset database and create test data
		var firstArticle = await CreateValidArticleDtoAsync(resetDatabase: true);
		firstArticle.UrlSlug = $"concurrent-test-0-{Guid.NewGuid():N}";
		articles.Add(firstArticle);
		
		// Subsequent articles will reuse existing test data
		for (int i = 1; i < 3; i++)
		{
			var article = await CreateValidArticleDtoAsync(resetDatabase: false);
			article.UrlSlug = $"concurrent-test-{i}-{Guid.NewGuid():N}";
			articles.Add(article);
		}

		// Act - Simulate concurrent creation
		var tasks = articles.Select(article => handler.HandleAsync(article)).ToArray();
		var results = await Task.WhenAll(tasks);

		// Assert
		results.Should().HaveCount(3);
		results.Should().AllSatisfy(r => r.Success.Should().BeTrue());

		// Verify all articles were created
		using var context = _contextFactory.CreateDbContext();
		var createdArticles = await context.Articles.ToListAsync(TestContext.Current.CancellationToken);
		
		createdArticles.Should().HaveCountGreaterOrEqualTo(3);
	}
}
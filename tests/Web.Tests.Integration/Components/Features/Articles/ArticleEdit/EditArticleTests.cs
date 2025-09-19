// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditArticleTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================

namespace Web.Components.Features.Articles.ArticleEdit;

[ExcludeFromCodeCoverage]
[Collection("Test Collection")]
public class EditArticleTests : IAsyncLifetime
{
	private const string CLEANUP_VALUE = "Articles";

	private readonly WebTestFactory _factory;

	private readonly IServiceScope _scope;

	private readonly EditArticle.IEditArticleHandler _sut;

	private readonly CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;

	public EditArticleTests(WebTestFactory factory)
	{
		_factory = factory;

		// Create a scope here so scoped services (like IArticleDbContextFactory) are resolved correctly.
		_scope = _factory.Services.CreateScope();
		_sut = _scope.ServiceProvider.GetRequiredService<EditArticle.IEditArticleHandler>();
	}

	public ValueTask InitializeAsync()
	{
		return ValueTask.CompletedTask;
	}

	public async ValueTask DisposeAsync()
	{
		try
		{
			await _factory.ResetCollectionAsync(CLEANUP_VALUE);
		}
		finally
		{
			_scope.Dispose();
		}
	}

	[Fact(DisplayName = "HandleAsync Should Update Article Successfully With Valid Data")]
	public async Task HandleAsync_Should_UpdateArticleSuccessfully_With_ValidData_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Test Category", IsArchived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "testuser", Email = "test@example.com" };

		var originalArticle = new Article(
			"Original Title",
			"Original introduction",
			"Original content",
			"https://example.com/original.jpg",
			"original_article",
			author,
			CategoryDto.FromEntity(category),
			false,
			null,
			false
		);

		await ctx.Articles.InsertOneAsync(originalArticle, new MongoDB.Driver.InsertOneOptions(), _cancellationToken);

		// Create updated DTO
		var updatedDto = new ArticleDto
		{
			Id = originalArticle.Id,
			Title = "Updated Title",
			Introduction = "Updated introduction",
			Content = "Updated content",
			CoverImageUrl = "https://example.com/updated.jpg",
			UrlSlug = "updated_article",
			Author = author,
			Category = CategoryDto.FromEntity(category),
			CreatedOn = originalArticle.CreatedOn,
			IsPublished = true,
			PublishedOn = DateTime.UtcNow,
			IsArchived = false
		};

		// Act
		var result = await _sut.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was updated in the database
		var updatedArticle = await ctx.Articles.Find(a => a.Id == originalArticle.Id).FirstOrDefaultAsync(_cancellationToken);
		updatedArticle.Should().NotBeNull();
		updatedArticle.Title.Should().Be("Updated Title");
		updatedArticle.Introduction.Should().Be("Updated introduction");
		updatedArticle.Content.Should().Be("Updated content");
		updatedArticle.CoverImageUrl.Should().Be("https://example.com/updated.jpg");
		updatedArticle.UrlSlug.Should().Be("updated_article");
		updatedArticle.IsPublished.Should().BeTrue();
		updatedArticle.PublishedOn.Should().NotBeNull();
		updatedArticle.IsArchived.Should().BeFalse();
		updatedArticle.ModifiedOn.Should().NotBeNull();
	}

	[Fact(DisplayName = "HandleAsync Should Return Failure When Request Is Null")]
	public async Task HandleAsync_Should_ReturnFailure_When_RequestIsNull_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();

		// Act
		var result = await _sut.HandleAsync(null);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("The request is null.");
	}

	[Fact(DisplayName = "HandleAsync Should Return Failure When Article Does Not Exist")]
	public async Task HandleAsync_Should_ReturnFailure_When_ArticleDoesNotExist_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Test Category", IsArchived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "testuser", Email = "test@example.com" };

		var nonExistentDto = new ArticleDto
		{
			Id = ObjectId.GenerateNewId(),
			Title = "Non-existent Article",
			Introduction = "This article doesn't exist",
			Content = "Content",
			CoverImageUrl = "https://example.com/image.jpg",
			UrlSlug = "non_existent",
			Author = author,
			Category = CategoryDto.FromEntity(category),
			CreatedOn = DateTime.UtcNow,
			IsPublished = true,
			PublishedOn = DateTime.UtcNow,
			IsArchived = false
		};

		// Act
		var result = await _sut.HandleAsync(nonExistentDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("Article not found.");
	}

	[Fact(DisplayName = "HandleAsync Should Update Article To Draft Status")]
	public async Task HandleAsync_Should_UpdateArticleToDraftStatus_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Test Category", IsArchived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "testuser", Email = "test@example.com" };

		var publishedArticle = new Article(
			"Published Article",
			"Published introduction",
			"Published content",
			"https://example.com/published.jpg",
			"published_article",
			author,
			CategoryDto.FromEntity(category),
			true,
			DateTime.UtcNow,
			false
		);

		await ctx.Articles.InsertOneAsync(publishedArticle, cancellationToken: CancellationToken.None);

		// Create updated DTO to unpublish
		var updatedDto = new ArticleDto
		{
			Id = publishedArticle.Id,
			Title = "Now Draft Article",
			Introduction = "Now a draft",
			Content = "Draft content",
			CoverImageUrl = "https://example.com/draft.jpg",
			UrlSlug = "draft_article",
			Author = author,
			Category = CategoryDto.FromEntity(category),
			CreatedOn = publishedArticle.CreatedOn,
			IsPublished = false,
			PublishedOn = null,
			IsArchived = false
		};

		// Act
		var result = await _sut.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was updated
		var updatedArticle = await ctx.Articles.Find(a => a.Id == publishedArticle.Id).FirstOrDefaultAsync(_cancellationToken);
		updatedArticle.Should().NotBeNull();
		updatedArticle.IsPublished.Should().BeFalse();
		updatedArticle.PublishedOn.Should().BeNull();
	}

	[Fact(DisplayName = "HandleAsync Should Update Article To Archived Status")]
	public async Task HandleAsync_Should_UpdateArticleToArchivedStatus_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Test Category", IsArchived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "testuser", Email = "test@example.com" };

		var activeArticle = new Article(
			"Active Article",
			"Active introduction",
			"Active content",
			"https://example.com/active.jpg",
			"active_article",
			author,
			CategoryDto.FromEntity(category),
			true,
			DateTime.UtcNow,
			false
		);

		await ctx.Articles.InsertOneAsync(activeArticle, cancellationToken: CancellationToken.None);

		// Create updated DTO to archive
		var updatedDto = new ArticleDto
		{
			Id = activeArticle.Id,
			Title = "Archived Article",
			Introduction = "Archived introduction",
			Content = "Archived content",
			CoverImageUrl = "https://example.com/archived.jpg",
			UrlSlug = "archived_article",
			Author = author,
			Category = CategoryDto.FromEntity(category),
			CreatedOn = activeArticle.CreatedOn,
			IsPublished = true,
			PublishedOn = DateTime.UtcNow,
			IsArchived = true
		};

		// Act
		var result = await _sut.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was updated
		var updatedArticle = await ctx.Articles.Find(a => a.Id == activeArticle.Id).FirstOrDefaultAsync(_cancellationToken);
		updatedArticle.Should().NotBeNull();
		updatedArticle.IsArchived.Should().BeTrue();
	}

	[Fact(DisplayName = "HandleAsync Should Handle Unicode Characters In Updated Data")]
	public async Task HandleAsync_Should_HandleUnicodeCharacters_In_UpdatedData_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "测试分类", IsArchived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "测试用户", Email = "test@example.com" };

		var originalArticle = new Article(
			"Original Title",
			"Original intro",
			"Original content",
			"https://example.com/original.jpg",
			"original_article",
			author,
			CategoryDto.FromEntity(category),
			false,
			null,
			false
		);

		await ctx.Articles.InsertOneAsync(originalArticle, cancellationToken: CancellationToken.None);

		// Create updated DTO with Unicode
		var updatedDto = new ArticleDto
		{
			Id = originalArticle.Id,
			Title = "Unicode 标题",
			Introduction = "Unicode 简介",
			Content = "Unicode 内容：αβγδε 中文 español français",
			CoverImageUrl = "https://example.com/unicode.jpg",
			UrlSlug = "unicode_article",
			Author = author,
			Category = CategoryDto.FromEntity(category),
			CreatedOn = originalArticle.CreatedOn,
			IsPublished = true,
			PublishedOn = DateTime.UtcNow,
			IsArchived = false
		};

		// Act
		var result = await _sut.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was updated
		var updatedArticle = await ctx.Articles.Find(a => a.Id == originalArticle.Id).FirstOrDefaultAsync(_cancellationToken);
		updatedArticle.Should().NotBeNull();
		updatedArticle.Title.Should().Be("Unicode 标题");
		updatedArticle.Introduction.Should().Be("Unicode 简介");
		updatedArticle.Content.Should().Be("Unicode 内容：αβγδε 中文 español français");
	}

	[Fact(DisplayName = "HandleAsync Should Handle Special Characters In Updated Data")]
	public async Task HandleAsync_Should_HandleSpecialCharacters_In_UpdatedData_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Special & Category", IsArchived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "special@user", Email = "special@example.com" };

		var originalArticle = new Article(
			"Original Title",
			"Original intro",
			"Original content",
			"https://example.com/original.jpg",
			"original_article",
			author,
			CategoryDto.FromEntity(category),
			false,
			null,
			false
		);

		await ctx.Articles.InsertOneAsync(originalArticle, cancellationToken: CancellationToken.None);

		// Create updated DTO with special characters
		var updatedDto = new ArticleDto
		{
			Id = originalArticle.Id,
			Title = "Special Characters: @#$%^&*()",
			Introduction = "Intro with <tags> & symbols",
			Content = "Content with émojis 😀 and symbols: ©®™",
			CoverImageUrl = "https://example.com/special.jpg",
			UrlSlug = "special_characters_article",
			Author = author,
			Category = CategoryDto.FromEntity(category),
			CreatedOn = originalArticle.CreatedOn,
			IsPublished = true,
			PublishedOn = DateTime.UtcNow,
			IsArchived = false
		};

		// Act
		var result = await _sut.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was updated
		var updatedArticle = await ctx.Articles.Find(a => a.Id == originalArticle.Id).FirstOrDefaultAsync(_cancellationToken);
		updatedArticle.Should().NotBeNull();
		updatedArticle.Title.Should().Be("Special Characters: @#$%^&*()");
		updatedArticle.Introduction.Should().Be("Intro with <tags> & symbols");
		updatedArticle.Content.Should().Be("Content with émojis 😀 and symbols: ©®™");
	}

	[Fact(DisplayName = "HandleAsync Should Update ModifiedOn Timestamp")]
	public async Task HandleAsync_Should_UpdateModifiedOnTimestamp_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Test Category", IsArchived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "testuser", Email = "test@example.com" };

		var originalArticle = new Article(
			"Original Title",
			"Original introduction",
			"Original content",
			"https://example.com/original.jpg",
			"original_article",
			author,
			CategoryDto.FromEntity(category),
			false,
			null,
			false
		);

		await ctx.Articles.InsertOneAsync(originalArticle, cancellationToken: CancellationToken.None);

		var beforeUpdate = DateTime.UtcNow;

		// Create updated DTO
		var updatedDto = new ArticleDto
		{
			Id = originalArticle.Id,
			Title = "Updated Title",
			Introduction = "Updated introduction",
			Content = "Updated content",
			CoverImageUrl = "https://example.com/updated.jpg",
			UrlSlug = "updated_article",
			Author = author,
			Category = CategoryDto.FromEntity(category),
			CreatedOn = originalArticle.CreatedOn,
			IsPublished = true,
			PublishedOn = DateTime.UtcNow,
			IsArchived = false
		};

		// Act
		var result = await _sut.HandleAsync(updatedDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify ModifiedOn was updated
		var updatedArticle = await ctx.Articles.Find(a => a.Id == originalArticle.Id).FirstOrDefaultAsync(_cancellationToken);
		updatedArticle.Should().NotBeNull();
		updatedArticle.ModifiedOn.Should().NotBeNull();
		updatedArticle.ModifiedOn.Should().BeAfter(beforeUpdate);
	}
}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetArticleTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================

namespace Web.Components.Features.Articles.ArticleDetails;

[ExcludeFromCodeCoverage]
[Collection("Test Collection")]
public class GetArticleTests : IAsyncLifetime
{
	private const string CLEANUP_VALUE = "Articles";

	private readonly WebTestFactory _factory;

	private readonly IServiceScope _scope;

	private readonly GetArticle.IGetArticleHandler _sut;

	public GetArticleTests(WebTestFactory factory)
	{
		_factory = factory;

		// Create a scope here so scoped services (like IMyBlogContextFactory) are resolved correctly.
		_scope = _factory.Services.CreateScope();
		_sut = _scope.ServiceProvider.GetRequiredService<GetArticle.IGetArticleHandler>();
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

	[Fact(DisplayName = "HandleAsync Should Return Article When Article Exists")]
	public async Task HandleAsync_Should_ReturnArticle_When_ArticleExists_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Test Category", Archived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "testuser", Email = "test@example.com" };

		var article = new Article(
			"Test Article",
			"Test introduction",
			"Test content for the article",
			"https://example.com/image.jpg",
			"test_article",
			author,
			CategoryDto.FromEntity(category),
			true,
			DateTime.UtcNow,
			false
		);

		await ctx.Articles.InsertOneAsync(article, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(article.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Id.Should().Be(article.Id);
		result.Value.Title.Should().Be("Test Article");
		result.Value.Introduction.Should().Be("Test introduction");
		result.Value.Content.Should().Be("Test content for the article");
		result.Value.CoverImageUrl.Should().Be("https://example.com/image.jpg");
		result.Value.UrlSlug.Should().Be("test_article");
		result.Value.Author.Id.Should().Be(author.Id);
		result.Value.Category.Id.Should().Be(category.Id);
		result.Value.IsPublished.Should().BeTrue();
		result.Value.IsArchived.Should().BeFalse();
	}

	[Fact(DisplayName = "HandleAsync Should Return Failure When Article Does Not Exist")]
	public async Task HandleAsync_Should_ReturnFailure_When_ArticleDoesNotExist_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();

		var nonExistentId = ObjectId.GenerateNewId();

		// Act
		var result = await _sut.HandleAsync(nonExistentId);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("Article not found.");
	}

	[Fact(DisplayName = "HandleAsync Should Return Failure When Id Is Empty")]
	public async Task HandleAsync_Should_ReturnFailure_When_IdIsEmpty_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();

		var emptyId = ObjectId.Empty;

		// Act
		var result = await _sut.HandleAsync(emptyId);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("The ID cannot be empty.");
	}

	[Fact(DisplayName = "HandleAsync Should Handle Unicode Characters In Article Data")]
	public async Task HandleAsync_Should_HandleUnicodeCharacters_In_ArticleData_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "测试分类", Archived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "测试用户", Email = "test@example.com" };

		var article = new Article(
			"Unicode 文章标题",
			"这是 Unicode 简介",
			"Unicode 内容：αβγδε 中文 español français",
			"https://example.com/unicode.jpg",
			"unicode_article",
			author,
			CategoryDto.FromEntity(category),
			true,
			DateTime.UtcNow,
			false
		);

		await ctx.Articles.InsertOneAsync(article, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(article.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Title.Should().Be("Unicode 文章标题");
		result.Value.Introduction.Should().Be("这是 Unicode 简介");
		result.Value.Content.Should().Be("Unicode 内容：αβγδε 中文 español français");
	}

	[Fact(DisplayName = "HandleAsync Should Handle Special Characters In Article Data")]
	public async Task HandleAsync_Should_HandleSpecialCharacters_In_ArticleData_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Special & Category", Archived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "special@user", Email = "special@example.com" };

		var article = new Article(
			"Special Characters: @#$%^&*()",
			"Intro with <tags> & symbols",
			"Content with émojis 😀 and symbols: ©®™",
			"https://example.com/special.jpg",
			"special_characters_article",
			author,
			CategoryDto.FromEntity(category),
			true,
			DateTime.UtcNow,
			false
		);

		await ctx.Articles.InsertOneAsync(article, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(article.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Title.Should().Be("Special Characters: @#$%^&*()");
		result.Value.Introduction.Should().Be("Intro with <tags> & symbols");
		result.Value.Content.Should().Be("Content with émojis 😀 and symbols: ©®™");
	}

	[Fact(DisplayName = "HandleAsync Should Return Draft Article Correctly")]
	public async Task HandleAsync_Should_ReturnDraftArticleCorrectly_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Draft Category", Archived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "draftauthor", Email = "draft@example.com" };

		var article = new Article(
			"Draft Article",
			"This is a draft article",
			"Draft content that is not published yet",
			"https://example.com/draft.jpg",
			"draft_article",
			author,
			CategoryDto.FromEntity(category),
			false,
			null,
			false
		);

		await ctx.Articles.InsertOneAsync(article, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(article.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.IsPublished.Should().BeFalse();
		result.Value.PublishedOn.Should().BeNull();
	}

	[Fact(DisplayName = "HandleAsync Should Return Archived Article Correctly")]
	public async Task HandleAsync_Should_ReturnArchivedArticleCorrectly_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var category = new Category { CategoryName = "Archived Category", Archived = false };
		await ctx.Categories.InsertOneAsync(category, cancellationToken: CancellationToken.None);

		var author = new AppUserDto { Id = ObjectId.GenerateNewId().ToString(), UserName = "archivedauthor", Email = "archived@example.com" };

		var article = new Article(
			"Archived Article",
			"This is an archived article",
			"Archived content",
			"https://example.com/archived.jpg",
			"archived_article",
			author,
			CategoryDto.FromEntity(category),
			true,
			DateTime.UtcNow,
			true
		);

		await ctx.Articles.InsertOneAsync(article, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(article.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.IsArchived.Should().BeTrue();
	}
}

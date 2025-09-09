// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateArticleTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================

namespace Web.Components.Features.Articles.ArticleCreate;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(CreateArticle.Handler))]
[Collection("Test Collection")]
public class CreateArticleTests : IAsyncLifetime
{
	private const string CLEANUP_VALUE = "Articles";

	private readonly WebTestFactory _factory;

	private readonly IServiceScope _scope;

	private readonly CreateArticle.ICreateArticleHandler _sut;

	private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

	public CreateArticleTests(WebTestFactory factory)
	{
		_factory = factory;

		// Create a scope here so scoped services (like IMyBlogContextFactory) are resolved correctly.
		_scope = _factory.Services.CreateScope();
		_sut = _scope.ServiceProvider.GetRequiredService<CreateArticle.ICreateArticleHandler>();
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

	[Fact(DisplayName = "HandleAsync Should Create Article Successfully With Valid Data")]
	public async Task HandleAsync_Should_CreateArticleSuccessfully_With_ValidData_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(_cancellationToken);

		// Create required dependencies
		var request = FakeArticleDto.GetNewArticleDto(true);

		// Act
		var result = await _sut.HandleAsync(request);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was created in the database
		var createdArticle = await ctx.Articles.Find(a => a.Title == request.Title).FirstOrDefaultAsync(_cancellationToken);
		createdArticle.Should().NotBeNull();
		createdArticle.Title.Should().Be(request.Title);
		createdArticle.Introduction.Should().Be(request.Introduction);
		createdArticle.Content.Should().Be(request.Content);
		createdArticle.CoverImageUrl.Should().Be(request.CoverImageUrl);
		createdArticle.UrlSlug.Should().Be(request.UrlSlug);
		createdArticle.IsPublished.Should().Be(request.IsPublished);
		createdArticle.IsArchived.Should().Be(request.IsArchived);
		createdArticle.PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromHours(7));
		createdArticle.Author.UserName.Should().Be(request.Author.UserName);
		createdArticle.Author.Email.Should().Be(request.Author.Email);
		createdArticle.Category.CategoryName.Should().Be(request.Category.CategoryName);
		createdArticle.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromHours(7));
		createdArticle.IsArchived.Should().Be(request.IsArchived);
		createdArticle.IsPublished.Should().Be(request.IsPublished);
		createdArticle.Content.Length.Should().BeGreaterThan(0);

	}

	[Fact(DisplayName = "HandleAsync Should Create Draft Article When IsPublished Is False")]
	public async Task HandleAsync_Should_CreateDraftArticle_When_IsPublishedIsFalse_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var request = FakeArticleDto.GetNewArticleDto(true);

		// Act
		var result = await _sut.HandleAsync(request);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was created as a draft
		var createdArticle = await ctx.Articles.Find(a => a.Title == request.Title).FirstOrDefaultAsync(_cancellationToken);
		createdArticle.Should().NotBeNull();
		createdArticle.Title.Should().Be(request.Title);
		createdArticle.Introduction.Should().Be(request.Introduction);
		createdArticle.Content.Should().Be(request.Content);
		createdArticle.CoverImageUrl.Should().Be(request.CoverImageUrl);
		createdArticle.UrlSlug.Should().Be(request.UrlSlug);
		createdArticle.IsPublished.Should().Be(request.IsPublished);
		createdArticle.IsArchived.Should().Be(request.IsArchived);
		createdArticle.PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromHours(7));
		createdArticle.Author.UserName.Should().Be(request.Author.UserName);
		createdArticle.Author.Email.Should().Be(request.Author.Email);
		createdArticle.Category.CategoryName.Should().Be(request.Category.CategoryName);
		createdArticle.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromHours(7));
		createdArticle.IsArchived.Should().Be(request.IsArchived);
		createdArticle.IsPublished.Should().Be(request.IsPublished);
		createdArticle.Content.Length.Should().BeGreaterThan(0);

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

	[Fact(DisplayName = "HandleAsync Should Handle Unicode Characters In Title And Content")]
	public async Task HandleAsync_Should_HandleUnicodeCharacters_In_TitleAndContent_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var request = FakeArticleDto.GetNewArticleDto(true);
		request.Title = "测试文章标题";
		request.Introduction = "这是一个测试简介";
		request.Content = "这是文章的内容，包含中文字符和特殊符号：@#$%^&*()";

		// Act
		var result = await _sut.HandleAsync(request);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was created with Unicode characters
		var createdArticle = await ctx.Articles.Find(a => a.Title == request.Title).FirstOrDefaultAsync(_cancellationToken);
		createdArticle.Should().NotBeNull();
		createdArticle.Title.Should().Be(request.Title);
		createdArticle.Introduction.Should().Be(request.Introduction);
		createdArticle.Content.Should().Be(request.Content);
		createdArticle.CoverImageUrl.Should().Be(request.CoverImageUrl);
		createdArticle.UrlSlug.Should().Be(request.UrlSlug);
		createdArticle.IsPublished.Should().Be(request.IsPublished);
		createdArticle.IsArchived.Should().Be(request.IsArchived);
		createdArticle.PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromHours(7));
		createdArticle.Author.UserName.Should().Be(request.Author.UserName);
		createdArticle.Author.Email.Should().Be(request.Author.Email);
		createdArticle.Category.CategoryName.Should().Be(request.Category.CategoryName);
		createdArticle.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromHours(7));
		createdArticle.IsArchived.Should().Be(request.IsArchived);
		createdArticle.IsPublished.Should().Be(request.IsPublished);
		createdArticle.Content.Length.Should().BeGreaterThan(0);

	}

	[Fact(DisplayName = "HandleAsync Should Handle Special Characters In Title And Content")]
	public async Task HandleAsync_Should_HandleSpecialCharacters_In_TitleAndContent_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var request = FakeArticleDto.GetNewArticleDto(true);
		request.Title = "Special Characters: @#$%^&*()";
		request.Introduction = "Intro with <tags> & symbols";
		request.Content = "Content with émojis 😀 and symbols: ©®™";

		// Act
		var result = await _sut.HandleAsync(request);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was created with special characters
		var createdArticle = await ctx.Articles.Find(a => a.Title == request.Title).FirstOrDefaultAsync(_cancellationToken);
		createdArticle.Should().NotBeNull();
		createdArticle.Title.Should().Be(request.Title);
		createdArticle.Introduction.Should().Be(request.Introduction);
		createdArticle.Content.Should().Be(request.Content);
		createdArticle.CoverImageUrl.Should().StartWith("https://");
		createdArticle.UrlSlug.Should().NotBeNullOrEmpty();
		createdArticle.IsPublished.Should().Be(request.IsPublished);
		createdArticle.IsArchived.Should().Be(request.IsArchived);
		createdArticle.PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromHours(7));
		createdArticle.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromHours(7));
		createdArticle.IsArchived.Should().Be(request.IsArchived);
		createdArticle.IsPublished.Should().Be(request.IsPublished);
		createdArticle.Content.Length.Should().BeGreaterThan(0);

	}

	[Fact(DisplayName = "HandleAsync Should Set CreatedOn Date Correctly")]
	public async Task HandleAsync_Should_SetCreatedOnDateCorrectly_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var request = FakeArticleDto.GetNewArticleDto(true);

		// Act
		var result = await _sut.HandleAsync(request);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the article was created with the correct CreatedOn date
		var createdArticle = await ctx.Articles.Find(a => a.Title == request.Title).FirstOrDefaultAsync(_cancellationToken);
		createdArticle.Should().NotBeNull();
		createdArticle.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromHours(7));

	}
}

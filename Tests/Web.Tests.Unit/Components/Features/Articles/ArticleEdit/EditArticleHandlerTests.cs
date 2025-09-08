// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditArticleHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Articles.ArticleEdit;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(EditArticle.Handler))]
public class EditArticleHandlerTests
{

	private readonly ArticlesTestFixture _fixture = new();

	[Fact]
	public async Task HandleAsync_WithValidArticle_ReplacesArticleAndReturnsOk()
	{
		// Arrange
		var existingArticle = new Article(
			"Original Title",
			"Original Introduction",
			"Original Content",
			"https://example.com/original.jpg",
			"original_article",
			FakeAppUserDto.GetNewAppUserDto(true),
			FakeCategoryDto.GetNewCategoryDto(true),
			false,
			null,
			false);

		// Use reflection to set Id and CreatedOn since they have protected init setters
		var idProperty = typeof(Entity).GetProperty("Id");
		idProperty?.SetValue(existingArticle, ObjectId.GenerateNewId());

		var createdOnProperty = typeof(Entity).GetProperty("CreatedOn");
		createdOnProperty?.SetValue(existingArticle, DateTime.UtcNow.AddDays(-1));

		_fixture.SetupFindAsync([existingArticle]);

		_fixture.ArticlesCollection.ReplaceOneAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<Article>(),
						Arg.Any<ReplaceOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult<ReplaceOneResult?>(null!));

		var handler = _fixture.CreateEditHandler();

		var dto = FakeArticleDto.GetNewArticleDto(true);
		dto.Id = existingArticle.Id; // Use the same ID as the existing article

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();

		// Verify ReplaceOneAsync was called and ModifiedOn was set on the replacement Article
		_ = _fixture.ArticlesCollection.Received(1).ReplaceOneAsync(
				Arg.Any<FilterDefinition<Article>>(),
				Arg.Is<Article>(a => a.Title == dto.Title && a.Introduction == dto.Introduction && a.ModifiedOn != null),
				Arg.Any<ReplaceOptions>(),
				Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_WhenReplaceThrows_ReturnsFailWithErrorMessage()
	{
		// Arrange
		var existingArticle = new Article(
			"Original Title",
			"Original Introduction",
			"Original Content",
			"https://example.com/original.jpg",
			"original_article",
			FakeAppUserDto.GetNewAppUserDto(true),
			FakeCategoryDto.GetNewCategoryDto(true),
			false,
			null,
			false);

		// Use reflection to set Id and CreatedOn since they have protected init setters
		var idProperty = typeof(Entity).GetProperty("Id");
		idProperty?.SetValue(existingArticle, ObjectId.GenerateNewId());

		var createdOnProperty = typeof(Entity).GetProperty("CreatedOn");
		createdOnProperty?.SetValue(existingArticle, DateTime.UtcNow.AddDays(-1));

		_fixture.SetupFindAsync([existingArticle]);

		_fixture.ArticlesCollection.When(c => c.ReplaceOneAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<Article>(),
						Arg.Any<ReplaceOptions>(), Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("DB error"));

		var handler = _fixture.CreateEditHandler();

		var dto = new ArticleDto { Title = "T" };
		dto.Id = existingArticle.Id; // Use the same ID as the existing article

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("DB error");

		// Verify logger received an Error-level log and an exception was passed
		_fixture.EditLogger.Received(1).Log(
				LogLevel.Error,
				Arg.Any<EventId>(),
				Arg.Is<object>(o => o != null && o.ToString()!.Contains("Failed to update")),
				Arg.Is<Exception>(e => e is InvalidOperationException && e.Message.Contains("DB error")),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_NullRequest_ReturnsFail()
	{
		// Arrange
		_fixture.ArticlesCollection.ReplaceOneAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<Article>(),
						Arg.Any<ReplaceOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult<ReplaceOneResult?>(null!));

		var handler = _fixture.CreateEditHandler();

		// Act
		var result = await handler.HandleAsync(null);

		// Assert - handler should return a failure result indicating a null request
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("The request is null");
	}

	[Fact]
	public async Task HandleAsync_LogsInformation_OnSuccess()
	{
		// Arrange
		var existingArticle = new Article(
			"Original Title",
			"Original Introduction",
			"Original Content",
			"https://example.com/original.jpg",
			"original_article",
			FakeAppUserDto.GetNewAppUserDto(true),
			FakeCategoryDto.GetNewCategoryDto(true),
			false,
			null,
			false);

		// Use reflection to set Id and CreatedOn since they have protected init setters
		var idProperty = typeof(Entity).GetProperty("Id");
		idProperty?.SetValue(existingArticle, ObjectId.GenerateNewId());

		var createdOnProperty = typeof(Entity).GetProperty("CreatedOn");
		createdOnProperty?.SetValue(existingArticle, DateTime.UtcNow.AddDays(-1));

		_fixture.SetupFindAsync([existingArticle]);

		_fixture.ArticlesCollection.ReplaceOneAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<Article>(),
						Arg.Any<ReplaceOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult<ReplaceOneResult?>(null!));

		var handler = _fixture.CreateEditHandler();

		var dto = FakeArticleDto.GetNewArticleDto(true);
		dto.Id = existingArticle.Id; // Use the same ID as the existing article

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();

		_fixture.EditLogger.Received(1).Log(
				LogLevel.Information,
				Arg.Any<EventId>(),
				Arg.Is<object>(o => o != null && o.ToString()!.Contains("Article updated successfully")),
				Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_SetsModifiedOn_ToRecentUtcAndPreservesPublishedOn()
	{
		// Arrange
		var existingArticle = new Article(
			"Original Title",
			"Original Introduction",
			"Original Content",
			"https://example.com/original.jpg",
			"original_article",
			FakeAppUserDto.GetNewAppUserDto(true),
			FakeCategoryDto.GetNewCategoryDto(true),
			false,
			null,
			false);

		// Use reflection to set Id and CreatedOn since they have protected init setters
		var idProperty = typeof(Entity).GetProperty("Id");
		idProperty?.SetValue(existingArticle, ObjectId.GenerateNewId());

		var createdOnProperty = typeof(Entity).GetProperty("CreatedOn");
		createdOnProperty?.SetValue(existingArticle, DateTime.UtcNow.AddDays(-1));

		_fixture.SetupFindAsync([existingArticle]);

		Article? captured = null;

		_fixture.ArticlesCollection.ReplaceOneAsync(Arg.Any<FilterDefinition<Article>>(),
						Arg.Do<Article>(a => captured = a), Arg.Any<ReplaceOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.FromResult<ReplaceOneResult?>(null!));

		var handler = _fixture.CreateEditHandler();

		var dto = FakeArticleDto.GetNewArticleDto(true);
		dto.Id = existingArticle.Id; // Use the same ID as the existing article
		var providedPublished = DateTime.UtcNow.AddDays(-2);
		dto.PublishedOn = providedPublished;

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();
		captured.Should().NotBeNull();
		captured!.PublishedOn.Should().Be(providedPublished);

		// ModifiedOn should be set to a recent UTC time
		captured.ModifiedOn.HasValue.Should().BeTrue();
		var delta = DateTime.UtcNow - captured.ModifiedOn!.Value;
		delta.TotalSeconds.Should().BeLessThan(10);
	}


}

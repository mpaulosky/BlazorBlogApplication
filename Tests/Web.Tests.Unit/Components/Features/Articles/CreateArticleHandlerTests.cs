// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateArticleHandlerTests.cs
// Company :       mpaulosky
// Author :        Copilot
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Components.Features.Articles.ArticleCreate;

namespace Web.Components.Features.Articles;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(CreateArticle.Handler))]
public class CreateArticleHandlerTests
{
	[Fact]
	public async Task HandleAsync_WithValidArticle_InsertsArticleAndReturnsOk()
	{
		// Arrange
		var collection = Substitute.For<IMongoCollection<Article>>();
		// Ensure InsertOneAsync completes successfully
		collection.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var handler = new CreateArticle.Handler(context, logger);

		var dto = FakeArticleDto.GetNewArticleDto(true);

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();
		// Verify an Article was inserted, and PublishedOn was set (not default)
		_ = collection.Received(1).InsertOneAsync(Arg.Is<Article>(a => a.Title == dto.Title && a.Introduction == dto.Introduction && a.PublishedOn != null), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_WhenInsertThrows_ReturnsFailWithErrorMessage()
	{
		// Arrange
		var collection = Substitute.For<IMongoCollection<Article>>();
		collection.When(c => c.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("DB error"));

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var handler = new CreateArticle.Handler(context, logger);

		var dto = new ArticleDto { Title = "T" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("DB error");
	}

	[Fact]
	public async Task HandleAsync_PublishedOnProvided_UsesProvidedPublishedOn()
	{
		// Arrange
		var collection = Substitute.For<IMongoCollection<Article>>();
		collection.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var handler = new CreateArticle.Handler(context, logger);

		var provided = DateTime.UtcNow.AddDays(-1);
		var dto = new ArticleDto { Title = "T", PublishedOn = provided };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();
		_ = collection.Received(1).InsertOneAsync(Arg.Is<Article>(a => a.PublishedOn == provided && a.Title == dto.Title), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_LogsInformation_OnSuccess()
	{
		// Arrange
		var collection = Substitute.For<IMongoCollection<Article>>();
		collection.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var handler = new CreateArticle.Handler(context, logger);

		var dto = new ArticleDto { Title = "LoggingTest" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();
		// Verify logger received an Information-level log containing the expected text
		logger.Received(1).Log(
				LogLevel.Information,
				Arg.Any<EventId>(),
				Arg.Is<object>(o => o != null && o.ToString()!.Contains("Category created successfully")),
				Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_LogsError_OnException()
	{
		// Arrange
		var collection = Substitute.For<IMongoCollection<Article>>();
		collection.When(c => c.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("DB error"));

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var handler = new CreateArticle.Handler(context, logger);

		var dto = new ArticleDto { Title = "T" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Failure.Should().BeTrue();
		// Verify logger received an Error-level log and an exception was passed through
		logger.Received(1).Log(
				LogLevel.Error,
				Arg.Any<EventId>(),
				Arg.Is<object>(o => o != null && o.ToString()!.Contains("Failed to create category")),
				Arg.Is<Exception>(e => e is InvalidOperationException && e.Message.Contains("DB error")),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_NullRequest_ReturnsFail()
	{
		// Arrange
		var collection = Substitute.For<IMongoCollection<Article>>();
		collection.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var handler = new CreateArticle.Handler(context, logger);

		// Act
		var result = await handler.HandleAsync(null);

		// Assert - should return failure and include exception information
		result.Failure.Should().BeTrue();
		result.Error.Should().NotBeNullOrEmpty();
	}

}
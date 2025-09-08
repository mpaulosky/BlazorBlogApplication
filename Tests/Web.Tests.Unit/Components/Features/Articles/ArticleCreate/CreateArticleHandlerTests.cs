// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateArticleHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Articles.ArticleCreate;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(CreateArticle.Handler))]
public class CreateArticleHandlerTests : BunitContext
{

	private readonly ArticlesTestFixture _fixture = new();
	private readonly CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;

	[Fact]
	public async Task HandleAsync_WithValidArticle_InsertsArticleAndReturnsOk()
	{
		// Arrange - use fixture-backed collection
		_fixture.ArticlesCollection
				.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		factory.CreateContext(_cancellationToken).Returns((MyBlogContext)_fixture.BlogContext);
		var handler = new CreateArticle.Handler(factory, logger);

		var dto = FakeArticleDto.GetNewArticleDto(true);

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();

		// Verify an Article was inserted, and PublishedOn was set (not default)
		_ = _fixture.ArticlesCollection
				.Received(1)
				.InsertOneAsync(
						Arg.Is<Article>(a => a.Title == dto.Title && a.Introduction == dto.Introduction && a.PublishedOn != null),
						Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_WhenInsertThrows_ReturnsFailWithErrorMessage()
	{
		// Arrange
		_fixture.ArticlesCollection.When(c =>
						c.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("DB error"));

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		factory.CreateContext(_cancellationToken).Returns((MyBlogContext)_fixture.BlogContext);
		var handler = new CreateArticle.Handler(factory, logger);

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
		_fixture.ArticlesCollection
				.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		factory.CreateContext(_cancellationToken).Returns((MyBlogContext)_fixture.BlogContext);
		var handler = new CreateArticle.Handler(factory, logger);

		var provided = DateTime.UtcNow.AddDays(-1);
		var dto = new ArticleDto { Title = "T", PublishedOn = provided };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();

		_ = _fixture.ArticlesCollection.Received(1).InsertOneAsync(
				Arg.Is<Article>(a => a.PublishedOn == provided && a.Title == dto.Title), Arg.Any<InsertOneOptions>(),
				Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_LogsInformation_OnSuccess()
	{
		// Arrange
		_fixture.ArticlesCollection
				.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		factory.CreateContext(_cancellationToken).Returns((MyBlogContext)_fixture.BlogContext);
		var handler = new CreateArticle.Handler(factory, logger);

		var dto = new ArticleDto { Title = "LoggingTest" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();

		// Verify logger received an Information-level log containing the expected text
		logger.Received(1).Log(
				LogLevel.Information,
				Arg.Any<EventId>(),
				Arg.Is<object>(o => o != null && o.ToString()!.Contains("Article created successfully")),
				Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_LogsError_OnException()
	{
		// Arrange
		_fixture.ArticlesCollection.When(c =>
						c.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("DB error"));

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		factory.CreateContext(_cancellationToken).Returns((MyBlogContext)_fixture.BlogContext);
		var handler = new CreateArticle.Handler(factory, logger);

		var dto = new ArticleDto { Title = "T" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Failure.Should().BeTrue();

		// Verify logger received an Error-level log and an exception was passed through
		logger.Received(1).Log(
				LogLevel.Error,
				Arg.Any<EventId>(),
				Arg.Is<object>(o => o != null && o.ToString()!.Contains("Failed to create article")),
				Arg.Is<Exception>(e => e is InvalidOperationException && e.Message.Contains("DB error")),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_NullRequest_ReturnsFail()
	{
		// Arrange
		_fixture.ArticlesCollection
				.InsertOneAsync(Arg.Any<Article>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var logger = Substitute.For<ILogger<CreateArticle.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		factory.CreateContext(_cancellationToken).Returns((MyBlogContext)_fixture.BlogContext);
		var handler = new CreateArticle.Handler(factory, logger);

		// Act
		var result = await handler.HandleAsync(null);

		// Assert - should return failure and include exception information
		result.Failure.Should().BeTrue();
		result.Error.Should().NotBeNullOrEmpty();
	}

}

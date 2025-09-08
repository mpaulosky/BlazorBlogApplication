// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetArticleHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Articles.ArticleDetails;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetArticle.Handler))]
public class GetArticleHandlerTests
{

	static GetArticleHandlerTests()
	{
		MapsterConfig.RegisterMappings();
	}

	private readonly ArticlesTestFixture _fixture = new();

	public enum FailureScenario
	{

		EMPTY_ID,

		NOT_FOUND,

		FIND_THROWS,

		ARTICLES_GETTER_THROWS

	}

	[Theory]
	[InlineData(FailureScenario.EMPTY_ID)]
	[InlineData(FailureScenario.NOT_FOUND)]
	[InlineData(FailureScenario.FIND_THROWS)]
	[InlineData(FailureScenario.ARTICLES_GETTER_THROWS)]
	public async Task HandleAsync_UnexpectedStates_ReturnsFailure(FailureScenario scenario)
	{
		// Arrange - prefer fixture-backed collection/context when possible
		var collection = _fixture.ArticlesCollection;
		var context = _fixture.BlogContext;

		switch (scenario)
		{
			case FailureScenario.EMPTY_ID:

				// fixture BlogContext already provides Articles collection
				break;

			case FailureScenario.NOT_FOUND:

				var emptyCursor = new StubCursor<Article>(new List<Article>());

				_fixture.ArticlesCollection
						.FindAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<FindOptions<Article, Article>>(),
								Arg.Any<CancellationToken>())
						.ReturnsForAnyArgs(Task.FromResult((IAsyncCursor<Article>)emptyCursor));

				break;

			case FailureScenario.FIND_THROWS:

				_fixture.ArticlesCollection.When(c => c.FindAsync(Arg.Any<FilterDefinition<Article>>(),
								Arg.Any<FindOptions<Article, Article>>(), Arg.Any<CancellationToken>()))
						.Do(_ => throw new InvalidOperationException("Find failed"));

				break;

			case FailureScenario.ARTICLES_GETTER_THROWS:

				// Simulate Articles property throwing when accessed
				// Create a substitute context that throws when Articles getter is accessed
				context = Substitute.For<IMyBlogContext>();
				context.Articles.Returns(_ => throw new InvalidOperationException("Getter fail"));

				break;
		}

		var logger = Substitute.For<ILogger<GetArticle.Handler>>();
		var handler = new GetArticle.Handler(new TestMyBlogContextFactory(context), logger);

		var id = scenario == FailureScenario.EMPTY_ID ? ObjectId.Empty : ObjectId.GenerateNewId();

		// Act
		var result = await handler.HandleAsync(id);

		// Assert
		result.Failure.Should().BeTrue();

		// Ensure an error message is present for all failure cases
		result.Error.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async Task HandleAsync_WithValidId_ReturnsArticleDto()
	{
		// Arrange
		var article = FakeArticle.GetNewArticle(true);

		var collection = Substitute.For<IMongoCollection<Article>>();
		var cursor = new StubCursor<Article>([article]);

		collection
				.FindAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<FindOptions<Article, Article>>(),
						Arg.Any<CancellationToken>())
				.ReturnsForAnyArgs(Task.FromResult<IAsyncCursor<Article>>(cursor));

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<GetArticle.Handler>>();
		var handler = new GetArticle.Handler(new TestMyBlogContextFactory(context), logger);

		//var id = ObjectId.GenerateNewId();

		// Act
		var result = await handler.HandleAsync(article.Id);

		// Assert
		if (result.Failure)
		{
			// Surface the failure message to help debug the test environment
			throw new InvalidOperationException($"Handler returned Failure: {result.Error}");
		}

		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value!.Title.Should().Be(article.Title);
		result.Value.UrlSlug.Should().Be(article.UrlSlug);
		result.Value.Id.Should().Be(article.Id);

	}

	[Fact]
	public async Task HandleAsync_EmptyId_ReturnsFailAndLogsError()
	{
		// Arrange
		var collection = Substitute.For<IMongoCollection<Article>>();
		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<GetArticle.Handler>>();
		var handler = new GetArticle.Handler(new TestMyBlogContextFactory(context), logger);

		// Act
		var result = await handler.HandleAsync(ObjectId.Empty);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("cannot be empty");

		logger.Received(1).Log(
				LogLevel.Error,
				Arg.Any<EventId>(),
				Arg.Is<object>(o => o != null && o.ToString()!.Contains("The ID is empty.")),
				Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_WhenArticleNotFound_ReturnsFailAndLogsWarning()
	{
		// Arrange - empty cursor
		var collection = Substitute.For<IMongoCollection<Article>>();
		var cursor = new StubCursor<Article>(new List<Article>());

		collection
				.FindAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<FindOptions<Article, Article>>(),
						Arg.Any<CancellationToken>())
				.ReturnsForAnyArgs(Task.FromResult((IAsyncCursor<Article>)cursor));

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<GetArticle.Handler>>();
		var handler = new GetArticle.Handler(new TestMyBlogContextFactory(context), logger);

		var id = ObjectId.GenerateNewId();

		// Act
		var result = await handler.HandleAsync(id);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("Article not found");

		logger.Received(1).Log(
				LogLevel.Warning,
				Arg.Any<EventId>(),
				Arg.Is<object>(o => o != null && o.ToString()!.Contains("Article not found")),
				Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_WhenFindThrows_ReturnsFailAndLogsError()
	{
		// Arrange - make FindAsync throw
		var collection = Substitute.For<IMongoCollection<Article>>();

		collection.When(c => c.FindAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<FindOptions<Article, Article>>(),
						Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("DB fail"));

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<GetArticle.Handler>>();
		var handler = new GetArticle.Handler(new TestMyBlogContextFactory(context), logger);

		var id = ObjectId.GenerateNewId();

		// Act
		var result = await handler.HandleAsync(id);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("DB fail");

		logger.Received(1).Log(
				LogLevel.Error,
				Arg.Any<EventId>(),
				Arg.Is<object>(o => o != null && o.ToString()!.Contains("Failed to find the article")),
				Arg.Is<Exception>(e => e is InvalidOperationException && e.Message.Contains("DB fail")),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task Diagnostic_StubCursor_FirstOrDefaultAsync_Works()
	{
		// Arrange - ensure the StubCursor behaves as expected when FirstOrDefaultAsync is called
		var article = FakeArticle.GetNewArticle(true);
		var cursor = new StubCursor<Article>([article]);

		// Act
		Article? found = null;
		Exception? ex = null;

		try
		{
			found = await cursor.FirstOrDefaultAsync(CancellationToken.None);
		}
		catch (Exception e)
		{
			ex = e;
		}

		// Assert - if an exception occurred, fail with its message to aid debugging
		if (ex is not null)
		{
			throw new InvalidOperationException($"Diagnostic failure: {ex.GetType().FullName} - {ex.Message}", ex);
		}

		found.Should().NotBeNull();
		found.Id.Should().Be(article.Id);
	}

	// Lightweight IMyBlogContextFactory stub used by handlers in tests
	private class TestMyBlogContextFactory : IMyBlogContextFactory
	{

		private readonly IMyBlogContext _ctx;

		public TestMyBlogContextFactory(IMyBlogContext ctx)
		{
			_ctx = ctx;
		}

		public Task<IMyBlogContext> CreateContext(CancellationToken cancellationToken = default)
		{
			return Task.FromResult(_ctx);
		}

		public MyBlogContext CreateContext()
		{
			return (MyBlogContext)_ctx;
		}

	}

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetArticlesHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Articles.ArticlesList;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetArticles.Handler))]
public class GetArticlesHandlerTests
{

	private readonly ArticlesTestFixture _fixture = new();

	[Fact]
	public async Task HandleAsync_ReturnsArticles_WhenFound()
	{
		// Arrange
		var articles = FakeArticle.GetArticles(2, true);

		_fixture.SetupFindAsync(articles);
		var handler = _fixture.CreateGetHandler();

		// Act
		var result = await handler.HandleAsync();

		// Assert
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Should().HaveCount(2);
	}

	[Fact]
	public async Task HandleAsync_ExcludeArchived_AppliesFilter()
	{
		// Arrange
		var articles = FakeArticle.GetArticles(2, true);
		articles[0].IsArchived = false;
		articles[1].IsArchived = true;

		_fixture.SetupFindAsync(articles.Where(a => !a.IsArchived).ToList());

		var handler = _fixture.CreateGetHandler();

		// Act
		var result = await handler.HandleAsync(true);

		// Assert
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Should().HaveCount(1);
	}

	[Fact]
	public async Task HandleAsync_ReturnsFail_WhenNoArticles()
	{
		// Arrange
		_fixture.SetupFindAsync(new List<Article>());
		var handler = _fixture.CreateGetHandler();

		// Act
		var result = await handler.HandleAsync();

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("No articles found");

		_fixture.Logger.Received(1).Log(LogLevel.Warning, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_ReturnsFail_OnException()
	{
		// Arrange
		// Arrange - simulate exception during FindAsync via collection substitute
		var collection = Substitute.For<IMongoCollection<Article>>();

		collection.When(c => c.FindAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<FindOptions<Article, Article>>(),
						Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("Find failed"));

		var context = Substitute.For<IMyBlogContext>();
		context.Articles.Returns(collection);

		var logger = Substitute.For<ILogger<GetArticles.Handler>>();
		var handler = new GetArticles.Handler(new TestMyBlogContextFactory(context), logger);

		// Act
		var result = await handler.HandleAsync();

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("Find failed");

		logger.Received(1).Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
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

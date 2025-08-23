// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticlesTestFixture.cs
// Company :       mpaulosky
// Author :        Copilot
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Components.Features.Articles.ArticleList;
using Web.Components.Features.Articles.ArticleDetails;
using Web.Components.Features.Categories; // for StubCursor
using static Web.Components.Features.Articles.ArticleDetails.GetArticle;
using Web.Components.Features.Articles.ArticleEdit;

namespace Web.Components.Features.Articles;

[CollectionDefinition(nameof(ArticlesTestFixture), DisableParallelization = true)]
[ExcludeFromCodeCoverage]
public class ArticlesTestFixture : IAsyncDisposable
{
	private IMongoClient MongoClient { get; }
	private IMongoDatabase MongoDatabase { get; }
	public IMongoCollection<Article> ArticlesCollection { get; }
	private MyBlogContext _blogContext;
	public IMyBlogContext BlogContext { get; }
	public ILogger<GetArticles.Handler> Logger { get; }
	public ILogger<EditArticle.Handler> EditLogger { get; }

	public ArticlesTestFixture()
	{
		MongoClient = Substitute.For<IMongoClient>();
		MongoDatabase = Substitute.For<IMongoDatabase>();
		ArticlesCollection = Substitute.For<IMongoCollection<Article>>();
		MongoClient.GetDatabase(Arg.Any<string>()).Returns(MongoDatabase);
		MongoDatabase.GetCollection<Article>(Arg.Any<string>()).Returns(ArticlesCollection);
		_blogContext = new MyBlogContext(MongoClient);
		BlogContext = _blogContext;
		Logger = Substitute.For<ILogger<GetArticles.Handler>>();
		EditLogger = Substitute.For<ILogger<EditArticle.Handler>>();
	}

	/// <summary>
	/// Configure the underlying articles collection to return the supplied articles
	/// from FindAsync via the generic <see cref="StubCursor{T}" />.
	/// </summary>
	public void SetupFindAsync(IEnumerable<Article> articles)
	{
		var cursor = new StubCursor<Article>(articles?.ToList() ?? new List<Article>());

		ArticlesCollection.FindAsync(Arg.Any<FilterDefinition<Article>>(), Arg.Any<FindOptions<Article, Article>>(), Arg.Any<CancellationToken>())
				.ReturnsForAnyArgs(Task.FromResult((IAsyncCursor<Article>)cursor));
	}

	/// <summary>
	/// Create a concrete GetArticles.Handler wired to the fixture's MyBlogContext and logger.
	/// Tests can register this into a bUnit TestContext or the test DI container.
	/// </summary>
	public GetArticles.Handler CreateGetHandler()
	{
		return new GetArticles.Handler(_blogContext, Logger as ILogger<GetArticles.Handler> ?? Substitute.For<ILogger<GetArticles.Handler>>());
	}

	/// <summary>
	/// Create a concrete EditArticle.Handler wired to the fixture's MyBlogContext.
	/// Tests can register this into a bUnit TestContext or the test DI container.
	/// </summary>
	public EditArticle.Handler CreateEditHandler()
	{
		return new EditArticle.Handler(_blogContext, EditLogger);
	}

	/// <summary>
	/// Helper to apply fixture-provided services into a bUnit <see cref="BunitContext" />.
	/// This keeps the TestContext per-test while sharing the mocks from the fixture.
	/// </summary>
	public void ApplyTo(BunitContext ctx)
	{
		if (ctx is null)
		{
			throw new ArgumentNullException(nameof(ctx));
		}

		// Register the concrete Get handler (components may resolve concrete handler types)
		var getHandler = CreateGetHandler();
		ctx.Services.AddScoped(_ => getHandler);
		ctx.Services.AddScoped<GetArticles.IGetArticlesHandler>(_ => getHandler);

		// Register Edit handler concrete type wired to the fixture's context via a factory stub
		var editHandler = CreateEditHandler();
		ctx.Services.AddScoped(_ => editHandler);
		ctx.Services.AddScoped<EditArticle.IEditArticleHandler>(_ => editHandler);

		// Register the concrete MyBlogContext so handlers resolving MyBlogContext get the fixture instance
		ctx.Services.AddScoped(_ => _blogContext);
		ctx.Services.AddScoped<IMyBlogContext>(_ => _blogContext);

		// Register logger instance used by handlers
		ctx.Services.AddSingleton(Logger);
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}

}

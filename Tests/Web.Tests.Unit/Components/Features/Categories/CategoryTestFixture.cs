// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryTestFixture.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Categories;

// Test fixture that provides common mocks, test data and helpers for category tests.
// Designed to be used as an xUnit class fixture (IClassFixture) or constructed directly
// by tests. It supports async disposal for future async resources.
[CollectionDefinition(nameof(CategoryTestFixture), DisableParallelization = true)]

// This fixture is not parallelized to avoid issues with shared MongoDB collections.
[ExcludeFromCodeCoverage]
public class CategoryTestFixture : IAsyncDisposable
{

	private IMongoClient MongoClient { get; }

	private IMongoDatabase MongoDatabase { get; }

	// Expose the collection so handler tests can configure InsertOneAsync behavior
	public IMongoCollection<Category> CategoriesCollection { get; }

	// Expose the IMyBlogContext for tests similar to ArticlesTestFixture
	public IMyBlogContext BlogContext { get; private set; }

	public ILogger<GetCategories.Handler> Logger { get; }

	public CategoryTestFixture()
	{
		MongoClient = Substitute.For<IMongoClient>();
		MongoDatabase = Substitute.For<IMongoDatabase>();
		CategoriesCollection = Substitute.For<IMongoCollection<Category>>();
		MongoClient.GetDatabase(Arg.Any<string>()).Returns(MongoDatabase);
		MongoDatabase.GetCollection<Category>(Arg.Any<string>()).Returns(CategoriesCollection);
		var blogContext = new MyBlogContext(MongoClient);
		BlogContext = blogContext;
		Logger = Substitute.For<ILogger<GetCategories.Handler>>();
	}

	// Lightweight IMyBlogContextFactory stub used by EditCategory.Handler in tests
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

	/// <summary>
	///   Configure the underlying categories collection to return the supplied categories
	///   from FindAsync via the generic <see cref="StubCursor{T}" />.
	/// </summary>
	public void SetupFindAsync(IEnumerable<Category> categories)
	{
		var cursor = new StubCursor<Category>(categories.ToList());

		// Match any filter/options/token so tests can call FindAsync with a filter
		// (for example, Builders<Category>.Filter.Eq("_id", id)) and still receive
		// the prepared cursor.
		CategoriesCollection
				.FindAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<FindOptions<Category, Category>>(),
						Arg.Any<CancellationToken>())
				.ReturnsForAnyArgs(Task.FromResult((IAsyncCursor<Category>)cursor));
	}

	/// <summary>
	///   Create a concrete GetCategory.Handler wired to the fixture's MyBlogContext and logger.
	///   Tests can register this into a bUnit TestContext or the test DI container.
	/// </summary>
	public GetCategory.Handler CreateGetCategoryHandler()
	{
		var categoryLogger = Substitute.For<ILogger<GetCategory.Handler>>();
		return new GetCategory.Handler(new TestMyBlogContextFactory(BlogContext), categoryLogger);
	}

	/// <summary>
	///   Create a concrete GetCategories.Handler wired to the fixture's MyBlogContext and logger.
	///   Tests can register this into a bUnit TestContext or the test DI container.
	/// </summary>
	public GetCategories.Handler CreateGetCategoriesHandler()
	{
		return new GetCategories.Handler(new TestMyBlogContextFactory(BlogContext), Logger);
	}

	/// <summary>
	///   Helper to apply fixture-provided services into a bUnit <see cref="BunitContext" />.
	///   This keeps the TestContext per-test while sharing the mocks from the fixture.
	/// </summary>
	public void ApplyTo(BunitContext ctx)
	{
		if (ctx is null)
		{
			throw new ArgumentNullException(nameof(ctx));
		}

		// Register the concrete handler (components may resolve concrete handler types)
		var getHandler = CreateGetCategoriesHandler();
		ctx.Services.AddScoped(_ => getHandler);

		// Also, register the handler interface so components injecting interfaces are satisfied
		ctx.Services.AddScoped<GetCategories.IGetCategoriesHandler>(_ => getHandler);

		// Register Edit handler concrete type wired to the fixture's context via a factory stub
		var editHandler = CreateEditHandler();
		ctx.Services.AddScoped(_ => editHandler);

		// And register the Edit handler interface used by components
		ctx.Services.AddScoped<EditCategory.IEditCategoryHandler>(_ => editHandler);

		// Register the concrete MyBlogContext so handlers resolving MyBlogContext get the fixture instance
		ctx.Services.AddScoped(_ => BlogContext);

		// Also register the IMyBlogContext and factory interfaces
		ctx.Services.AddScoped<IMyBlogContext>(_ => BlogContext);
		ctx.Services.AddScoped<IMyBlogContextFactory>(_ => new TestMyBlogContextFactory(BlogContext));

		// Register logger instance used by handlers
		ctx.Services.AddSingleton(Logger);
	}

	/// <summary>
	///   Create a concrete EditCategory.Handler wired to the fixture's MyBlogContext via a factory stub.
	///   Tests can register this into a bUnit TestContext or the test DI container.
	/// </summary>
	private EditCategory.Handler CreateEditHandler()
	{
		var editLogger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = new TestMyBlogContextFactory(BlogContext);

		return new EditCategory.Handler(factory, editLogger);
	}

	/// <summary>
	///   Convenience: configure the categories returned and return a handler already wired to the fixture context.
	/// </summary>
	public GetCategories.Handler ConfigureGetHandler(IEnumerable<Category> categories)
	{
		SetupFindAsync(categories);

		return CreateGetCategoriesHandler();
	}

	/// <summary>
	///   Async cleanup placeholder. If future fixtures allocate async resources, implement cleanup here.
	/// </summary>
	public ValueTask DisposeAsync()
	{
		// No async resources currently, but keep the method for future-proofing.
		return ValueTask.CompletedTask;
	}

}

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
public class CategoryTestFixture : IAsyncDisposable, IDisposable
{

	// Expose the ArticleDbContext for tests
	public ArticleDbContext BlogContext { get; private set; }

	public ILogger<GetCategories.Handler> Logger { get; }

	public CategoryTestFixture()
	{
		var options = new DbContextOptionsBuilder<ArticleDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		BlogContext = new ArticleDbContext(options);
		Logger = Substitute.For<ILogger<GetCategories.Handler>>();
	}

	// Lightweight IArticleDbContextFactory stub used by EditCategory.Handler in tests
	private class TestArticleDbContextFactory : IArticleDbContextFactory
	{

		private readonly ArticleDbContext _ctx;

		public TestArticleDbContextFactory(ArticleDbContext ctx)
		{
			_ctx = ctx;
		}

		public ArticleDbContext CreateDbContext()
		{
			return _ctx;
		}

	}

	/// <summary>
	///   Configure the underlying categories collection to return the supplied categories
	///   from Entity Framework in-memory database.
	/// </summary>
	public void SetupFindAsync(IEnumerable<Category> categories)
	{
		// Add categories to the in-memory database
		BlogContext.Categories.AddRange(categories);
		BlogContext.SaveChanges();
	}

	/// <summary>
	///   Create a concrete GetCategory.Handler wired to the fixture's MyzBlogContext and logger.
	///   Tests can register this into a bUnit TestContext or the test DI container.
	/// </summary>
	public GetCategory.Handler CreateGetCategoryHandler()
	{
		var categoryLogger = Substitute.For<ILogger<GetCategory.Handler>>();
		return new GetCategory.Handler(new TestArticleDbContextFactory(BlogContext), categoryLogger);
	}

	/// <summary>
	///   Create a concrete GetCategories.Handler wired to the fixture's ArticleDbContext and logger.
	///   Tests can register this into a bUnit TestContext or the test DI container.
	/// </summary>
	public GetCategories.Handler CreateGetCategoriesHandler()
	{
		return new GetCategories.Handler(new TestArticleDbContextFactory(BlogContext), Logger);
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

		// Register the concrete ArticleDbContext so handlers resolving ArticleDbContext get the fixture instance
		ctx.Services.AddScoped(_ => BlogContext);

		// Also register the factory interfaces
		ctx.Services.AddScoped<IArticleDbContextFactory>(_ => new TestArticleDbContextFactory(BlogContext));

		// Register logger instance used by handlers
		ctx.Services.AddSingleton(Logger);
	}

	/// <summary>
	///   Create a concrete EditCategory.Handler wired to the fixture's ArticleDbContext via a factory stub.
	///   Tests can register this into a bUnit TestContext or the test DI container.
	/// </summary>
	private EditCategory.Handler CreateEditHandler()
	{
		var editLogger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = new TestArticleDbContextFactory(BlogContext);

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
		Dispose();
		return ValueTask.CompletedTask;
	}

	/// <summary>
	///   Cleanup the ArticleDbContext
	/// </summary>
	public void Dispose()
	{
		BlogContext?.Dispose();
	}

}

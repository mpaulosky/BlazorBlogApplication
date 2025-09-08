// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategoriesHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Categories.CategoriesList;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetCategories.Handler))]
public class GetCategoriesHandlerTests
{

	private readonly CategoryTestFixture _fixture = new();

	[Fact]
	public async Task HandleAsync_ReturnsCategories_WhenFound()
	{
		// Arrange
		var categories = FakeCategory.GetCategories(2, true);
		_fixture.SetupFindAsync(categories);
		var logger = Substitute.For<ILogger<GetCategories.Handler>>();
		var handler = new GetCategories.Handler(new TestMyBlogContextFactory(_fixture.BlogContext), logger);

		// Act
		var result = await handler.HandleAsync();

		// Assert
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Should().HaveCount(2);
	}

	[Fact]
	public async Task HandleAsync_ReturnsFail_WhenNoCategories()
	{
		// Arrange
		_fixture.SetupFindAsync(new List<Category>());
		var logger = Substitute.For<ILogger<GetCategories.Handler>>();
		var handler = new GetCategories.Handler(new TestMyBlogContextFactory(_fixture.BlogContext), logger);

		// Act
		var result = await handler.HandleAsync();

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("No categories found");

		logger.Received(1).Log(LogLevel.Warning, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_ReturnsFail_OnException()
	{
		// Arrange - simulate exception during FindAsync via collection substitute
		var collection = Substitute.For<IMongoCollection<Category>>();

		collection
				.When(c => c.FindAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<FindOptions<Category, Category>>(),
						Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("Find failed"));

		var context = Substitute.For<IMyBlogContext>();
		context.Categories.Returns(collection);

		var logger = Substitute.For<ILogger<GetCategories.Handler>>();
		var handler = new GetCategories.Handler(new TestMyBlogContextFactory(context), logger);

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

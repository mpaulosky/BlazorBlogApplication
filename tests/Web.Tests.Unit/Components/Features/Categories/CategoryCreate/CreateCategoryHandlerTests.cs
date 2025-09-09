// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateCategoryHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Categories.CategoryCreate;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(CreateCategory.Handler))]
public class CreateCategoryHandlerTests
{

	private readonly CategoryTestFixture _fixture = new();

	[Fact]
	public async Task HandleAsync_InsertsCategoryAndReturnsOk()
	{
		// Arrange - ensure insert returns completed task
		_fixture.CategoriesCollection
				.InsertOneAsync(Arg.Any<Category>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var logger = Substitute.For<ILogger<CreateCategory.Handler>>();
		var handler = new CreateCategory.Handler(new TestMyBlogContextFactory(_fixture.BlogContext), logger);

		var dto = new CategoryDto { CategoryName = "Test Cat" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();

		_ = _fixture.CategoriesCollection.Received(1)
				.InsertOneAsync(Arg.Is<Category>(c => c.CategoryName == dto.CategoryName), Arg.Any<InsertOneOptions>(),
						Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_ReturnsFail_WhenInsertThrows()
	{
		// Arrange - make the collection throw on insert
		_fixture.CategoriesCollection.When(c =>
						c.InsertOneAsync(Arg.Any<Category>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("DB error"));

		var logger = Substitute.For<ILogger<CreateCategory.Handler>>();
		var handler = new CreateCategory.Handler(new TestMyBlogContextFactory(_fixture.BlogContext), logger);

		var dto = new CategoryDto { CategoryName = "T" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("DB error");

		logger.Received(1).Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_NullRequest_ReturnsFail()
	{
		// Arrange
		_fixture.CategoriesCollection
				.InsertOneAsync(Arg.Any<Category>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var logger = Substitute.For<ILogger<CreateCategory.Handler>>();
		var handler = new CreateCategory.Handler(new TestMyBlogContextFactory(_fixture.BlogContext), logger);

		// Act
		var result = await handler.HandleAsync(null);

		// Assert - should return failure and include exception information

		result.Failure.Should().BeTrue();
		result.Error.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async Task HandleAsync_EmptyName_InsertsEmptyAndReturnsOk()
	{
		// Arrange - ensure insert returns completed task
		_fixture.CategoriesCollection
				.InsertOneAsync(Arg.Any<Category>(), Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>())
				.Returns(Task.CompletedTask);

		var logger = Substitute.For<ILogger<CreateCategory.Handler>>();
		var handler = new CreateCategory.Handler(new TestMyBlogContextFactory(_fixture.BlogContext), logger);
		var dto = new CategoryDto { CategoryName = string.Empty };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert - current handler will accept empty names; ensure insert called and success returned
		result.Success.Should().BeTrue();

		_ = _fixture.CategoriesCollection.Received(1).InsertOneAsync(
				Arg.Is<Category>(c => c.CategoryName == dto.CategoryName), Arg.Any<InsertOneOptions>(),
				Arg.Any<CancellationToken>());

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

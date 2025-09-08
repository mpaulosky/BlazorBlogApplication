// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategoryHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Categories.CategoryDetails;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetCategory.Handler))]
public class GetCategoryHandlerTests
{

	private readonly CategoryTestFixture _fixture = new();

	[Fact]
	public async Task HandleAsync_ReturnsCategory_WhenFound()
	{
		// Arrange
		var category = FakeCategory.GetNewCategory(true);
		_fixture.SetupFindAsync(new List<Category> { category });
		var handler = _fixture.CreateGetCategoryHandler();

		// Act
		var result = await handler.HandleAsync(category.Id);

		// Assert
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value!.Id.Should().Be(category.Id);
	}

	[Fact]
	public async Task HandleAsync_ReturnsFail_WhenIdIsEmpty()
	{
		// Arrange
		var logger = Substitute.For<ILogger<GetCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new GetCategory.Handler(factory, logger);

		// Act
		var result = await handler.HandleAsync(ObjectId.Empty);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Be("The ID cannot be empty.");

		// Ensure no database call was performed
		await _fixture.CategoriesCollection.DidNotReceive().FindAsync(
				Arg.Any<FilterDefinition<Category>>(),
				Arg.Any<FindOptions<Category, Category>>(),
				Arg.Any<CancellationToken>());

		// Verify an error was logged
		logger.Received(1).Log(
				LogLevel.Error,
				Arg.Any<EventId>(),
				Arg.Any<object>(),
				Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_WhenFindThrows_ShouldReturnFailureAndLogError()
	{
		// Arrange
		var logger = Substitute.For<ILogger<GetCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new GetCategory.Handler(factory, logger);

		// Configure the CategoriesCollection to throw when FindAsync is called
		_fixture.CategoriesCollection
				.When(x => x.FindAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<FindOptions<Category, Category>>(),
						Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("boom"));

		// Act
		var result = await handler.HandleAsync(ObjectId.GenerateNewId());

		// Assert - use Result<T> API used across the codebase
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("boom");

		// Verify logger logged an error (signature matches other tests)
		logger.Received(1).Log(
				LogLevel.Error,
				Arg.Any<EventId>(),
				Arg.Any<object>(),
				Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategoryHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Categories.CategoryDetails;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetCategory.Handler))]
public class GetCategoryHandlerTests
{
	private readonly CategoryTestFixture _fixture = new ();

	[Fact]
	public async Task HandleAsync_ReturnsCategory_WhenFound()
	{
		// Arrange
		var category = FakeCategory.GetNewCategory(true);
		_fixture.SetupFindAsync(new List<Category> { category });
		var handler = _fixture.CreateGetHandler();

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
		var handler = _fixture.CreateGetHandler();

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
		_fixture.Logger.Received(1).Log(
			LogLevel.Error,
			Arg.Any<EventId>(),
			Arg.Any<object>(),
			Arg.Any<Exception?>(),
			Arg.Any<Func<object, Exception?, string>>());
	}
}
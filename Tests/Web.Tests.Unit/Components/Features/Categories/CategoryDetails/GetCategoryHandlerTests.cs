// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategoryHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

	[Fact]
	public async Task HandleAsync_WhenFindThrows_ShouldReturnFailureAndLogError()
	{
		// Arrange
		await using var fixture = new CategoryTestFixture();

		// Configure the CategoriesCollection to throw when FindAsync is called
		fixture.CategoriesCollection
				.When(x => x.FindAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<FindOptions<Category, Category>>(), Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("boom"));

		var handler = fixture.CreateGetHandler();

		// Act
		var result = await handler.HandleAsync(ObjectId.GenerateNewId());

		// Assert - use Result<T> API used across the codebase
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("boom");

		// Verify logger logged an error (signature matches other tests)
		fixture.Logger.Received(1).Log(
				LogLevel.Error,
				Arg.Any<EventId>(),
				Arg.Any<object>(),
				Arg.Any<Exception?>(),
				Arg.Any<Func<object, Exception?, string>>());
	}

}
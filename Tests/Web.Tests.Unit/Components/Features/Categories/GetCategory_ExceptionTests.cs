// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategory_ExceptionTests.cs
// Company :       mpaulosky
// Author :        Test
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using MongoDB.Bson;
using Web.Components.Features.Categories.CategoryDetails;

namespace Web.Components.Features.Categories;

[ExcludeFromCodeCoverage]
public class GetCategory_ExceptionTests
{
	[Fact]
	public async Task HandleAsync_WhenFindThrows_ShouldReturnFailureAndLogError()
	{
		// Arrange
		await using var fixture = new CategoryTestFixture();

		// Configure the CategoriesCollection to throw when FindAsync is called
		fixture.CategoriesCollection
				.When(x => x.FindAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<FindOptions<Category, Category>>(), Arg.Any<CancellationToken>()))
				.Do(ci => throw new InvalidOperationException("boom"));

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

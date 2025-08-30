// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditCategoryHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using System.Threading;
using System.Threading.Tasks;

namespace Web.Components.Features.Categories.CategoryEdit;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(EditCategory.Handler))]
public class EditCategoryHandlerTests
{
	private readonly CategoryTestFixture _fixture = new();

	[Fact]
	public async Task HandleAsync_WithValidCategory_ReplacesCategory_ReturnsOk_AndLogsInformation()
	{
		// Arrange
		_fixture.CategoriesCollection
			.ReplaceOneAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<Category>(), Arg.Any<ReplaceOptions>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<ReplaceOneResult?>(null!));

		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var dto = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "Updated Name" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();
		_ = _fixture.CategoriesCollection.Received(1).ReplaceOneAsync(
			Arg.Any<FilterDefinition<Category>>(),
			Arg.Is<Category>(c => c.CategoryName == dto.CategoryName && c.ModifiedOn.HasValue),
			Arg.Any<ReplaceOptions>(),
			Arg.Any<CancellationToken>());

		logger.Received(1).Log(
			LogLevel.Information,
			Arg.Any<EventId>(),
			Arg.Is<object>(o => o != null && o.ToString()!.Contains("Category updated successfully")),
			Arg.Any<Exception?>(),
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_NotFoundId_StillReturnsOk_AndLogsInformation()
	{
		// Arrange: simulate a replacement call that completes but does not throw (handler does not inspect a result)
		_fixture.CategoriesCollection
			.ReplaceOneAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<Category>(), Arg.Any<ReplaceOptions>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<ReplaceOneResult?>(null!));

		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var dto = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "DoesNotExist" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert: current handler treats this as success
		result.Success.Should().BeTrue();
		_ = _fixture.CategoriesCollection.Received(1).ReplaceOneAsync(
			Arg.Any<FilterDefinition<Category>>(),
			Arg.Any<Category>(),
			Arg.Any<ReplaceOptions>(),
			Arg.Any<CancellationToken>());

		logger.Received(1).Log(
			LogLevel.Information,
			Arg.Any<EventId>(),
			Arg.Is<object>(o => o != null && o.ToString()!.Contains("Category updated successfully")),
			Arg.Any<Exception?>(),
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_WhenReplaceThrows_ReturnsFail_AndLogsError()
	{
		// Arrange
		_fixture.CategoriesCollection
			.When(c => c.ReplaceOneAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<Category>(), Arg.Any<ReplaceOptions>(), Arg.Any<CancellationToken>()))
			.Do(_ => throw new InvalidOperationException("DB error"));

		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var dto = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "Any" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("DB error");
		logger.Received(1).Log(
			LogLevel.Error,
			Arg.Any<EventId>(),
			Arg.Is<object>(o => o != null && o.ToString()!.Contains("Failed to update")),
			Arg.Is<Exception>(e => e is InvalidOperationException && e.Message.Contains("DB error")),
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task HandleAsync_NullRequest_CurrentlyThrowsNullReferenceException()
	{
		// Arrange
		_fixture.CategoriesCollection
			.ReplaceOneAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<Category>(), Arg.Any<ReplaceOptions>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<ReplaceOneResult?>(null!));

		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		// Act
		var act = async () => await handler.HandleAsync(null!);

		// Assert - document current behavior: a NullReferenceException bubbles due to logging request.CategoryName in catch
		await act.Should().ThrowAsync<NullReferenceException>();
		// Ensure no Information log was written
		logger.DidNotReceive().Log(
			LogLevel.Information,
			Arg.Any<EventId>(),
			Arg.Any<object>(),
			Arg.Any<Exception?>(),
			Arg.Any<Func<object, Exception?, string>>());
	}
}
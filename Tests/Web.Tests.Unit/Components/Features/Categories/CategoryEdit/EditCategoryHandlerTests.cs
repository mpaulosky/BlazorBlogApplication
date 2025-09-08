// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditCategoryHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

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
				.UpdateOneAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<UpdateDefinition<Category>>(), Arg.Any<UpdateOptions>(),
						Arg.Any<CancellationToken>())
				.Returns(_ => Task.FromResult<UpdateResult>(new UpdateResult.Acknowledged(1, 1, null)));

		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var dto = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "Updated Name" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();

		_ = _fixture.CategoriesCollection.Received(1).UpdateOneAsync(
				Arg.Any<FilterDefinition<Category>>(),
				Arg.Any<UpdateDefinition<Category>>(),
				Arg.Any<UpdateOptions>(),
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
				.UpdateOneAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<UpdateDefinition<Category>>(), Arg.Any<UpdateOptions>(),
						Arg.Any<CancellationToken>())
				.Returns(_ => Task.FromResult<UpdateResult>(new UpdateResult.Acknowledged(1, 1, null)));

		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var dto = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "DoesNotExist" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert: current handler treats this as success
		result.Success.Should().BeTrue();

		_ = _fixture.CategoriesCollection.Received(1).UpdateOneAsync(
				Arg.Any<FilterDefinition<Category>>(),
				Arg.Any<UpdateDefinition<Category>>(),
				Arg.Any<UpdateOptions>(),
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
				.When(c => c.UpdateOneAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<UpdateDefinition<Category>>(),
						Arg.Any<UpdateOptions>(), Arg.Any<CancellationToken>()))
				.Do(_ => throw new InvalidOperationException("DB error"));

		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
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
	public async Task HandleAsync_EmptyCategoryName_ReturnsFail()
	{
		// Arrange
		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var dto = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("Category name cannot be empty");

		// Ensure no database call was performed
		await _fixture.CategoriesCollection.DidNotReceive().ReplaceOneAsync(
				Arg.Any<FilterDefinition<Category>>(),
				Arg.Any<Category>(),
				Arg.Any<ReplaceOptions>(),
				Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_WhitespaceCategoryName_ReturnsFail()
	{
		// Arrange
		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var dto = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "   " };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("Category name cannot be empty");

		// Ensure no database call was performed
		await _fixture.CategoriesCollection.DidNotReceive().ReplaceOneAsync(
				Arg.Any<FilterDefinition<Category>>(),
				Arg.Any<Category>(),
				Arg.Any<ReplaceOptions>(),
				Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_VeryLongCategoryName_StillProcessesSuccessfully()
	{
		// Arrange
		_fixture.CategoriesCollection
				.UpdateOneAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<UpdateDefinition<Category>>(), Arg.Any<UpdateOptions>(),
						Arg.Any<CancellationToken>())
				.Returns(_ => Task.FromResult<UpdateResult>(new UpdateResult.Acknowledged(1, 1, null)));

		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var longName = new string('A', 500); // Very long category name
		var dto = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = longName };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();

		_ = _fixture.CategoriesCollection.Received(1).UpdateOneAsync(
				Arg.Any<FilterDefinition<Category>>(),
				Arg.Any<UpdateDefinition<Category>>(),
				Arg.Any<UpdateOptions>(),
				Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_CategoryNameWithSpecialCharacters_ProcessesSuccessfully()
	{
		// Arrange
		_fixture.CategoriesCollection
				.UpdateOneAsync(Arg.Any<FilterDefinition<Category>>(), Arg.Any<UpdateDefinition<Category>>(), Arg.Any<UpdateOptions>(),
						Arg.Any<CancellationToken>())
				.Returns(_ => Task.FromResult<UpdateResult>(new UpdateResult.Acknowledged(1, 1, null)));

		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var specialName = "Test & Category < > \" ' ";
		var dto = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = specialName };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Success.Should().BeTrue();

		_ = _fixture.CategoriesCollection.Received(1).UpdateOneAsync(
				Arg.Any<FilterDefinition<Category>>(),
				Arg.Any<UpdateDefinition<Category>>(),
				Arg.Any<UpdateOptions>(),
				Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task HandleAsync_EmptyObjectId_ReturnsFail()
	{
		// Arrange
		var logger = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = Substitute.For<IMyBlogContextFactory>();
		factory.CreateContext(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_fixture.BlogContext));
		var handler = new EditCategory.Handler(factory, logger);

		var dto = new CategoryDto { Id = ObjectId.Empty, CategoryName = "Test Category" };

		// Act
		var result = await handler.HandleAsync(dto);

		// Assert
		result.Failure.Should().BeTrue();
		result.Error.Should().Contain("ID");

		// Ensure no database call was performed
		await _fixture.CategoriesCollection.DidNotReceive().ReplaceOneAsync(
				Arg.Any<FilterDefinition<Category>>(),
				Arg.Any<Category>(),
				Arg.Any<ReplaceOptions>(),
				Arg.Any<CancellationToken>());
	}

}

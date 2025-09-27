// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditCategoryHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests (migrated from Web.Tests.Integration)
// =======================================================
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Categories.CategoryEdit;

namespace Web.Features.Categories;

/// <summary>
/// Integration tests for the EditCategory.Handler class.
/// Tests the complete workflow of updating categories in the database.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(EditCategory.Handler))]
[Collection("Test Collection")]
public class EditCategoryHandlerTests
{
	private readonly WebTestFactory _factory;
	private readonly ILogger<EditCategory.Handler> _logger;
	private readonly IApplicationDbContextFactory _contextFactory;

	public EditCategoryHandlerTests(WebTestFactory factory)
	{
		_factory = factory;
		using var scope = _factory.Services.CreateScope();
		_logger = scope.ServiceProvider.GetRequiredService<ILogger<EditCategory.Handler>>();
		_contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
	}

	[Fact]
	public async Task HandleAsync_WithValidCategoryUpdate_ReturnsSuccessAndUpdatesCategory()
	{
		// Arrange  
		var categoryId = await CreateValidCategoryAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);
		var updateRequest = new CategoryDto 
		{ 
			Id = categoryId,
			CategoryName = "Updated Category Name"
		};

		// Act
		var result = await handler.HandleAsync(updateRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();

		// Verify the category was actually updated in the database
		await VerifyCategoryUpdatedInDatabaseAsync(categoryId, "Updated Category Name");
	}

	[Fact]
	public async Task HandleAsync_WithNullRequest_ReturnsFailureResult()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(null);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
		result.Error.Should().Be("The request is null.");
	}

	[Fact]
	public async Task HandleAsync_WithEmptyOrWhitespaceCategoryName_ReturnsFailureResult()
	{
		// Arrange  
		var categoryId = await CreateValidCategoryAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);

		// Test empty string
		var emptyNameRequest = new CategoryDto 
		{ 
			Id = categoryId,
			CategoryName = string.Empty
		};

		// Act & Assert - Empty name
		var emptyResult = await handler.HandleAsync(emptyNameRequest);
		emptyResult.Should().NotBeNull();
		emptyResult.Success.Should().BeFalse();
		emptyResult.Error.Should().Be("Category name cannot be empty or whitespace.");

		// Test whitespace string
		var whitespaceNameRequest = new CategoryDto 
		{ 
			Id = categoryId,
			CategoryName = "   "
		};

		var whitespaceResult = await handler.HandleAsync(whitespaceNameRequest);
		whitespaceResult.Should().NotBeNull();
		whitespaceResult.Success.Should().BeFalse();
		whitespaceResult.Error.Should().Be("Category name cannot be empty or whitespace.");
	}

	[Fact]
	public async Task HandleAsync_WithEmptyGuid_ReturnsFailureResult()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);
		var invalidRequest = new CategoryDto 
		{ 
			Id = Guid.Empty,
			CategoryName = "Valid Name"
		};

		// Act
		var result = await handler.HandleAsync(invalidRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
		result.Error.Should().Be("Category ID cannot be empty.");
	}

	[Fact]
	public async Task HandleAsync_WithNonExistentCategoryId_ReturnsFailureResult()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);
		var nonExistentId = Guid.NewGuid();
		var updateRequest = new CategoryDto 
		{ 
			Id = nonExistentId,
			CategoryName = "Updated Name"
		};

		// Act
		var result = await handler.HandleAsync(updateRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
		result.Error.Should().Be("Category not found.");
	}

	[Fact]
	public async Task HandleAsync_UpdatesModifiedOnTimestamp()
	{
		// Arrange  
		var categoryId = await CreateValidCategoryAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);
		var updateRequest = new CategoryDto 
		{ 
			Id = categoryId,
			CategoryName = "Updated With Timestamp"
		};

		// Get original modified date (should be null for new category)
		var originalModifiedOn = await GetCategoryModifiedOnAsync(categoryId);

		// Act
		var result = await handler.HandleAsync(updateRequest);

		// Assert
		result.Success.Should().BeTrue();

		// Verify ModifiedOn was updated
		var updatedModifiedOn = await GetCategoryModifiedOnAsync(categoryId);
		updatedModifiedOn.Should().NotBeNull();
		updatedModifiedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
		updatedModifiedOn.Should().NotBe(originalModifiedOn);
	}

	[Fact]
	public async Task HandleAsync_WithArchivedCategory_UpdatesSuccessfully()
	{
		// Arrange  
		var categoryId = await CreateArchivedCategoryAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);
		var updateRequest = new CategoryDto 
		{ 
			Id = categoryId,
			CategoryName = "Updated Archived Category"
		};

		// Act
		var result = await handler.HandleAsync(updateRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();

		// Verify the archived category was updated but remains archived
		await VerifyCategoryUpdatedInDatabaseAsync(categoryId, "Updated Archived Category");
		await VerifyCategoryIsArchivedAsync(categoryId, expectedArchived: true);
	}

	[Fact]
	public async Task HandleAsync_WithSpecialCharacters_UpdatesCategory()
	{
		// Arrange  
		var categoryId = await CreateValidCategoryAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);
		var specialName = "C# & .NET (Framework/Core) - Updated!";
		var updateRequest = new CategoryDto 
		{ 
			Id = categoryId,
			CategoryName = specialName
		};

		// Act
		var result = await handler.HandleAsync(updateRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();

		// Verify the category was updated with special characters intact
		await VerifyCategoryUpdatedInDatabaseAsync(categoryId, specialName);
	}

	[Fact]
	public async Task HandleAsync_WithLongCategoryName_HandlesConstraintViolation()
	{
		// Arrange  
		var categoryId = await CreateValidCategoryAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);
		var longName = new string('A', 100); // 100 characters, exceeds max length of 80
		var updateRequest = new CategoryDto 
		{ 
			Id = categoryId,
			CategoryName = longName
		};

		// Act & Assert - This should fail due to database constraints
		var result = await handler.HandleAsync(updateRequest);
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();

		// Verify the original category name was not changed
		await VerifyCategoryUpdatedInDatabaseAsync(categoryId, "Original Category");
	}

	[Fact]
	public async Task HandleAsync_WithMultipleUpdates_PersistsLatestUpdate()
	{
		// Arrange  
		var categoryId = await CreateValidCategoryAsync();
		var handler = new EditCategory.Handler(_contextFactory, _logger);

		// Act - Perform multiple updates
		var update1 = new CategoryDto { Id = categoryId, CategoryName = "First Update" };
		var update2 = new CategoryDto { Id = categoryId, CategoryName = "Second Update" };
		var update3 = new CategoryDto { Id = categoryId, CategoryName = "Final Update" };

		var result1 = await handler.HandleAsync(update1);
		var result2 = await handler.HandleAsync(update2);
		var result3 = await handler.HandleAsync(update3);

		// Assert
		result1.Success.Should().BeTrue();
		result2.Success.Should().BeTrue();
		result3.Success.Should().BeTrue();

		// Verify the final update is persisted
		await VerifyCategoryUpdatedInDatabaseAsync(categoryId, "Final Update");
	}

	/// <summary>
	/// Creates a valid test category for update testing.
	/// </summary>
	private async Task<Guid> CreateValidCategoryAsync()
	{
		await _factory.ResetDatabaseAsync();

		using var context = _contextFactory.CreateDbContext();

		var category = new Category("Original Category");
		context.Categories.Add(category);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		return category.Id;
	}

	/// <summary>
	/// Creates an archived test category.
	/// </summary>
	private async Task<Guid> CreateArchivedCategoryAsync()
	{
		await _factory.ResetDatabaseAsync();

		using var context = _contextFactory.CreateDbContext();

		var category = new Category("Archived Category", isArchived: true);
		context.Categories.Add(category);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		return category.Id;
	}

	/// <summary>
	/// Verifies that a category was updated with the expected name.
	/// </summary>
	private async Task VerifyCategoryUpdatedInDatabaseAsync(Guid categoryId, string expectedName)
	{
		using var context = _contextFactory.CreateDbContext();
		var category = await context.Categories.FirstAsync(c => c.Id == categoryId, TestContext.Current.CancellationToken);
		category.CategoryName.Should().Be(expectedName);
	}

	/// <summary>
	/// Gets the ModifiedOn timestamp for a category.
	/// </summary>
	private async Task<DateTime?> GetCategoryModifiedOnAsync(Guid categoryId)
	{
		using var context = _contextFactory.CreateDbContext();
		var category = await context.Categories.FirstAsync(c => c.Id == categoryId, TestContext.Current.CancellationToken);
		return category.ModifiedOn;
	}

	/// <summary>
	/// Verifies the archived status of a category.
	/// </summary>
	private async Task VerifyCategoryIsArchivedAsync(Guid categoryId, bool expectedArchived)
	{
		using var context = _contextFactory.CreateDbContext();
		var category = await context.Categories.FirstAsync(c => c.Id == categoryId, TestContext.Current.CancellationToken);
		category.IsArchived.Should().Be(expectedArchived);
	}
}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategoryHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Categories.CategoryDetails;

namespace Web.Features.Categories;

/// <summary>
/// Integration tests for the GetCategory.Handler class.
/// Tests the complete workflow of getting a category from the database by ID.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetCategory.Handler))]
[Collection("Test Collection")]
public class GetCategoryHandlerTests
{
	private readonly WebTestFactory _factory;
	private readonly ILogger<GetCategory.Handler> _logger;
	private readonly IApplicationDbContextFactory _contextFactory;

	public GetCategoryHandlerTests(WebTestFactory factory)
	{
		_factory = factory;
		using var scope = _factory.Services.CreateScope();
		_logger = scope.ServiceProvider.GetRequiredService<ILogger<GetCategory.Handler>>();
		_contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
	}

	[Fact]
	public async Task HandleAsync_WithEmptyGuid_ReturnsFailureResult()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new GetCategory.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(Guid.Empty);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
		result.Error.Should().Be("The ID cannot be empty.");
		result.Value.Should().BeNull();
	}

	[Fact]
	public async Task HandleAsync_WithNonExistentId_ReturnsFailureResult()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new GetCategory.Handler(_contextFactory, _logger);
		var nonExistentId = Guid.NewGuid();

		// Act
		var result = await handler.HandleAsync(nonExistentId);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
		result.Error.Should().Be("Category not found.");
		result.Value.Should().BeNull();
	}

	[Fact]
	public async Task HandleAsync_WithValidExistingCategory_ReturnsSuccessWithCompleteDetails()
	{
		// Arrange
		var categoryId = await CreateValidCategoryAsync();
		var handler = new GetCategory.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(categoryId);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();
		result.Value.Should().NotBeNull();

		// Verify category details
		result.Value.Id.Should().Be(categoryId);
		result.Value.CategoryName.Should().Be("Test Category");
		result.Value.IsArchived.Should().BeFalse();
		result.Value.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
		result.Value.ModifiedOn.Should().BeNull();
	}

	[Fact]
	public async Task HandleAsync_WithArchivedCategory_ReturnsArchivedCategory()
	{
		// Arrange
		var categoryId = await CreateArchivedCategoryAsync();
		var handler = new GetCategory.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(categoryId);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();
		result.Value.Should().NotBeNull();

		// Verify archived category details
		result.Value.Id.Should().Be(categoryId);
		result.Value.CategoryName.Should().Be("Archived Category");
		result.Value.IsArchived.Should().BeTrue();
		result.Value.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
		result.Value.ModifiedOn.Should().BeNull();
	}

	[Fact]
	public async Task HandleAsync_WithModifiedCategory_ReturnsUpdatedCategory()
	{
		// Arrange
		var categoryId = await CreateModifiedCategoryAsync();
		var handler = new GetCategory.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(categoryId);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();
		result.Value.Should().NotBeNull();

		// Verify modified category details
		result.Value.Id.Should().Be(categoryId);
		result.Value.CategoryName.Should().Be("Modified Category");
		result.Value.IsArchived.Should().BeFalse();
		result.Value.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
		result.Value.ModifiedOn.Should().NotBeNull();
		result.Value.ModifiedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
	}

	[Fact]
	public async Task HandleAsync_ReturnsProperlyMappedCategoryDto()
	{
		// Arrange
		var categoryId = await CreateValidCategoryAsync();
		var handler = new GetCategory.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(categoryId);

		// Assert - Verify all DTO properties are properly mapped
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();

		var category = result.Value;
		category.Id.Should().NotBe(Guid.Empty);
		category.CategoryName.Should().NotBeNullOrWhiteSpace();
		category.CreatedOn.Should().NotBe(default);

		// Verify the DTO is the correct type
		category.Should().BeOfType<CategoryDto>();
		
		// Verify IsArchived property is properly mapped (boolean should have a valid value)
		category.IsArchived.Should().BeFalse();  // We created a non-archived category
	}

	[Fact]
	public async Task HandleAsync_WithDatabaseContextDisposal_HandlesResourcesCorrectly()
	{
		// Arrange
		var categoryId = await CreateValidCategoryAsync();
		var handler = new GetCategory.Handler(_contextFactory, _logger);

		// Act & Assert - Multiple calls should not cause disposal issues
		var result1 = await handler.HandleAsync(categoryId);
		var result2 = await handler.HandleAsync(categoryId);
		var result3 = await handler.HandleAsync(categoryId);

		result1.Success.Should().BeTrue();
		result2.Success.Should().BeTrue();
		result3.Success.Should().BeTrue();

		result1.Value.Should().NotBeNull();
		result2.Value.Should().NotBeNull();
		result3.Value.Should().NotBeNull();

		result1.Value!.CategoryName.Should().Be(result2.Value!.CategoryName);
		result2.Value!.CategoryName.Should().Be(result3.Value!.CategoryName);
	}

	[Fact]
	public async Task HandleAsync_WithMultipleCategories_ReturnsSpecificCategoryById()
	{
		// Arrange
		await CreateMultipleDifferentCategoriesAsync();
		var targetCategoryId = await GetCategoryIdByNameAsync("Target Category");
		var handler = new GetCategory.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(targetCategoryId);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.CategoryName.Should().Be("Target Category");
		result.Value.Id.Should().Be(targetCategoryId);

		// Ensure it's the correct category and not another one
		result.Value.CategoryName.Should().NotBe("Other Category 1");
		result.Value.CategoryName.Should().NotBe("Other Category 2");
	}

	/// <summary>
	/// Creates a valid test category for basic tests.
	/// </summary>
	private async Task<Guid> CreateValidCategoryAsync(bool resetDatabase = true)
	{
		if (resetDatabase)
		{
			await _factory.ResetDatabaseAsync();
		}

		using var context = _contextFactory.CreateDbContext();

		var category = new Category("Test Category");
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
	/// Creates a modified test category with ModifiedOn date set.
	/// </summary>
	private async Task<Guid> CreateModifiedCategoryAsync()
	{
		await _factory.ResetDatabaseAsync();

		using var context = _contextFactory.CreateDbContext();

		var category = new Category("Modified Category");
		context.Categories.Add(category);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Update to set ModifiedOn
		category.CategoryName = "Modified Category";
		category.ModifiedOn = DateTime.UtcNow;
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		return category.Id;
	}

	/// <summary>
	/// Creates multiple categories with different names for targeted retrieval tests.
	/// </summary>
	private async Task CreateMultipleDifferentCategoriesAsync()
	{
		await _factory.ResetDatabaseAsync();

		using var context = _contextFactory.CreateDbContext();

		var categories = new List<Category>
		{
			new("Target Category"),
			new("Other Category 1"),
			new("Other Category 2")
		};

		context.Categories.AddRange(categories);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
	}

	/// <summary>
	/// Gets the category ID by category name.
	/// </summary>
	private async Task<Guid> GetCategoryIdByNameAsync(string categoryName)
	{
		using var context = _contextFactory.CreateDbContext();
		var category = await context.Categories.FirstAsync(c => c.CategoryName == categoryName, TestContext.Current.CancellationToken);
		return category.Id;
	}
}
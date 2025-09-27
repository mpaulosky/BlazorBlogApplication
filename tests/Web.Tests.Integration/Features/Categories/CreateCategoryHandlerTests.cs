// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateCategoryHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests (migrated from Web.Tests.Integration)
// =======================================================
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Categories.CategoryCreate;

namespace Web.Features.Categories;

/// <summary>
/// Integration tests for the CreateCategory.Handler class.
/// Tests the complete workflow of creating categories in the database.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(CreateCategory.Handler))]
[Collection("Test Collection")]
public class CreateCategoryHandlerTests
{
	private readonly WebTestFactory _factory;
	private readonly ILogger<CreateCategory.Handler> _logger;
	private readonly IApplicationDbContextFactory _contextFactory;

	public CreateCategoryHandlerTests(WebTestFactory factory)
	{
		_factory = factory;
		using var scope = _factory.Services.CreateScope();
		_logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateCategory.Handler>>();
		_contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
	}

	[Fact]
	public async Task HandleAsync_WithValidCategoryDto_ReturnsSuccessAndCreatesCategory()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new CreateCategory.Handler(_contextFactory, _logger);
		var categoryDto = new CategoryDto { CategoryName = "Test Category" };

		// Act
		var result = await handler.HandleAsync(categoryDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();

		// Verify the category was actually created in the database
		await VerifyCategoryExistsInDatabaseAsync("Test Category");
	}

	[Fact]
	public async Task HandleAsync_WithNullRequest_ReturnsFailureResult()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new CreateCategory.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(null);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
		result.Error.Should().Be("The request is null.");

		// Verify no category was created
		await VerifyNoCategoriesInDatabaseAsync();
	}

	[Fact]
	public async Task HandleAsync_WithEmptyCategoryName_CreatesCategory()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new CreateCategory.Handler(_contextFactory, _logger);
		var categoryDto = new CategoryDto { CategoryName = string.Empty };

		// Act
		var result = await handler.HandleAsync(categoryDto);

		// Assert - The handler doesn't validate, it just creates the category with the empty name
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();

		// Verify the category was created even with empty name
		await VerifyCategoryExistsInDatabaseAsync(string.Empty);
	}

	[Fact]
	public async Task HandleAsync_WithWhitespaceCategoryName_CreatesCategory()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new CreateCategory.Handler(_contextFactory, _logger);
		var categoryDto = new CategoryDto { CategoryName = "   " };

		// Act
		var result = await handler.HandleAsync(categoryDto);

		// Assert - The handler doesn't validate, it just creates the category
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();

		// Verify the category was created with whitespace name
		await VerifyCategoryExistsInDatabaseAsync("   ");
	}

	[Fact]
	public async Task HandleAsync_WithLongCategoryName_CreatesCategory()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new CreateCategory.Handler(_contextFactory, _logger);
		var longName = new string('A', 100); // 100 characters, exceeds max length of 80
		var categoryDto = new CategoryDto { CategoryName = longName };

		// Act & Assert - This should throw or fail due to database constraints
		var result = await handler.HandleAsync(categoryDto);
        
		// The handler might succeed but database constraints should apply
		// This tests the actual database behavior with constraint violations
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async Task HandleAsync_WithValidCategoryName_SetsCorrectDatabaseProperties()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new CreateCategory.Handler(_contextFactory, _logger);
		var categoryDto = new CategoryDto { CategoryName = "Technology" };

		// Act
		var result = await handler.HandleAsync(categoryDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the database entity was created with correct properties
		using var context = _contextFactory.CreateDbContext();
		var category = await context.Categories.FirstAsync(c => c.CategoryName == "Technology", TestContext.Current.CancellationToken);
        
		category.Should().NotBeNull();
		category.CategoryName.Should().Be("Technology");
		category.Id.Should().NotBe(Guid.Empty);
		category.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
		category.ModifiedOn.Should().BeNull();
		category.IsArchived.Should().BeFalse();
	}

	[Fact]
	public async Task HandleAsync_WithMultipleCategories_CreatesAllCategories()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new CreateCategory.Handler(_contextFactory, _logger);

		// Act - Create multiple categories
		var result1 = await handler.HandleAsync(new CategoryDto { CategoryName = "Category 1" });
		var result2 = await handler.HandleAsync(new CategoryDto { CategoryName = "Category 2" });
		var result3 = await handler.HandleAsync(new CategoryDto { CategoryName = "Category 3" });

		// Assert
		result1.Success.Should().BeTrue();
		result2.Success.Should().BeTrue();
		result3.Success.Should().BeTrue();

		// Verify all categories exist in database
		await VerifyCategoryExistsInDatabaseAsync("Category 1");
		await VerifyCategoryExistsInDatabaseAsync("Category 2");
		await VerifyCategoryExistsInDatabaseAsync("Category 3");

		// Verify total count
		using var context = _contextFactory.CreateDbContext();
		var totalCount = await context.Categories.CountAsync(TestContext.Current.CancellationToken);
		totalCount.Should().Be(3);
	}

	[Fact]
	public async Task HandleAsync_WithDuplicateCategoryNames_CreatesBothCategories()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new CreateCategory.Handler(_contextFactory, _logger);

		// Act - Create categories with same name (no unique constraint on name)
		var result1 = await handler.HandleAsync(new CategoryDto { CategoryName = "Duplicate Name" });
		var result2 = await handler.HandleAsync(new CategoryDto { CategoryName = "Duplicate Name" });

		// Assert
		result1.Success.Should().BeTrue();
		result2.Success.Should().BeTrue();

		// Verify both categories exist (they should have different IDs)
		using var context = _contextFactory.CreateDbContext();
		var duplicateCategories = await context.Categories.Where(c => c.CategoryName == "Duplicate Name").ToListAsync(TestContext.Current.CancellationToken);
		duplicateCategories.Should().HaveCount(2);
		duplicateCategories[0].Id.Should().NotBe(duplicateCategories[1].Id);
	}

	[Fact]
	public async Task HandleAsync_WithSpecialCharacters_CreatesCategory()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new CreateCategory.Handler(_contextFactory, _logger);
		var specialName = "C# & .NET (Framework/Core) - Testing!";
		var categoryDto = new CategoryDto { CategoryName = specialName };

		// Act
		var result = await handler.HandleAsync(categoryDto);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();

		// Verify the category was created with special characters intact
		await VerifyCategoryExistsInDatabaseAsync(specialName);
	}

	/// <summary>
	/// Verifies that a category with the specified name exists in the database.
	/// </summary>
	private async Task VerifyCategoryExistsInDatabaseAsync(string categoryName)
	{
		using var context = _contextFactory.CreateDbContext();
		var categoryExists = await context.Categories.AnyAsync(c => c.CategoryName == categoryName, TestContext.Current.CancellationToken);
		categoryExists.Should().BeTrue($"Category '{categoryName}' should exist in the database");
	}

	/// <summary>
	/// Verifies that no categories exist in the database.
	/// </summary>
	private async Task VerifyNoCategoriesInDatabaseAsync()
	{
		using var context = _contextFactory.CreateDbContext();
		var categoryCount = await context.Categories.CountAsync(TestContext.Current.CancellationToken);
		categoryCount.Should().Be(0, "No categories should exist in the database");
	}
}

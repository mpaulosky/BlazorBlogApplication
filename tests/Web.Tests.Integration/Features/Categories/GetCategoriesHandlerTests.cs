// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategoriesHandlerTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests (migrated from Web.Tests.Integration)
// =======================================================
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Categories.CategoriesList;

namespace Web.Features.Categories;

/// <summary>
/// Integration tests for the GetCategories.Handler class.
/// Tests the complete workflow of retrieving categories from the database with filtering.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetCategories.Handler))]
[Collection("Test Collection")]
public class GetCategoriesHandlerTests
{
	private readonly WebTestFactory _factory;
	private readonly ILogger<GetCategories.Handler> _logger;
	private readonly IApplicationDbContextFactory _contextFactory;

	public GetCategoriesHandlerTests(WebTestFactory factory)
	{
		_factory = factory;
		using var scope = _factory.Services.CreateScope();
		_logger = scope.ServiceProvider.GetRequiredService<ILogger<GetCategories.Handler>>();
		_contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
	}

	[Fact]
	public async Task HandleAsync_WithMultipleCategories_ReturnsAllCategories()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		await CreateMultipleTestCategoriesAsync();
		var handler = new GetCategories.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(excludeArchived: false);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();
		result.Value.Should().NotBeNull();
		result.Value.Should().HaveCount(5); // We created 5 categories

		var categories = result.Value.ToList();
		categories.Should().Contain(c => c.CategoryName == "Technology");
		categories.Should().Contain(c => c.CategoryName == "Programming");
		categories.Should().Contain(c => c.CategoryName == "Sports");
		categories.Should().Contain(c => c.CategoryName == "Archived Category");
		categories.Should().Contain(c => c.CategoryName == "Business");

		// Verify one archived category is included when excludeArchived=false
		categories.Should().Contain(c => c.CategoryName == "Archived Category" && c.IsArchived);
	}

	[Fact]
	public async Task HandleAsync_WithExcludeArchived_ReturnsOnlyNonArchivedCategories()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		await CreateMultipleTestCategoriesAsync();
		var handler = new GetCategories.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(excludeArchived: true);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();
		result.Value.Should().NotBeNull();
		result.Value.Should().HaveCount(4); // 5 total - 1 archived = 4

		var categories = result.Value.ToList();
		categories.Should().Contain(c => c.CategoryName == "Technology");
		categories.Should().Contain(c => c.CategoryName == "Programming");
		categories.Should().Contain(c => c.CategoryName == "Sports");
		categories.Should().Contain(c => c.CategoryName == "Business");

		// Verify archived categories are excluded
		categories.Should().NotContain(c => c.IsArchived);
		categories.Should().NotContain(c => c.CategoryName == "Archived Category");
	}

	[Fact]
	public async Task HandleAsync_WithEmptyDatabase_ReturnsFailureResult()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		var handler = new GetCategories.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync();

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
		result.Error.Should().Be("No categories found.");
		result.Value.Should().BeNull();
	}

	[Fact]
	public async Task HandleAsync_WithOnlyArchivedCategories_AndExcludeArchived_ReturnsFailureResult()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		await CreateOnlyArchivedCategoriesAsync();
		var handler = new GetCategories.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(excludeArchived: true);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().NotBeNullOrEmpty();
		result.Error.Should().Be("No categories found.");
		result.Value.Should().BeNull();
	}

	[Fact]
	public async Task HandleAsync_WithOnlyArchivedCategories_AndIncludeArchived_ReturnsArchivedCategories()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		await CreateOnlyArchivedCategoriesAsync();
		var handler = new GetCategories.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync(excludeArchived: false);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Error.Should().BeNullOrEmpty();
		result.Value.Should().NotBeNull();
		result.Value.Should().HaveCount(3); // All 3 archived categories

		var categories = result.Value.ToList();
		categories.Should().OnlyContain(c => c.IsArchived);
		categories.Should().Contain(c => c.CategoryName == "Archived Tech");
		categories.Should().Contain(c => c.CategoryName == "Archived Sports");
		categories.Should().Contain(c => c.CategoryName == "Archived Business");
	}

	[Fact]
	public async Task HandleAsync_ReturnsProperlyMappedCategoryDtos()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		await CreateSingleTestCategoryAsync();
		var handler = new GetCategories.Handler(_contextFactory, _logger);

		// Act
		var result = await handler.HandleAsync();

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().HaveCount(1);

		var category = result.Value.First();
		category.Should().NotBeNull();
		category.Id.Should().NotBe(Guid.Empty);
		category.CategoryName.Should().Be("Test Single Category");
		category.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
		category.ModifiedOn.Should().BeNull();
		category.IsArchived.Should().BeFalse();
	}

	[Fact]
	public async Task HandleAsync_WithDatabaseContextDisposal_HandlesResourcesCorrectly()
	{
		// Arrange  
		await _factory.ResetDatabaseAsync();
		await CreateSingleTestCategoryAsync();
		var handler = new GetCategories.Handler(_contextFactory, _logger);

		// Act & Assert - Multiple calls should not cause disposal issues
		var result1 = await handler.HandleAsync();
		var result2 = await handler.HandleAsync();
		var result3 = await handler.HandleAsync();

		result1.Success.Should().BeTrue();
		result2.Success.Should().BeTrue();
		result3.Success.Should().BeTrue();

		result1.Value.Should().HaveCount(1);
		result2.Value.Should().HaveCount(1);
		result3.Value.Should().HaveCount(1);
	}

	/// <summary>
	/// Creates multiple test categories with a mix of archived and non-archived.
	/// </summary>
	private async Task CreateMultipleTestCategoriesAsync()
	{
		using var context = _contextFactory.CreateDbContext();

		var categories = new List<Category>
		{
			new("Technology"),
			new("Programming"), 
			new("Sports"),
			new("Archived Category", isArchived: true),
			new("Business")
		};

		context.Categories.AddRange(categories);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
	}

	/// <summary>
	/// Creates only archived categories for testing exclusion logic.
	/// </summary>
	private async Task CreateOnlyArchivedCategoriesAsync()
	{
		using var context = _contextFactory.CreateDbContext();

		var categories = new List<Category>
		{
			new("Archived Tech", isArchived: true),
			new("Archived Sports", isArchived: true), 
			new("Archived Business", isArchived: true)
		};

		context.Categories.AddRange(categories);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
	}

	/// <summary>
	/// Creates a single test category for basic validation.
	/// </summary>
	private async Task CreateSingleTestCategoryAsync()
	{
		using var context = _contextFactory.CreateDbContext();

		var category = new Category("Test Single Category");
		context.Categories.Add(category);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
	}
}

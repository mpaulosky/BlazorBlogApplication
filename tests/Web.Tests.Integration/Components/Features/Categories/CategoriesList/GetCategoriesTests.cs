// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategoriesTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================

namespace Web.Components.Features.Categories.CategoriesList;

[ExcludeFromCodeCoverage]
[Collection("Test Collection")]
public class GetCategoriesTests : IAsyncLifetime
{

	private const string CLEANUP_VALUE = "Categories";

	private readonly WebTestFactory _factory;

	private readonly IServiceScope _scope;

	private readonly GetCategories.IGetCategoriesHandler _sut;

	public GetCategoriesTests(WebTestFactory factory)
	{
		_factory = factory;

		// Create a scope here so scoped services (like IMyBlogContextFactory) are resolved correctly.
		_scope = _factory.Services.CreateScope();
		_sut = _scope.ServiceProvider.GetRequiredService<GetCategories.IGetCategoriesHandler>();
	}

	public ValueTask InitializeAsync()
	{
		return ValueTask.CompletedTask;
	}

	public async ValueTask DisposeAsync()
	{
		try
		{
			await _factory.ResetCollectionAsync(CLEANUP_VALUE);
		}
		finally
		{
			_scope.Dispose();
		}
	}

	[Fact(DisplayName = "HandleAsync Should Return All Categories When No Filter Applied")]
	public async Task HandleAsync_Should_ReturnAllCategories_When_NoFilterApplied_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var testId = Guid.NewGuid().ToString("N").Substring(0, 8);

		var categories = new List<Category>
		{
			new() { CategoryName = $"Tech {testId}", Archived = false },
			new() { CategoryName = $"Science {testId}", Archived = false },
			new() { CategoryName = $"History {testId}", Archived = true }
		};

		await ctx.Categories.InsertManyAsync(categories, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(excludeArchived: false);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		var categoriesWithTestId = result.Value.Where(c => c.CategoryName.Contains(testId)).ToList();
		categoriesWithTestId.Should().HaveCount(3);

		var categoryNames = categoriesWithTestId.Select(c => c.CategoryName).ToList();
		categoryNames.Should().Contain($"Tech {testId}");
		categoryNames.Should().Contain($"Science {testId}");
		categoryNames.Should().Contain($"History {testId}");
	}

	[Fact(DisplayName = "HandleAsync Should Exclude Archived Categories When Filter Applied")]
	public async Task HandleAsync_Should_ExcludeArchivedCategories_When_FilterApplied_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var testId = Guid.NewGuid().ToString("N").Substring(0, 8);

		var categories = new List<Category>
		{
			new() { CategoryName = $"Tech {testId}", Archived = false },
			new() { CategoryName = $"Science {testId}", Archived = false },
			new() { CategoryName = $"History {testId}", Archived = true }
		};

		await ctx.Categories.InsertManyAsync(categories, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(excludeArchived: true);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		var categoriesWithTestId = result.Value.Where(c => c.CategoryName.Contains(testId)).ToList();
		categoriesWithTestId.Should().HaveCount(2);

		var categoryNames = categoriesWithTestId.Select(c => c.CategoryName).ToList();
		categoryNames.Should().Contain($"Tech {testId}");
		categoryNames.Should().Contain($"Science {testId}");
		categoryNames.Should().NotContain($"History {testId}");
	}

	[Fact(DisplayName = "HandleAsync Should Return Failure When No Categories Found")]
	public async Task HandleAsync_Should_ReturnFailure_When_NoCategoriesFound_TestAsync()
	{
		// Arrange - Create a completely isolated test by using a unique collection name
		var testId = Guid.NewGuid().ToString("N").Substring(0, 8);
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(TestContext.Current.CancellationToken);

		// Delete all categories to ensure empty state
		await ctx.Categories.DeleteManyAsync(Builders<Category>.Filter.Empty, cancellationToken: TestContext.Current.CancellationToken);

		// Verify database is empty
		var count = await ctx.Categories.CountDocumentsAsync(Builders<Category>.Filter.Empty, cancellationToken: TestContext.Current.CancellationToken);
		count.Should().Be(0);

		// Act
		var result = await _sut.HandleAsync();

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("No categories found.");
	}

	[Fact(DisplayName = "HandleAsync Should Return Failure When Only Archived Categories Exist And Filter Applied")]
	public async Task HandleAsync_Should_ReturnFailure_When_OnlyArchivedCategoriesExistAndFilterApplied_TestAsync()
	{
		// Arrange - Create a completely isolated test by using a unique collection name
		var testId = Guid.NewGuid().ToString("N").Substring(0, 8);
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(TestContext.Current.CancellationToken);

		// Clear all existing categories to ensure clean state
		await ctx.Categories.DeleteManyAsync(Builders<Category>.Filter.Empty, cancellationToken: TestContext.Current.CancellationToken);

		var categories = new List<Category>
		{
			new() { CategoryName = $"History {testId}", Archived = true },
			new() { CategoryName = $"Old News {testId}", Archived = true }
		};

		await ctx.Categories.InsertManyAsync(categories, cancellationToken: TestContext.Current.CancellationToken);

		// Act
		var result = await _sut.HandleAsync(excludeArchived: true);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("No categories found.");
	}

	[Fact(DisplayName = "HandleAsync Should Handle Large Number Of Categories")]
	public async Task HandleAsync_Should_HandleLargeNumberOfCategories_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var testId = Guid.NewGuid().ToString("N").Substring(0, 8);

		var categories = new List<Category>();
		for (int i = 1; i <= 1000; i++)
		{
			categories.Add(new Category
			{
				CategoryName = $"Category{i}_{testId}",
				Archived = i % 10 == 0 // Every 10th category is archived
			});
		}

		await ctx.Categories.InsertManyAsync(categories, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(excludeArchived: false);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		var categoriesWithTestId = result.Value.Where(c => c.CategoryName.Contains(testId)).ToList();
		categoriesWithTestId.Should().HaveCount(1000);

		// Test with archived filter
		var resultFiltered = await _sut.HandleAsync(excludeArchived: true);
		resultFiltered.Should().NotBeNull();
		resultFiltered.Success.Should().BeTrue();
		resultFiltered.Value.Should().NotBeNull();
		var filteredCategoriesWithTestId = resultFiltered.Value.Where(c => c.CategoryName.Contains(testId)).ToList();
		filteredCategoriesWithTestId.Should().HaveCount(900); // 100 archived categories excluded
	}

	[Fact(DisplayName = "HandleAsync Should Return Categories With Correct Data Mapping")]
	public async Task HandleAsync_Should_ReturnCategoriesWithCorrectDataMapping_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var testId = Guid.NewGuid().ToString("N").Substring(0, 8);

		var testDate = DateTime.UtcNow.AddDays(-5);
		var categories = new List<Category>
		{
			new()
			{
				CategoryName = $"Test Category {testId}",
				Archived = false
			}
		};

		await ctx.Categories.InsertManyAsync(categories, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync();

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		var categoriesWithTestId = result.Value.Where(c => c.CategoryName.Contains(testId)).ToList();
		categoriesWithTestId.Should().HaveCount(1);

		var category = categoriesWithTestId.First();
		category.CategoryName.Should().Be($"Test Category {testId}");
		category.Id.Should().NotBe(ObjectId.Empty);
	}

	[Fact(DisplayName = "HandleAsync Should Handle Categories With Special Characters")]
	public async Task HandleAsync_Should_HandleCategoriesWithSpecialCharacters_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var testId = Guid.NewGuid().ToString("N").Substring(0, 8);

		var categories = new List<Category>
		{
			new() { CategoryName = $"Test & Category {testId}", Archived = false },
			new() { CategoryName = $"Science & Technology {testId}", Archived = false },
			new() { CategoryName = $"History/Politics {testId}", Archived = false }
		};

		await ctx.Categories.InsertManyAsync(categories, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync();

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		var categoriesWithTestId = result.Value.Where(c => c.CategoryName.Contains(testId)).ToList();
		categoriesWithTestId.Should().HaveCount(3);

		var categoryNames = categoriesWithTestId.Select(c => c.CategoryName).ToList();
		categoryNames.Should().Contain($"Test & Category {testId}");
		categoryNames.Should().Contain($"Science & Technology {testId}");
		categoryNames.Should().Contain($"History/Politics {testId}");
	}

	[Fact(DisplayName = "HandleAsync Should Handle Unicode Category Names")]
	public async Task HandleAsync_Should_HandleUnicodeCategoryNames_TestAsync()
	{
		// Arrange
		await _factory.ResetDatabaseAsync();
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var testId = Guid.NewGuid().ToString("N").Substring(0, 8);

		var categories = new List<Category>
		{
			new() { CategoryName = $"测试 {testId}", Archived = false },
			new() { CategoryName = $"Español {testId}", Archived = false },
			new() { CategoryName = $"Français {testId}", Archived = false }
		};

		await ctx.Categories.InsertManyAsync(categories, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync();

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		var categoriesWithTestId = result.Value.Where(c => c.CategoryName.Contains(testId)).ToList();
		categoriesWithTestId.Should().HaveCount(3);

		var categoryNames = categoriesWithTestId.Select(c => c.CategoryName).ToList();
		categoryNames.Should().Contain($"测试 {testId}");
		categoryNames.Should().Contain($"Español {testId}");
		categoryNames.Should().Contain($"Français {testId}");
	}

}

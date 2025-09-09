// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategoryTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================

namespace Web.Components.Features.Categories.CategoryDetails;

[ExcludeFromCodeCoverage]
[Collection("Test Collection")]
public class GetCategoryTests : IAsyncLifetime
{

	private const string CLEANUP_VALUE = "Categories";

	private readonly WebTestFactory _factory;

	private readonly IServiceScope _scope;

	private readonly GetCategory.IGetCategoryHandler _sut;

	public GetCategoryTests(WebTestFactory factory)
	{
		_factory = factory;

		// Create a scope here so scoped services (like IMyBlogContextFactory) are resolved correctly.
		_scope = _factory.Services.CreateScope();
		_sut = _scope.ServiceProvider.GetRequiredService<GetCategory.IGetCategoryHandler>();
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

	[Fact(DisplayName = "HandleAsync Should Return Category When Valid Id Provided")]
	public async Task HandleAsync_Should_ReturnCategory_When_ValidIdProvided_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var request = FakeCategory.GetNewCategory(true);
		await ctx.Categories.InsertOneAsync(request, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(request.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Id.Should().Be(request.Id);
		result.Value.CategoryName.Should().Be(request.CategoryName);
		result.Value.IsArchived.Should().Be(request.Archived);
		result.Value.CreatedOn.Should().BeCloseTo(request.CreatedOn, TimeSpan.FromHours(7));

	}

	[Fact(DisplayName = "HandleAsync Should Return Failure When Empty Id Provided")]
	public async Task HandleAsync_Should_ReturnFailure_When_EmptyIdProvided_TestAsync()
	{
		// Arrange & Act
		var result = await _sut.HandleAsync(ObjectId.Empty);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("The ID cannot be empty.");
	}

	[Fact(DisplayName = "HandleAsync Should Return Failure When Category Not Found")]
	public async Task HandleAsync_Should_ReturnFailure_When_CategoryNotFound_TestAsync()
	{
		// Arrange
		var nonExistentId = ObjectId.GenerateNewId();

		// Act
		var result = await _sut.HandleAsync(nonExistentId);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("Category not found.");
	}

	[Fact(DisplayName = "HandleAsync Should Return Correct Data Mapping")]
	public async Task HandleAsync_Should_ReturnCorrectDataMapping_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var testCategory = FakeCategory.GetNewCategory(true);

		await ctx.Categories.InsertOneAsync(testCategory, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(testCategory.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Id.Should().Be(testCategory.Id);
		result.Value.CategoryName.Should().Be(testCategory.CategoryName);

	}

	[Fact(DisplayName = "HandleAsync Should Handle Archived Categories")]
	public async Task HandleAsync_Should_HandleArchivedCategories_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create required dependencies
		var request = FakeCategory.GetNewCategory(true);
		request.Archived = true;

		await ctx.Categories.InsertOneAsync(request, cancellationToken: CancellationToken.None);
		var result = await _sut.HandleAsync(request.Id);

		// Assert - Should still return archived categories
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Id.Should().Be(request.Id);
		result.Value.CategoryName.Should().Be(request.CategoryName);
		result.Value.IsArchived.Should().Be(request.Archived);

	}

	[Fact(DisplayName = "HandleAsync Should Handle Categories With Special Characters")]
	public async Task HandleAsync_Should_HandleCategoriesWithSpecialCharacters_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		// Create a category with special characters
		var request = FakeCategory.GetNewCategory(true);
		request.CategoryName = "Special & Category #123 @ Test!";
		request.IsArchived = false;

		await ctx.Categories.InsertOneAsync(request, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(request.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.CategoryName.Should().Be(request.CategoryName);
		result.Value.Id.Should().Be(request.Id);
		result.Value.IsArchived.Should().Be(request.IsArchived);

	}

	[Fact(DisplayName = "HandleAsync Should Handle Unicode Category Names")]
	public async Task HandleAsync_Should_HandleUnicodeCategoryNames_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var unicodeCategory = new Category
		{
			CategoryName = "测试 Category 中文 Español",
			Archived = false
		};

		await ctx.Categories.InsertOneAsync(unicodeCategory, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(unicodeCategory.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.CategoryName.Should().Be("测试 Category 中文 Español");
	}

	[Fact(DisplayName = "HandleAsync Should Handle Very Long Category Names")]
	public async Task HandleAsync_Should_HandleVeryLongCategoryNames_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var longName = new string('A', 1000);
		var longCategory = new Category
		{
			CategoryName = longName,
			Archived = false
		};

		await ctx.Categories.InsertOneAsync(longCategory, cancellationToken: CancellationToken.None);

		// Act
		var result = await _sut.HandleAsync(longCategory.Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.CategoryName.Should().Be(longName);
	}

	[Fact(DisplayName = "HandleAsync Should Return First Category When Multiple Categories Exist")]
	public async Task HandleAsync_Should_ReturnFirstCategory_When_MultipleCategoriesExist_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var categories = new List<Category>
		{
			new() { CategoryName = "First Category", Archived = false },
			new() { CategoryName = "Second Category", Archived = false }
		};

		await ctx.Categories.InsertManyAsync(categories, cancellationToken: CancellationToken.None);

		// Act - Get the first category
		var result = await _sut.HandleAsync(categories[0].Id);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		result.Value.Id.Should().Be(categories[0].Id);
		result.Value.CategoryName.Should().Be("First Category");
	}

}

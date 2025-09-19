// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditCategoryTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================

namespace Web.Components.Features.Categories.CategoryEdit;

[ExcludeFromCodeCoverage]
[Collection("Test Collection")]
public class EditCategoryTests : IAsyncLifetime
{

	private const string CLEANUP_VALUE = "Categories";

	private readonly WebTestFactory _factory;

	private readonly IServiceScope _scope;

	private readonly EditCategory.IEditCategoryHandler _sut;

	public EditCategoryTests(WebTestFactory factory)
	{
		_factory = factory;

		// Create a scope here so scoped services (like IArticleDbContextFactory) are resolved correctly.
		_scope = _factory.Services.CreateScope();
		_sut = _scope.ServiceProvider.GetRequiredService<EditCategory.IEditCategoryHandler>();
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

	[Fact(DisplayName = "HandleAsync Category With Valid Data Should Succeed")]
	public async Task HandleAsync_With_ValidData_Should_UpdateCategory_TestAsync()
	{
		// Arrange
		await _factory.ResetCollectionAsync(CLEANUP_VALUE);
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create an existing category to edit
		var existingCategory = new Category
		{
			CategoryName = "Original Category",
			IsArchived = false
		};

		await ctx.Categories.InsertOneAsync(existingCategory, cancellationToken: CancellationToken.None);

		var updateRequest = new CategoryDto
		{
			Id = existingCategory.Id,
			CategoryName = "Updated Category"
		};

		// Act
		var result = await _sut.HandleAsync(updateRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the document was updated in MongoDB
		var updated = await ctx.Categories.Find(c => c.Id == existingCategory.Id)
			.FirstOrDefaultAsync(CancellationToken.None);

		updated.Should().NotBeNull();
		updated.CategoryName.Should().Be("Updated Category");
		updated.ModifiedOn.Should().NotBeNull();
	}

	[Fact(DisplayName = "HandleAsync Category With Null Data Should Fail")]
	public async Task HandleAsync_With_NullData_Should_ReturnFailure_TestAsync()
	{
		// Arrange & Act
		var result = await _sut.HandleAsync(null);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("The request is null.");
	}

	[Fact(DisplayName = "HandleAsync Category With Empty Name Should Fail")]
	public async Task HandleAsync_With_EmptyName_Should_ReturnFailure_TestAsync()
	{
		// Arrange
		var request = new CategoryDto
		{
			Id = ObjectId.GenerateNewId(),
			CategoryName = ""
		};

		// Act
		var result = await _sut.HandleAsync(request);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("Category name cannot be empty or whitespace.");
	}

	[Fact(DisplayName = "HandleAsync Category With Whitespace Name Should Fail")]
	public async Task HandleAsync_With_WhitespaceName_Should_ReturnFailure_TestAsync()
	{
		// Arrange
		var request = new CategoryDto
		{
			Id = ObjectId.GenerateNewId(),
			CategoryName = "   "
		};

		// Act
		var result = await _sut.HandleAsync(request);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("Category name cannot be empty or whitespace.");
	}

	[Fact(DisplayName = "HandleAsync Category With Empty ObjectId Should Fail")]
	public async Task HandleAsync_With_EmptyObjectId_Should_ReturnFailure_TestAsync()
	{
		// Arrange
		var request = new CategoryDto
		{
			Id = Guid.Empty,
			CategoryName = "Valid Category"
		};

		// Act
		var result = await _sut.HandleAsync(request);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("Category ID cannot be empty.");
	}

	[Fact(DisplayName = "HandleAsync Category With Special Characters Should Succeed")]
	public async Task HandleAsync_With_SpecialCharacters_Should_Succeed_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create an existing category to edit
		var existingCategory = new Category
		{
			CategoryName = "Original Category"
		};

		await ctx.Categories.InsertOneAsync(existingCategory, cancellationToken: CancellationToken.None);

		var updateRequest = new CategoryDto
		{
			Id = existingCategory.Id,
			CategoryName = "Test & Category #123 @ Special!"
		};

		// Act
		var result = await _sut.HandleAsync(updateRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the document was updated in MongoDB
		var updated = await ctx.Categories.Find(c => c.Id == existingCategory.Id)
			.FirstOrDefaultAsync(CancellationToken.None);

		updated.Should().NotBeNull();
		updated.CategoryName.Should().Be("Test & Category #123 @ Special!");
	}

	[Fact(DisplayName = "HandleAsync Category With Very Long Name Should Succeed")]
	public async Task HandleAsync_With_VeryLongName_Should_Succeed_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create an existing category to edit
		var existingCategory = new Category
		{
			CategoryName = "Original Category"
		};

		await ctx.Categories.InsertOneAsync(existingCategory, cancellationToken: CancellationToken.None);

		var longName = new string('A', 500); // Very long category name
		var updateRequest = new CategoryDto
		{
			Id = existingCategory.Id,
			CategoryName = longName
		};

		// Act
		var result = await _sut.HandleAsync(updateRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the document was updated in MongoDB
		var updated = await ctx.Categories.Find(c => c.Id == existingCategory.Id)
			.FirstOrDefaultAsync(CancellationToken.None);

		updated.Should().NotBeNull();
		updated.CategoryName.Should().Be(longName);
	}

	[Fact(DisplayName = "HandleAsync Category With Nonexistent Id Should Fail")]
	public async Task HandleAsync_With_NonexistentId_Should_Fail_TestAsync()
	{
		// Arrange
		var updateRequest = new CategoryDto
		{
			Id = ObjectId.GenerateNewId(), // Non-existent ID
			CategoryName = "Updated Category"
		};

		// Act
		var result = await _sut.HandleAsync(updateRequest);

		// Assert - Handler should return failure when no document is found
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();
		result.Error.Should().Be("Category not found.");
	}

	[Fact(DisplayName = "HandleAsync Category With Unicode Characters Should Succeed")]
	public async Task HandleAsync_With_UnicodeCharacters_Should_Succeed_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		// Create an existing category to edit
		var existingCategory = new Category
		{
			CategoryName = "Original Category"
		};

		await ctx.Categories.InsertOneAsync(existingCategory, cancellationToken: CancellationToken.None);

		var updateRequest = new CategoryDto
		{
			Id = existingCategory.Id,
			CategoryName = "测试 Category ño ños 中文"
		};

		// Act
		var result = await _sut.HandleAsync(updateRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the document was updated in MongoDB
		var updated = await ctx.Categories.Find(c => c.Id == existingCategory.Id)
			.FirstOrDefaultAsync(CancellationToken.None);

		updated.Should().NotBeNull();
		updated.CategoryName.Should().Be("测试 Category ño ños 中文");
	}

	[Fact(DisplayName = "HandleAsync Category Should Update ModifiedOn Timestamp")]
	public async Task HandleAsync_Should_UpdateModifiedOnTimestamp_TestAsync()
	{
		// Arrange
		var ctxFactory = _factory.Services.GetRequiredService<IArticleDbContextFactory>();
		var ctx = await ctxFactory.CreateDbContext(CancellationToken.None);

		var existingCategory = new Category
		{
			CategoryName = "Original Category"
		};

		await ctx.Categories.InsertOneAsync(existingCategory, cancellationToken: CancellationToken.None);

		// Get the original timestamp after insertion
		var originalDoc = await ctx.Categories.Find(c => c.Id == existingCategory.Id)
			.FirstOrDefaultAsync(CancellationToken.None);
		var originalTime = originalDoc!.ModifiedOn ?? originalDoc.CreatedOn;

		// Wait a bit to ensure timestamp difference
		await Task.Delay(10, TestContext.Current.CancellationToken);

		var updateRequest = new CategoryDto
		{
			Id = existingCategory.Id,
			CategoryName = "Updated Category"
		};

		// Act
		var result = await _sut.HandleAsync(updateRequest);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the ModifiedOn timestamp was updated
		var updated = await ctx.Categories.Find(c => c.Id == existingCategory.Id)
			.FirstOrDefaultAsync(CancellationToken.None);

		updated.Should().NotBeNull();
		updated.ModifiedOn.Should().NotBeNull();
		updated.ModifiedOn!.Value.Should().BeAfter(originalTime);
	}

}

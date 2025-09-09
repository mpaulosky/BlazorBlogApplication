// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateCategoryTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================

namespace Web.Components.Features.Categories.CategoryCreate;

[ExcludeFromCodeCoverage]
[Collection("Test Collection")]
public class CreateCategoryTests : IAsyncLifetime
{

	private const string CLEANUP_VALUE = "Categories";

	private readonly WebTestFactory _factory;

	private readonly IServiceScope _scope;

	private readonly CreateCategory.ICreateCategoryHandler _sut;

	public CreateCategoryTests(WebTestFactory factory)
	{
		_factory = factory;

		// Create a scope here so scoped services (like IMyBlogContextFactory) are resolved correctly.
		_scope = _factory.Services.CreateScope();
		_sut = _scope.ServiceProvider.GetRequiredService<CreateCategory.ICreateCategoryHandler>();
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
	public async Task HandleAsync_With_ValidData_Should_CreateACategory_TestAsync()
	{

		// Arrange
		var expected = FakeCategoryDto.GetNewCategoryDto();

		// Act
		var result = await _sut.HandleAsync(expected);

		// Assert
		result.Should().NotBeNull();
		result.Success.Should().BeTrue();

		// Verify the document was inserted into MongoDB
		var ctxFactory = _factory.Services.GetRequiredService<IMyBlogContextFactory>();
		var ctx = await ctxFactory.CreateContext(CancellationToken.None);

		var inserted = await ctx.Categories.Find(c => c.CategoryName == expected.CategoryName)
				.FirstOrDefaultAsync(CancellationToken.None);

		inserted.Should().NotBeNull();
		inserted.Id.Should().NotBe(ObjectId.Empty);

	}

	[Fact(DisplayName = "HandleAsync Category With Null Data Should Fail")]
	public async Task HandleAsync_With_NullData_Should_ReturnFailure_TestAsync()
	{

		// Arrange & Act
		var result = await _sut.HandleAsync(null);

		// Assert - handler catches exceptions and returns a failing Result
		result.Should().NotBeNull();
		result.Success.Should().BeFalse();

	}

}

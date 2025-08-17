// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ListTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  Web.Tests.Bunit
// =======================================================

using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Web.Components.Features.Categories.CategoryList;
using static Web.Components.Features.Categories.CategoryList.GetCategories;

namespace Web.Components.Features.Categories;

/// <summary>
///   Unit tests for <see cref="List{T}" />
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Web.Components.Features.Categories.CategoryList.List))]
public class ListTests : BunitContext
{
	private readonly IGetCategoriesHandler _mockHandler;

	public ListTests()
	{
		_mockHandler = Substitute.For<IGetCategoriesHandler>();
		Services.AddScoped(_ => _mockHandler);
		Services.AddScoped<ILogger<Web.Components.Features.Categories.CategoryList.List>>(_ => Substitute.For<ILogger<Web.Components.Features.Categories.CategoryList.List>>());
		Services.AddCascadingAuthenticationState();
		Services.AddAuthorization();
	}

	private void SetupHandlerCategories(IEnumerable<CategoryDto>? categories, bool success = true, string? error = null)
	{
		if (success)
			_mockHandler.HandleAsync(Arg.Any<bool>()).Returns(Task.FromResult(Result<IEnumerable<CategoryDto>>.Ok(categories ?? new List<CategoryDto>())));
		else
			_mockHandler.HandleAsync(Arg.Any<bool>()).Returns(Task.FromResult(Result<IEnumerable<CategoryDto>>.Fail(error ?? "Error")));
	}

	[Fact]
	public void RendersNoCategories_WhenCategoriesIsNullOrEmptyAndResultIsOk()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerCategories(null);

		// Act
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();

		// Assert
		cut.Markup.Should().Contain("No categories available.");
		cut.Markup.Should().Contain("Create New Category");

	}

	[Fact]
	public void RendersNoCategories_WhenCategoriesIsNullOrEmpty()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerCategories(null);

		// Act
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();

		// Assert
		cut.Markup.Should().Contain("No categories available");
		cut.Markup.Should().Contain("Create New Category");
		cut.Markup.Should().Contain("Welcome Test User!");
	}

	[Fact]
	public void RendersCategories_WhenCategoriesArePresent()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var categoriesDto = new List<CategoryDto>
		{
			new CategoryDto { CategoryName = "Cat1", CreatedOn = DateTimeOffset.Now, ModifiedOn = DateTimeOffset.Now, Archived = false },
			new CategoryDto { CategoryName = "Cat2", CreatedOn = DateTimeOffset.Now, ModifiedOn = null, Archived = true }
		};
		SetupHandlerCategories(categoriesDto);

		// Act
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();

		// Assert
		cut.Markup.Should().Contain("Cat1");
		cut.Markup.Should().Contain("Cat2");
	}

	[Fact]
	public void Renders_LoadingComponent_When_IsLoading()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerCategories(null);
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();
		cut.Instance.GetType().GetField("_isLoading", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, true);
		cut.Render();
		// loading component renders a spinner and 'Loading...' text
		cut.Markup.Should().Contain("Loading...");
	}

	[Fact]
	public void Renders_Error_State_When_Handler_Fails()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerCategories(null, success: false, error: "Failed to load categories");
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();
		cut.Instance.GetType().GetField("_isLoading", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Render();
		// Error state shows the failure message via logger/error text
		cut.Markup.Should().Contain("No categories available")
			.And.Contain("Create New Category");
		// The component logs errors but shows friendly message; ensure test checks for logged error indirectly by ensuring no rows are present
	}

	[Fact]
	public void Navigates_To_Create_New_Category()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var nav = Services.GetRequiredService<BunitNavigationManager>();
		SetupHandlerCategories(new List<CategoryDto>());
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();
		cut.Instance.GetType().GetField("_isLoading", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Render();
		cut.Find("button.btn-success").Click();
		nav.Uri.Should().EndWith("/categories/create");
	}

	[Fact]
	public void Navigates_To_Edit_Category()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var nav = Services.GetRequiredService<BunitNavigationManager>();
		var categoriesDto = new List<CategoryDto>
		{
			new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "Cat1", CreatedOn = DateTimeOffset.Now, ModifiedOn = DateTimeOffset.Now, Archived = false }
		};
		SetupHandlerCategories(categoriesDto);
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();
		cut.Instance.GetType().GetField("_isLoading", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_categories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, categoriesDto.AsQueryable());
		cut.Render();
		cut.Find("button.btn-primary").Click();
		// the component navigates to segment-style edit route
		nav.Uri.Should().Contain("/categories/edit/");
	}

	[Fact]
	public void Navigates_To_Details_Category()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var nav = Services.GetRequiredService<BunitNavigationManager>();
		var categoriesDto = new List<CategoryDto>
		{
			new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "Cat1", CreatedOn = DateTimeOffset.Now, ModifiedOn = DateTimeOffset.Now, Archived = false }
		};
		SetupHandlerCategories(categoriesDto);
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();
		cut.Instance.GetType().GetField("_isLoading", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_categories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, categoriesDto.AsQueryable());
		cut.Render();
		cut.Find("button.btn-info").Click();
		// the component navigates to segment-style details route
		nav.Uri.Should().Contain("/categories/");
	}

	[Fact]
	public void Renders_All_Table_Columns()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var categoriesDto = new List<CategoryDto>
		{
			new CategoryDto { CategoryName = "Cat1", CreatedOn = DateTimeOffset.Now, ModifiedOn = DateTimeOffset.Now, Archived = false }
		};
		SetupHandlerCategories(categoriesDto);
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();
		cut.Instance.GetType().GetField("_isLoading", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_categories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, categoriesDto.AsQueryable());
		cut.Render();
		var expectedHeaders = new[] { "Category Name", "Created On", "Modified On", "Archived", "Actions" };
		foreach (var header in expectedHeaders)
		{
			cut.Markup.Should().Contain(header);
		}
	}

	[Fact(Skip = "Bunit does not enforce [Authorize]; page is always rendered in test context.")]
	public void Only_Authorized_Users_Can_Access()
	{
		// Arrange
		Helpers.SetAuthorization(this, false);
		var cut = Render<Web.Components.Features.Categories.CategoryList.List>();
		// Assert
		cut.FindAll("table").Should().BeEmpty();
	}

}
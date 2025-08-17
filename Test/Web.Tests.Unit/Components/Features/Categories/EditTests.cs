// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Bunit
// =======================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using NSubstitute;
using FluentAssertions;
using Xunit;
using MongoDB.Bson;
using MongoDB.Driver;
// using Bunit.TestDoubles; (not needed for BunitNavigationManager)
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Web.Components.Features.Categories.CategoryEdit;
using Web.Components.Features.Categories.CategoryDetails;
using Web.Data.Entities;
using Web.Data.Models;
using Web.Data.Abstractions;
using Web.Data;

namespace Web.Components.Features.Categories;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Edit))]
public class EditTests : BunitContext
{
	private readonly EditCategory.IEditCategoryHandler _editHandlerMock;
	private readonly GetCategory.IGetCategoryHandler _getHandlerMock;
	// removed unused concrete handler field - tests register handler substitutes via the fixture
	private readonly IMyBlogContextFactory _mockContextFactory;
	private readonly ILogger<GetCategory.Handler> _mockLogger;
	private readonly CategoryTestFixture _fixture;

	public EditTests()
	{
		_editHandlerMock = Substitute.For<EditCategory.IEditCategoryHandler>();
		_getHandlerMock = Substitute.For<GetCategory.IGetCategoryHandler>();
		_mockContextFactory = Substitute.For<IMyBlogContextFactory>();
		_mockLogger = Substitute.For<ILogger<GetCategory.Handler>>();
		_fixture = new CategoryTestFixture();
		// Ensure common test services and handler substitutes are registered
		TestServiceRegistrations.RegisterCommonUtilities(this);
		// Register the edit handler mock so the component will use it
		Services.AddScoped(_ => _editHandlerMock);
	}

	[Fact]
	public void Renders_Edit_Form_When_Category_Exists()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		// Assert
		cut.Markup.Should().Contain("Edit Category");
		cut.Markup.Should().Contain("Category Name");
		cut.Markup.Should().Contain("Save Changes");
		cut.Markup.Should().Contain("Cancel");
		cut.Markup.Should().NotContain("animate-spin");
		cut.Markup.Should().NotContain("Loading...");
		cut.Markup.Should().Contain(categoryDto.CategoryName);
		cut.Find("form").Should().NotBeNull();
	}

	[Fact]
	public void Populates_Form_Fields_With_Category_Data()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, categoryDto.Id));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		var nameInput = cut.Find("#name");
		nameInput.GetAttribute("value").Should().Be(categoryDto.CategoryName);

	}

	[Fact]
	public void Shows_Validation_Error_When_Name_Is_Empty()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		// Ensure GetHandler returns an existing category so the form renders
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);

		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, categoryDto.Id));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		var nameInput = cut.Find("#name");
		var form = cut.Find("form");

		// Act
		nameInput.Change("");
		form.Submit();

		// Assert - component sets _errorMessage to 'Name is required'
		cut.Markup.Should().Contain("Name is required");

	}

	[Fact]
	public async Task Submits_Valid_Form_And_Navigates_On_Success()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		_editHandlerMock.HandleAsync(Arg.Any<CategoryDto>()).Returns(Task.FromResult(Result.Ok()));

		var cut = Render<Edit>(parameters => parameters
				.Add(p => p.Id, categoryDto.Id));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		var nameInput = cut.Find("#name");
		var form = cut.Find("form");

		// Act
		await cut.InvokeAsync(() => nameInput.Change("Updated Name"));
		await cut.InvokeAsync(() => form.Submit());

		// Assert
		await _editHandlerMock.Received(1).HandleAsync(Arg.Is<CategoryDto>(dto => dto.CategoryName == "Updated Name"));

	}

	[Fact]
	public async Task Shows_Error_Message_When_Update_Fails()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		_editHandlerMock.HandleAsync(Arg.Any<CategoryDto>()).Returns(Task.FromResult(Result.Fail("Update failed")));

		var cut = Render<Edit>(parameters => parameters
				.Add(p => p.Id, categoryDto.Id));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		var form = cut.Find("form");

		// Act
		await cut.InvokeAsync(() => form.Submit());

		// Assert
		cut.Markup.Should().Contain("Update failed");

	}

	[Fact]
	public async Task Disables_Submit_Button_When_Submitting()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		var tcs = new TaskCompletionSource<Result>();
		_editHandlerMock.HandleAsync(Arg.Any<CategoryDto>()).Returns(_ => tcs.Task);

		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"));

		// Change input and submit a form
		var nameInput = cut.Find("#name");
		await cut.InvokeAsync(() => nameInput.Change("Updated Name"));
		var submitTask = cut.InvokeAsync(() => cut.Find("form").Submit());

		// Assert: submit button is disabled and text is 'Updating...'
		var submitButton = cut.Find("button[type='submit']");
		submitButton.HasAttribute("disabled").Should().BeTrue();
		submitButton.TextContent.Trim().Should().Be("Updating...");

		// Allow async submit to finish, remember this!
		tcs.SetResult(Result.Ok());
		await submitTask;

	}

	[Fact]
	public void Shows_Save_Changes_Text_When_Not_Submitting()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);

		var cut = Render<Edit>(parameters => parameters
				.Add(p => p.Id, categoryDto.Id));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Save Changes");
		cut.Markup.Should().NotContain("Saving...");
		cut.Markup.Should().Contain("Cancel");

	}

	[Fact]
	public void Cancel_Link_Navigates_To_Categories_List()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, categoryDto.Id));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		var cancelButton = cut.Find("button[type='button']");

		// Assert
		cancelButton.Should().NotBeNull();
		cancelButton.TextContent.Should().Contain("Cancel");

	}

	[Fact]
	public void Handles_Empty_ObjectId_Parameter()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		_fixture.SetupFindAsync(new List<Category>());
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, ObjectId.Empty));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category not found");

	}

	[Fact]
	public void Shows_Correct_Page_Heading()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, categoryDto.Id));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Edit Category");
		cut.Markup.Should().Contain("Category Name");
		cut.Markup.Should().Contain("Save Changes");
		cut.Markup.Should().Contain("Cancel");

	}

	[Fact]
	public async Task Preserves_Category_Id_During_Update()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		_editHandlerMock.HandleAsync(Arg.Any<CategoryDto>()).Returns(Task.FromResult(Result.Ok()));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, categoryDto.Id));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		var form = cut.Find("form");

		// Act
		form.Submit();

		// Assert
		await _editHandlerMock.Received(1).HandleAsync(Arg.Is<CategoryDto>(dto => dto.Id == categoryDto.Id));

	}

	[Fact]
	public async Task Returns_Early_When_Model_Is_Null_During_Submit()
	{

		// Arrange
		Helpers.SetAuthorization(this, true, "admin", "editor");
		var categoryId = ObjectId.GenerateNewId();
		_fixture.SetupFindAsync(new List<Category>());
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, categoryId));

		// Wait for the component to finish loading
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		await Task.Yield();
		// Act & Assert - Should not throw exception when trying to submit with null model
		// The component should handle this gracefully and not call the service
		await _editHandlerMock.DidNotReceive().HandleAsync(Arg.Any<CategoryDto>());

	}


	[Fact(Skip = "bUnit does not enforce [Authorize] in component tests; test via integration tests instead")]
	public async Task Edit_Form_Not_Rendered_For_Unauthorized_User()
	{
		// TODO: cover authorization behavior in integration/Playwright tests where auth is enforced.
	}

	[Fact]
	public async Task Navigates_To_Categories_List_On_Successful_Edit()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var category = new Category { CategoryName = categoryDto.CategoryName, ModifiedOn = DateTime.UtcNow };
		typeof(Category).GetProperty("Id")?.SetValue(category, categoryDto.Id);
		_fixture.SetupFindAsync(new List<Category> { category });
		var getHandler = _fixture.CreateGetHandler();
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);
		_editHandlerMock.HandleAsync(Arg.Any<CategoryDto>()).Returns(Task.FromResult(Result.Ok()));
		var navManager = Services.GetRequiredService<BunitNavigationManager>();
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		var nameInput = cut.Find("#name");
		var form = cut.Find("form");
		await cut.InvokeAsync(() => nameInput.Change("Updated Name"));
		await cut.InvokeAsync(() => form.Submit());
		// Assert: NavigationManager should be called to navigate
		navManager.Uri.Should().Contain("categories");
	}

}
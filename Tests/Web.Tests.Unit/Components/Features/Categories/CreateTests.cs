// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  Web.Tests.Bunit
// =======================================================

using Web.Components.Features.Categories.CategoryCreate;

namespace Web.Components.Features.Categories;

/// <summary>
///   Bunit tests for <see cref="Articles.ArticleCreate.Create" /> component.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Create))]
public class CreateTests : BunitContext
{

	private readonly IValidator<CategoryDto> _validator = Substitute.For<IValidator<CategoryDto>>();
	private readonly CreateCategory.ICreateCategoryHandler _createHandlerMock;

	public CreateTests()
	{
		_createHandlerMock = Substitute.For<CreateCategory.ICreateCategoryHandler>();
		// Default handler behavior: return success to avoid awaiting null Tasks
		_createHandlerMock.HandleAsync(Arg.Any<CategoryDto>()).Returns(Task.FromResult(Result.Ok()));
		Services.AddScoped(_ => _createHandlerMock);
		Services.AddScoped<ILogger<Create>, Logger<Create>>();
		Services.AddScoped(_ => _validator);
		Services.AddCascadingAuthenticationState();
	}

	[Fact]
	public void Renders_Form_And_Heading()
	{

		// Arrange
		Helpers.SetAuthorization(this);

		// Act
		var cut = Render<Create>();

		// Assert
		cut.Markup.Should().Contain("Create Category");
		cut.Markup.Should().Contain("Name");
		cut.Markup.Should().Contain("Create");
		cut.Markup.Should().Contain("Cancel");
		cut.Find("form").Should().NotBeNull();
		cut.Find("button[type='submit']").TextContent.Should().Contain("Save Changes");

	}

	[Fact]
	public void Shows_Validation_Error_When_Name_Is_Empty()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Act
		form.Submit();

		// Assert
		cut.Markup.Should().Contain("CategoryName cannot be empty.");

	}

	[Fact]
	public async Task Submits_Valid_Form_And_Navigates()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		var navMan = Services.GetRequiredService<BunitNavigationManager>();
		// Ensure handler returns success so the component navigates
		_createHandlerMock.HandleAsync(Arg.Any<CategoryDto>()).Returns(Task.FromResult(Result.Ok()));
		var cut = Render<Create>();

		// Act
		await cut.InvokeAsync(() => cut.Find("#name").Change("Test Category"));
		await cut.InvokeAsync(() => cut.Find("form").Submit());

		// Assert
		navMan.Uri.Should().EndWith("/categories");

	}

	[Fact]
	public async Task Shows_Error_Message_On_Service_Exception()
	{

		// Arrange
		Helpers.SetAuthorization(this);

		// Arrange failure: configure the existing handler mock to return a failure
		_createHandlerMock.HandleAsync(Arg.Any<CategoryDto>()).Returns(Task.FromResult(Result.Fail("Service error")));

		var cut = Render<Create>();
		var nameInput = cut.Find("#name");
		var form = cut.Find("form");

		// Act
		await cut.InvokeAsync(() => nameInput.Change("Test"));
		await cut.InvokeAsync(() => form.Submit());

		// Assert
		cut.Markup.Should().Contain("Service error");

	}

	[Fact]
	public void Disables_Submit_Button_While_Submitting()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var cut = Render<Create>();
		var nameInput = cut.Find("#name");
		var form = cut.Find("form");
		var submitButton = cut.Find("button[type='submit']");

		var tcs = new TaskCompletionSource<Result>();

		// Make the handler return a long-running task so the component sets _isSubmitting
		_createHandlerMock.HandleAsync(Arg.Any<CategoryDto>()).Returns(_ => tcs.Task);

		// Act
		nameInput.Change("Test");
		form.Submit();

		// Assert
		submitButton.HasAttribute("disabled").Should().BeTrue();
		cut.Markup.Should().Contain("Creating");

		tcs.SetResult(Result.Ok()); // Complete the task
	}

	[Fact]
	public void Cancel_Link_Navigates_To_Categories_List()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var navMan = Services.GetRequiredService<BunitNavigationManager>();
		var cut = Render<Create>();

		var cancelButton = cut.Find("button[type='button']");

		// Act
		cancelButton.Click();

		// Assert
		navMan.Uri.Should().EndWith("/categories");

	}

}
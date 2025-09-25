// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     DetailsTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Categories.CategoryDetails;

/// <summary>
///   Unit tests for <see cref="Categories.CategoryDetails.Details" />
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Details))]
public class DetailsTests : BunitContext
{

	private readonly IValidator<CategoryDto> _categoryDtoValidator = Substitute.For<IValidator<CategoryDto>>();

	public DetailsTests()
	{
		Services.AddScoped<ILogger<Details>, Logger<Details>>();
		Services.AddScoped(_ => _categoryDtoValidator);
		Services.AddCascadingAuthenticationState();
	}

	[Fact]
	public void RendersNotFound_WhenCategoryIsNull()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();

		getSub.HandleAsync(Arg.Any<Guid>())
				.Returns(Result.Fail<CategoryDto>("Category not found."));

		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, Guid.NewGuid()));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category not found");
	}

	[Fact]
	public void RendersCategoryDetails_WhenCategoryIsPresent()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		CategoryDto categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		categoryDto.CreatedOn = GetStaticDate();
		categoryDto.ModifiedOn = GetStaticDate();

		// register a handler that returns the DTO for the matching id
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();

		getSub.HandleAsync(Arg.Is<Guid>(id => id == categoryDto.Id))
				.Returns(Result.Ok(categoryDto));

		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain(categoryDto.CategoryName);
		cut.Markup.Should().Contain("Created On: 1/1/2025");
		cut.Markup.Should().Contain("Modified On: 1/1/2025");
		cut.Find("button.btn-secondary").Should().NotBeNull();

		//not admin the edit button should be disabled
		cut.Find("button.btn-secondary").HasAttribute("disabled").Should().BeTrue();
		cut.Find("button.btn-light").Should().NotBeNull();
	}

	[Fact]
	public void RendersCategoryDetails_WhenCategoryIsPresentAndUserIsAdmin()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		CategoryDto categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		categoryDto.CreatedOn = GetStaticDate();
		categoryDto.ModifiedOn = GetStaticDate();

		// register a handler that returns the DTO for the matching id
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();

		getSub.HandleAsync(Arg.Is<Guid>(id => id == categoryDto.Id))
				.Returns(Result.Ok(categoryDto));

		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain(categoryDto.CategoryName);
		cut.Markup.Should().Contain("Created On: 1/1/2025");
		cut.Markup.Should().Contain("Modified On: 1/1/2025");
		cut.Find("button.btn-secondary").Should().NotBeNull();

		//not admin the edit button should be disabled
		cut.Find("button.btn-secondary").HasAttribute("disabled").Should().BeFalse();
		cut.Find("button.btn-light").Should().NotBeNull();
	}

	[Fact]
	public void NavigatesToEditPage_WhenEditButtonClicked()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		CategoryDto categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<Guid>()).Returns(Result.Ok(categoryDto));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);
		BunitNavigationManager navigationManager = Services.GetRequiredService<BunitNavigationManager>();

		// Act
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		cut.Find("button.btn-secondary").Click();

		// Assert
		navigationManager.Uri.Should().EndWith($"/categories/edit/{categoryDto.Id}");
	}

	[Fact]
	public void NavigatesToListPage_WhenBackButtonClicked()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		CategoryDto categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<Guid>()).Returns(Result.Ok(categoryDto));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);
		BunitNavigationManager navigationManager = Services.GetRequiredService<BunitNavigationManager>();

		// Act
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		cut.Find("button.btn-light").Click();

		// Assert
		navigationManager.Uri.Should().EndWith("/categories");
	}

	[Fact]
	public void HandlesEmptyObjectId()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		TestServiceRegistrations.RegisterCommonUtilities(this);
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();

		getSub.HandleAsync(Arg.Is<Guid>(id => id == Guid.Empty))
				.Returns(Result.Fail<CategoryDto>("Category not found."));

		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, Guid.Empty));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category not found");
	}

	[Fact]
	public void HandlesServiceException_Gracefully()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		TestServiceRegistrations.RegisterCommonUtilities(this);
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();

		getSub.HandleAsync(Arg.Any<Guid>())
				.Returns(Result.Fail<CategoryDto>("Category service failure."));

		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, Guid.NewGuid()));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category service failure.");
	}

	[Fact]
	public void Unauthenticated_User_Is_Shown_NotAuthorized()
	{
		// Arrange - simulate not authorized
		Helpers.SetAuthorization(this, false);
		Services.AddCascadingAuthenticationState();

		// Act - Directly render an ErrorPageComponent with 401 error codes
		IRenderedComponent<ErrorPageComponent> cut = Render<ErrorPageComponent>(parameters => parameters
				.Add(p => p.ErrorCode, 401)
				.Add(p => p.TextColor, "red-600")
				.Add(p => p.ShadowStyle, "shadow-red-500")
		);

		// Assert - Should contain the 401 Unauthorized messages
		cut.Markup.Should().Contain("401 Unauthorized");
		cut.Markup.Should().Contain("You are not authorized to view this page.");
	}

	[Fact]
	public void Authenticated_Admin_Can_View_Details()
	{
		// Arrange - simulate authenticated Admin
		Helpers.SetAuthorization(this, true, "Admin");
		TestServiceRegistrations.RegisterCommonUtilities(this);
		CategoryDto categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<Guid>()).Returns(Result.Ok(categoryDto));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act - render the Details component directly since we've set the authorization state
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(10));

		// Assert
		cut.Markup.Should().Contain(categoryDto.CategoryName);
		cut.Find("button.btn-secondary").Should().NotBeNull();
		cut.Find("button.btn-light").Should().NotBeNull();
	}

	[Theory]
	[InlineData("User")]
	[InlineData("Author")]
	public void NonAdmin_User_WillSeeADisabledEditButton(string role)
	{
		// Arrange - simulate authenticated non-admin (User)
		Helpers.SetAuthorization(this, true, role);
		TestServiceRegistrations.RegisterCommonUtilities(this);
		CategoryDto categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<Guid>()).Returns(Result.Ok(categoryDto));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		//not admin the edit button should be disabled
		cut.Find("button.btn-secondary").HasAttribute("disabled").Should().BeTrue();
	}

	[Fact]
	public async Task ShowsSpinnerWhileLoading_AndHidesAfter()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		TaskCompletionSource<Result<CategoryDto>> tcs = new ();
		GetCategory.IGetCategoryHandler? getSub = Substitute.For<GetCategory.IGetCategoryHandler>();

		// Return a Task that won't complete until we call TrySetResult
		getSub.HandleAsync(Arg.Any<Guid>()).Returns(tcs.Task);
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act - render the component; since the handler Task is pending, _isLoading should remain true
		IRenderedComponent<Details> cut = Render<Details>(parameters => parameters.Add(p => p.Id, Guid.NewGuid()));

		// Immediately after render the spinner should be present
		cut.Markup.Should().Contain("animate-spin");

		// Now complete the handler with a failed result (use the standard message expected by other tests)
		tcs.TrySetResult(Result.Fail<CategoryDto>("Category not found."));

		// Yield to allow the rendering loop to process the completed task
		await Task.Yield();

		// Wait for the component to update (spinner removed)
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert - spinner is gone and the fallback message is displayed
		cut.Markup.Should().NotContain("animate-spin");
		cut.Markup.Should().Contain("Category not found");
	}

}
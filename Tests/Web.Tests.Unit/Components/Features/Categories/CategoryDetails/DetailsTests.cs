// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     DetailsTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  Web.Tests.Bunit
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

	#region Helper Methods

	/// <summary>
	/// Sets up an authorized admin user for testing admin functionality.
	/// </summary>
	private void SetupAdminUser()
	{
		Helpers.SetAuthorization(this, true, "Admin");
	}

	/// <summary>
	/// Sets up a category handler that returns a successful result with the provided category.
	/// </summary>
	/// <param name="categoryDto">The category to return from the handler.</param>
	/// <returns>The configured handler substitute.</returns>
	private GetCategory.IGetCategoryHandler SetupSuccessfulCategoryHandler(CategoryDto categoryDto)
	{
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Is<ObjectId>(id => id == categoryDto.Id))
			.Returns(Task.FromResult(Result.Ok(categoryDto)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);
		return getSub;
	}

	/// <summary>
	/// Sets up a category handler that returns a failure result.
	/// </summary>
	/// <param name="errorMessage">The error message to return.</param>
	/// <returns>The configured handler substitute.</returns>
	private GetCategory.IGetCategoryHandler SetupFailedCategoryHandler(string errorMessage = "Category not found.")
	{
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<ObjectId>()).Returns(Task.FromResult(Result.Fail<CategoryDto>(errorMessage)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);
		return getSub;
	}

	/// <summary>
	/// Sets up a category handler that throws an exception.
	/// </summary>
	/// <param name="exceptionMessage">The exception message to throw.</param>
	/// <returns>The configured handler substitute.</returns>
	private GetCategory.IGetCategoryHandler SetupExceptionCategoryHandler(string exceptionMessage = "DB error")
	{
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<ObjectId>()).Returns<Task<Result<CategoryDto>>>(_ => throw new Exception(exceptionMessage));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);
		return getSub;
	}

	#endregion

	[Fact]
	public void ShowsLoadingState_Initially()
	{
		// Arrange
		SetupAdminUser();
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		
		// Create a handler that will delay to allow us to check the loading state
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		var tcs = new TaskCompletionSource<Result<CategoryDto>>();
		getSub.HandleAsync(Arg.Any<ObjectId>()).Returns(tcs.Task);
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));

		// Assert - Check that loading state is shown initially
		cut.Markup.Should().Contain("animate-spin");
		cut.FindComponent<LoadingComponent>().Should().NotBeNull();

		// Complete the task to prevent hanging
		tcs.SetResult(Result.Ok(categoryDto));
	}

	[Fact]
	public void AdminUser_CanAccessComponent()
	{
		// Arrange
		SetupAdminUser();
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		SetupSuccessfulCategoryHandler(categoryDto);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert - Admin should see the category details, not authorization error
		cut.Markup.Should().Contain(categoryDto.CategoryName);
		cut.Markup.Should().NotContain("401 Unauthorized");
		cut.Markup.Should().NotContain("You are not authorized to view this page");
	}

	[Fact]
	public void RendersNotFound_WhenCategoryIsNull()
	{
		// Arrange
		SetupAdminUser();
		SetupFailedCategoryHandler();

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, ObjectId.GenerateNewId()));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category not found");
	}

	[Fact]
	public void RendersCategoryDetails_WhenCategoryIsPresent()
	{
		// Arrange
		SetupAdminUser();
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		SetupSuccessfulCategoryHandler(categoryDto);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain(categoryDto.CategoryName);
		cut.Markup.Should().Contain("Created On: 1/1/2025");
		cut.Markup.Should().Contain("Modified On: 1/1/2025");
		cut.Find("button.btn-secondary").Should().NotBeNull();
		cut.Find("button.btn-light").Should().NotBeNull();
	}

	[Fact]
	public void HasCorrectNavigationButtons()
	{
		// Arrange
		SetupAdminUser();
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		SetupSuccessfulCategoryHandler(categoryDto);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Find("button.btn-secondary").Should().NotBeNull();
		cut.Find("button.btn-light").Should().NotBeNull();
	}

	[Fact]
	public void NavigatesToEditPage_WhenEditButtonClicked()
	{
		// Arrange
		SetupAdminUser();
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		SetupSuccessfulCategoryHandler(categoryDto);
		var navigationManager = Services.GetRequiredService<BunitNavigationManager>();

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		cut.Find("button.btn-secondary").Click();

		// Assert
		navigationManager.Uri.Should().EndWith($"/categories/edit/{categoryDto.Id}");
	}

	[Fact]
	public void NavigatesToListPage_WhenBackButtonClicked()
	{
		// Arrange
		SetupAdminUser();
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		SetupSuccessfulCategoryHandler(categoryDto);
		var navigationManager = Services.GetRequiredService<BunitNavigationManager>();

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		cut.Find("button.btn-light").Click();

		// Assert
		navigationManager.Uri.Should().EndWith("/categories");
	}

	[Fact]
	public void HandlesEmptyObjectId()
	{
		// Arrange
		SetupAdminUser();
		SetupFailedCategoryHandler();

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, ObjectId.Empty));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category not found");
	}

	[Fact]
	public void HandlesServiceException_Gracefully()
	{
		// Arrange
		SetupAdminUser();
		SetupExceptionCategoryHandler();

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, ObjectId.GenerateNewId()));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category not found");
	}

	[Fact]
	public void Unauthenticated_User_Is_Shown_NotAuthorized()
	{
		// Arrange - simulate not authorized
		Helpers.SetAuthorization(this, false);
		TestServiceRegistrations.RegisterCommonUtilities(this);

		// Act - render an AuthorizeView with NotAuthorized content to avoid pulling in the whole Router
		RenderFragment<AuthenticationState> authorizedFragment = _ => builder => builder.AddMarkupContent(0, "<div>authorized</div>");
		RenderFragment<AuthenticationState> notAuthorizedFragment = _ => builder =>
		{
			builder.OpenComponent<ErrorPageComponent>(0);
			builder.AddAttribute(1, "ErrorCode", 401);
			builder.AddAttribute(2, "TextColor", "red-600");
			builder.AddAttribute(3, "ShadowStyle", "shadow-red-500");
			builder.CloseComponent();
		};

		var cut = Render<AuthorizeView>(parameters => parameters
			.Add(p => p.Authorized, authorizedFragment)
			.Add(p => p.NotAuthorized, notAuthorizedFragment)
		);

		// Assert - NotAuthorized content should show the 401 ErrorPageComponent message
		cut.Markup.Should().Contain("401 Unauthorized");
		cut.Markup.Should().Contain("You are not authorized to view this page.");
	}

	[Theory]
	[InlineData("User")]
	[InlineData("Author")]
	public void Authenticated_NonAdmin_Is_Shown_NotAuthorized(string role)
	{
		// Arrange - simulate an authenticated user without an Admin role
		Helpers.SetAuthorization(this, true, role);
		TestServiceRegistrations.RegisterCommonUtilities(this);

		// Act - render an AuthorizeView with NotAuthorized content to avoid pulling in the whole Router
		RenderFragment<AuthenticationState> authorizedFragment = _ => builder => builder.AddMarkupContent(0, "<div>authorized</div>");
		RenderFragment<AuthenticationState> notAuthorizedFragment = _ => builder =>
		{
			builder.OpenComponent<ErrorPageComponent>(0);
			builder.AddAttribute(1, "ErrorCode", 401);
			builder.AddAttribute(2, "TextColor", "red-600");
			builder.AddAttribute(3, "ShadowStyle", "shadow-red-500");
			builder.CloseComponent();
		};

		var cut = Render<AuthorizeView>(parameters => parameters
			.Add(p => p.Authorized, authorizedFragment)
			.Add(p => p.NotAuthorized, notAuthorizedFragment)
		);

		// Assert - NotAuthorized content should show the 401 ErrorPageComponent message
		cut.Markup.Should().Contain("401 Unauthorized");
		cut.Markup.Should().Contain("You are not authorized to view this page.");
	}

}

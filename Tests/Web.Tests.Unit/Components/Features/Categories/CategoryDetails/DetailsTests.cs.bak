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

	[Fact]
	public void RendersNotFound_WhenCategoryIsNull()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<ObjectId>()).Returns(Task.FromResult(Result.Fail<CategoryDto>("Category not found.")));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

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
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Is<ObjectId>(id => id == categoryDto.Id)).Returns(Task.FromResult(Result.Ok(categoryDto)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

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
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<ObjectId>()).Returns(Task.FromResult(Result.Ok(categoryDto)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

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
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<ObjectId>()).Returns(Task.FromResult(Result.Ok(categoryDto)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);
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
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<ObjectId>()).Returns(Task.FromResult(Result.Ok(categoryDto)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);
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
		Helpers.SetAuthorization(this);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<ObjectId>()).Returns(Task.FromResult(Result.Fail<CategoryDto>("Category not found.")));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

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
		Helpers.SetAuthorization(this);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<ObjectId>()).Returns<Task<Result<CategoryDto>>>(_ => throw new Exception("DB error"));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

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
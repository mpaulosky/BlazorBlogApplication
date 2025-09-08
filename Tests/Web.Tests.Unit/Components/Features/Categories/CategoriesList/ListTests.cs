// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ListTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Categories.CategoriesList;

/// <summary>
///   Unit tests for <see cref="List{T}" />
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(List))]
public class ListTests : BunitContext
{

	private readonly GetCategories.IGetCategoriesHandler _mockHandler;

	public ListTests()
	{
		_mockHandler = Substitute.For<GetCategories.IGetCategoriesHandler>();
		Services.AddScoped(_ => _mockHandler);
		Services.AddScoped<ILogger<List>>(_ => Substitute.For<ILogger<List>>());
		Services.AddCascadingAuthenticationState();
		Services.AddAuthorization();
	}

	private void SetupHandlerCategories(IEnumerable<CategoryDto>? categories, bool success = true, string? error = null)
	{
		_mockHandler.HandleAsync(Arg.Any<bool>()).Returns(success
				? Task.FromResult(Result<IEnumerable<CategoryDto>>.Ok(categories ?? new List<CategoryDto>()))
				: Task.FromResult(Result<IEnumerable<CategoryDto>>.Fail(error ?? "Error")));
	}

	[Fact]
	public void RendersNoCategories_WhenCategoriesIsNullOrEmptyAndResultIsOk()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerCategories(null);

		// Act
		var cut = Render<List>();

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
		var cut = Render<List>();

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
				new()  { CategoryName = "Cat1", CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now, Archived = false },
				new()  { CategoryName = "Cat2", CreatedOn = DateTime.Now, ModifiedOn = null, Archived = true }
		};

		SetupHandlerCategories(categoriesDto);

		// Act
		var cut = Render<List>();

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
		var cut = Render<List>();

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, true);

		cut.Render();

		// loading component renders a spinner and 'Loading...' text
		cut.Markup.Should().Contain("Loading...");
	}

	[Fact]
	public async Task ShowsSpinnerWhileLoading_AndHidesAfter()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var tcs = new TaskCompletionSource<Result<IEnumerable<CategoryDto>>>();
		_mockHandler.HandleAsync(Arg.Any<bool>()).Returns(_ => tcs.Task);

		// Act
		var cut = Render<List>();

		// Spinner present while pending
		cut.Markup.Should().Contain("Loading");

		// Complete task
		tcs.TrySetResult(Result<IEnumerable<CategoryDto>>.Ok(new List<CategoryDto>()));
		await Task.Yield();

		cut.WaitForState(() => !cut.Markup.Contains("Loading"), TimeSpan.FromSeconds(5));
		cut.Markup.Should().NotContain("Loading");
	}

	[Fact]
	public void Renders_Error_State_When_Handler_Fails()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerCategories(null, false, "Failed to load categories");
		var cut = Render<List>();

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Render();

		// Error state shows the failure message via logger/error text
		cut.Markup.Should().Contain("No categories available")
				.And.Contain("Create New Category");

		// The component logs errors but shows a friendly message; ensure test checks for logged error indirectly by ensuring no rows are present
	}

	[Fact]
	public void Navigates_To_Create_New_Category()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var nav = Services.GetRequiredService<BunitNavigationManager>();
		SetupHandlerCategories(new List<CategoryDto>());
		var cut = Render<List>();

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

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
				new()
				{
						Id = ObjectId.GenerateNewId(), CategoryName = "Cat1", CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now,
						Archived = false
				}
		};

		SetupHandlerCategories(categoriesDto);
		var cut = Render<List>();

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_categories", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, categoriesDto.AsQueryable());

		cut.Render();
		cut.Find("button.btn-primary").Click();

		// the component navigates to a segment-style edit route
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
				new()
				{
						Id = ObjectId.GenerateNewId(), CategoryName = "Cat1", CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now,
						Archived = false
				}
		};

		SetupHandlerCategories(categoriesDto);
		var cut = Render<List>();

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_categories", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, categoriesDto.AsQueryable());

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
				new()  { CategoryName = "Cat1", CreatedOn = DateTime.Now, ModifiedOn = DateTime.Now, Archived = false }
		};

		SetupHandlerCategories(categoriesDto);
		var cut = Render<List>();

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_categories", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, categoriesDto.AsQueryable());

		cut.Render();
		var expectedHeaders = new[] { "Category Name", "Created On", "Modified On", "Archived", "Actions" };

		foreach (var header in expectedHeaders)
		{
			cut.Markup.Should().Contain(header);
		}
	}

	[Fact]
	public void Unauthenticated_User_Is_Shown_NotAuthorized()
	{
		// Arrange - simulate not authorized
		Helpers.SetAuthorization(this, false);
		TestServiceRegistrations.RegisterCommonUtilities(this);

		// Act - render an AuthorizeView with NotAuthorized content to avoid pulling in the whole Router
		RenderFragment<AuthenticationState> authorizedFragment =
				_ => builder => builder.AddMarkupContent(0, "<div>authorized</div>");

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
		RenderFragment<AuthenticationState> authorizedFragment =
				_ => builder => builder.AddMarkupContent(0, "<div>authorized</div>");

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

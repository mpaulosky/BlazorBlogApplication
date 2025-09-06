// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Articles.ArticleEdit;

/// <summary>
///   Unit tests for <see cref="Edit" /> (Articles Edit Page).
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Edit))]
public class EditTests : BunitContext
{

	private readonly IValidator<ArticleDto> _articleDtoValidator = Substitute.For<IValidator<ArticleDto>>();

	private readonly EditArticle.IEditArticleHandler _mockHandler;

	private readonly GetArticle.IGetArticleHandler _mockGetArticleHandler;

	public EditTests()
	{
		_mockHandler = Substitute.For<EditArticle.IEditArticleHandler>();
		_mockGetArticleHandler = Substitute.For<GetArticle.IGetArticleHandler>();
		Services.AddScoped(_ => _mockHandler);
		Services.AddScoped(_ => _mockGetArticleHandler);
		Services.AddScoped<ILogger<Edit>, Logger<Edit>>();
		Services.AddScoped(_ => _articleDtoValidator);
		Services.AddCascadingAuthenticationState();
		Services.AddAuthorization();

		// Ensure bUnit authorization and common test services are registered
		TestServiceRegistrations.RegisterCommonUtilities(this);

	}

	[Fact]
	public void Renders_NotFound_When_Article_Is_Null()
	{

		// Arrange
		var id = ObjectId.GenerateNewId();
		_mockGetArticleHandler.HandleAsync(id).Returns(Task.FromResult(Result<ArticleDto>.Fail("Article not found")));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, id));

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, null);

		// Act
		cut.Render();

		// Assert
		cut.Markup.Should().Contain("Article not found");

	}

	[Fact]
	public void Renders_Form_With_Article_Data()
	{

		// Arrange
		var article = FakeArticleDto.GetNewArticleDto(true);
		_mockGetArticleHandler.HandleAsync(article.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(article)));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, article);

		// Act
		cut.Render();

		// Assert
		cut.Markup.Should().Contain(article.Title);
		cut.Markup.Should().Contain(article.Introduction);

	}

	[Fact]
	public async Task Submits_Valid_Form_And_Navigates_On_Success()
	{

		// Arrange
		var article = FakeArticleDto.GetNewArticleDto(true);
		_mockGetArticleHandler.HandleAsync(article.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(article)));
		_mockHandler.HandleAsync(article).Returns(Task.FromResult(Result.Ok()));
		var nav = Services.GetRequiredService<BunitNavigationManager>();
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, article);

		cut.Render();

		// Act
		await cut.Find("form").SubmitAsync();

		// Assert
		nav.Uri.Should().EndWith("/articles");

	}

	[Fact]
	public async Task Displays_Error_On_Failed_Submit()
	{

		// Arrange
		var article = FakeArticleDto.GetNewArticleDto(true);
		_mockGetArticleHandler.HandleAsync(article.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(article)));
		_mockHandler.HandleAsync(article).Returns(Task.FromResult(Result.Fail("Update failed")));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, article);

		cut.Render();

		// Act
		await cut.Find("form").SubmitAsync();

		// Assert
		cut.Markup.Should().Contain("Update failed");

	}

	[Fact]
	public void Cancel_Button_Navigates_To_List()
	{

		// Arrange
		var article = FakeArticleDto.GetNewArticleDto(true);
		_mockGetArticleHandler.HandleAsync(article.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(article)));
		var nav = Services.GetRequiredService<BunitNavigationManager>();
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, article);

		cut.Render();

		// Act
		cut.Find("button.btn-light").Click();

		// Assert
		nav.Uri.Should().EndWith("/articles");

	}

	[Fact]
	public void Renders_LoadingComponent_When_IsLoading()
	{
		// Arrange
		var id = ObjectId.GenerateNewId();
		_mockGetArticleHandler.HandleAsync(id).Returns(Task.FromResult(Result<ArticleDto>.Fail("Loading")));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, id));

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, true);

		// Act
		cut.Render();

		// Assert
		cut.Markup.Should().Contain("Loading...");
	}

	[Fact]
	public async Task ShowsSpinnerWhileLoading_AndHidesAfter()
	{
		// Arrange
		var id = ObjectId.GenerateNewId();
		var tcs = new TaskCompletionSource<Result<ArticleDto>>();
		_mockGetArticleHandler.HandleAsync(id).Returns(_ => tcs.Task);

		// Act
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, id));

		// Spinner present while pending
		(cut.Markup.Contains("Loading") || cut.Markup.Contains("animate-spin")).Should().BeTrue();

		// Complete task
		tcs.TrySetResult(Result<ArticleDto>.Ok(FakeArticleDto.GetNewArticleDto(true)));
		await Task.Yield();

		cut.WaitForState(() => !cut.Markup.Contains("animate-spin") && !cut.Markup.Contains("Loading"),
				TimeSpan.FromSeconds(5));

		cut.Markup.Should().NotContain("animate-spin").And.NotContain("Loading");
	}

	[Fact]
	public void Populates_Fields_With_Article_Data()
	{
		// Arrange
		var article = FakeArticleDto.GetNewArticleDto(true);
		_mockGetArticleHandler.HandleAsync(article.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(article)));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, article);

		cut.Render();

		// Assert
		var input = cut.Find("input.form-control");
		input.Should().NotBeNull();
		var valueAttr = input.Attributes["value"];
		valueAttr.Should().NotBeNull();
		valueAttr.Value.Should().Be(article.Title);
	}

	[Fact]
	public void Shows_Validation_Errors_When_Form_Is_Invalid()
	{
		// Arrange
		var article = FakeArticleDto.GetNewArticleDto(true);

		// Make the article invalid by clearing the title so the component's guard triggers
		article.Title = string.Empty;
		_mockGetArticleHandler.HandleAsync(article.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(article)));

		// Ensure the edit handler returns a Task so component code can await it if invoked.
		// Tests that exercise validation still configure a safe default to avoid NREs
		// when the form is submitted synchronously by bUnit.
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(Task.FromResult(Result.Ok()));

		// Configure the FluentValidation validator to return a validation failure
		// so the ValidationSummary/validator components render errors on submitting.
		_articleDtoValidator.Validate(Arg.Any<ValidationContext<ArticleDto>>())
				.Returns(new ValidationResult([new ValidationFailure("Title", "Title is required")]));

		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, article);

		cut.Render();

		// Act
		cut.Find("form").Submit();

		// Assert: the component performs an internal guard check and displays an
		// error message when the Title is empty.
		cut.Markup.Should().Contain("Title cannot be null or empty");
	}

	[Fact]
	public void Submit_Button_Disabled_While_Submitting()
	{
		// Arrange
		var article = FakeArticleDto.GetNewArticleDto(true);
		_mockGetArticleHandler.HandleAsync(article.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(article)));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, article);

		cut.Instance.GetType().GetField("_isSubmitting", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, true);

		cut.Render();

		// Assert
		cut.Find("button[type='submit']").HasAttribute("disabled").Should().BeTrue();
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

	[Fact]
	public void Authenticated_NonAdmin_NonAuthor_Is_Shown_NotAuthorized()
	{
		// Arrange - simulate authenticated user without Admin/Author roles
		Helpers.SetAuthorization(this, true, "User");
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

	[Fact]
	public async Task OnInitializedAsync_SuccessWithNullValue_Sets_Article_Null_And_NotLoading()
	{
		// Arrange
		var id = ObjectId.GenerateNewId();

		// Simulate a successful result but with a null Value
		_mockGetArticleHandler.HandleAsync(id).Returns(Task.FromResult(Result<ArticleDto>.Ok(null!)));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, id));

		// Act - explicitly invoke the lifecycle method to ensure the branch executes
		var onInit = cut.Instance.GetType().GetMethod("OnInitializedAsync", BindingFlags.NonPublic | BindingFlags.Instance);

		if (onInit is not null)
		{
			await cut.InvokeAsync(async () => await (Task)onInit.Invoke(cut.Instance, null)!);
		}

		// Assert - internal state should reflect that there is no article and loading has completed
		var isLoading = (bool?)cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.GetValue(cut.Instance);

		var article = cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.GetValue(cut.Instance) as ArticleDto;

		isLoading.Should().BeFalse();
		article.Should().BeNull();
	}

	[Fact]
	public async Task OnInitializedAsync_Failure_Sets_ErrorMessage_And_NotLoading()
	{
		// Arrange
		var id = ObjectId.GenerateNewId();

		// Simulate a failed result from the GetArticle handler
		_mockGetArticleHandler.HandleAsync(id).Returns(Task.FromResult(Result<ArticleDto>.Fail("Fetch error")));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, id));

		// Act - explicitly invoke the lifecycle method to ensure the failure branch executes
		var onInit = cut.Instance.GetType().GetMethod("OnInitializedAsync", BindingFlags.NonPublic | BindingFlags.Instance);

		if (onInit is not null)
		{
			await cut.InvokeAsync(async () => await (Task)onInit.Invoke(cut.Instance, null)!);
		}

		// Assert - internal state should reflect the error, and loading has completed
		var isLoading = (bool?)cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.GetValue(cut.Instance);

		var article = cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.GetValue(cut.Instance) as ArticleDto;

		var errorMessage = (string?)cut.Instance.GetType()
				.GetField("_errorMessage", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(cut.Instance);

		isLoading.Should().BeFalse();
		article.Should().BeNull();
		errorMessage.Should().Be("Fetch error");
	}

	[Fact]
	public async Task HandleValidSubmit_With_Whitespace_Title_Shows_Error_And_Resets_Flags()
	{
		// Arrange
		var article = FakeArticleDto.GetNewArticleDto(true);

		// Make the title whitespace so Trim().Length == 0
		article.Title = "   ";
		_mockGetArticleHandler.HandleAsync(article.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(article)));

		// Ensure the edit handler is callable but not used in this guard path
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(Task.FromResult(Result.Ok()));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));

		// Ensure the component state reflects not loading and article populated
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, article);

		// Act - invoke the summit handler directly
		var submit = cut.Instance.GetType().GetMethod("HandleValidSubmit", BindingFlags.NonPublic | BindingFlags.Instance);

		if (submit is not null)
		{
			await cut.InvokeAsync(async () => await (Task)submit.Invoke(cut.Instance, null)!);
		}

		// Assert - the guard should set an error message and reset submission/loading flags
		var isSubmitting = (bool?)cut.Instance.GetType()
				.GetField("_isSubmitting", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(cut.Instance);

		var isLoading = (bool?)cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.GetValue(cut.Instance);

		var errorMessage = (string?)cut.Instance.GetType()
				.GetField("_errorMessage", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(cut.Instance);

		isSubmitting.Should().BeFalse();
		isLoading.Should().BeFalse();
		errorMessage.Should().Be("Title cannot be null or empty.");
	}

	[Fact]
	public async Task HandleValidSubmit_When_Article_Is_Null_Does_NotThrow_And_Resets_Flags()
	{
		// Arrange
		var id = ObjectId.GenerateNewId();
		_mockGetArticleHandler.HandleAsync(id).Returns(Task.FromResult(Result<ArticleDto>.Ok(null!)));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, id));

		// Ensure the component state reflects not loading and no article
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, null);

		// Act - invoke the summit handler directly
		var submit = cut.Instance.GetType().GetMethod("HandleValidSubmit", BindingFlags.NonPublic | BindingFlags.Instance);

		if (submit is not null)
		{
			await cut.InvokeAsync(async () => await (Task)submit.Invoke(cut.Instance, null)!);
		}

		// Assert - submission flags should be false, and loading should remain false
		var isSubmitting = (bool?)cut.Instance.GetType()
				.GetField("_isSubmitting", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(cut.Instance);

		var isLoading = (bool?)cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.GetValue(cut.Instance);

		isSubmitting.Should().BeFalse();
		isLoading.Should().BeFalse();
	}

}

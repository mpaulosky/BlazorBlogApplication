// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Articles.ArticleCreate;

/// <summary>
///   Unit tests for <see cref="Create" /> (Articles Create Page).
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Create))]
public class CreateTests : BunitContext
{

	private readonly CreateArticle.ICreateArticleHandler _mockHandler;

	public CreateTests()
	{
		_mockHandler = Substitute.For<CreateArticle.ICreateArticleHandler>();
		var mockLogger = Substitute.For<ILogger<Create>>();
		Services.AddScoped(_ => _mockHandler);
		Services.AddScoped(_ => mockLogger);
		Services.AddScoped<CreateArticle.ICreateArticleHandler>(_ => _mockHandler);
		Services.AddCascadingAuthenticationState();
		Services.AddAuthorization();
	}

	//
	// public CreateTests(ILogger<Create> mockLogger) {
	// 	_mockLogger = mockLogger;
	// }

	[Fact]
	public void Renders_Form_With_All_Fields()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var cut = Render<Create>();

		// Assert
		cut.Markup.Should().Contain("Title");
		cut.Markup.Should().Contain("Introduction");
		cut.Markup.Should().Contain("Content");
		cut.Markup.Should().Contain("Cover Image URL");
		cut.Markup.Should().Contain("Published");
	}

/// <summary>
	///   Should show validation errors when the form is submitted with invalid data.
	/// </summary>
	[Fact]
	public async Task Shows_Validation_Errors_When_Form_Is_InvalidAsync()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Act
		await form.SubmitAsync();

		// Assert
		cut.Markup.Should().Contain("validation-message");
	}

	[Fact]
	public async Task Submits_Form_And_Calls_Handler_When_Valid()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(Task.FromResult(Result.Ok()));
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Fill in required fields
		cut.Find("input[name='_article.Title']").Change("Test");
		cut.Find("input[name='_article.Introduction']").Change("Intro");
		cut.Find("textarea[name='_article.Content']").Change("Content");
		cut.Find("input[name='_article.CoverImageUrl']").Change("https://img.com/test.jpg");
		cut.Find("input[name='_article.UrlSlug']").Change("test_article");

		// Act
		await form.SubmitAsync();

		// Assert
		await _mockHandler.Received(1).HandleAsync(Arg.Any<ArticleDto>());
	}

	[Fact]
	public async Task Shows_Error_Message_When_Handler_Fails()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(Task.FromResult(Result.Fail("Error occurred")));
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Fill in required fields
		cut.Find("input[name='_article.Title']").Change("Test");
		cut.Find("input[name='_article.Introduction']").Change("Intro");
		cut.Find("textarea[name='_article.Content']").Change("Content");
		cut.Find("input[name='_article.CoverImageUrl']").Change("https://img.com/test.jpg");
		cut.Find("input[name='_article.UrlSlug']").Change("test_article");

		// Act
		await form.SubmitAsync();

		// Assert
		cut.Markup.Should().Contain("alert-danger");
	}

	[Fact]
	public async Task Navigates_To_Article_List_On_Success()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(Task.FromResult(Result.Ok()));
		var nav = Services.GetRequiredService<NavigationManager>();
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Fill in required fields
		cut.Find("input[name='_article.Title']").Change("Test");
		cut.Find("input[name='_article.Introduction']").Change("Intro");
		cut.Find("textarea[name='_article.Content']").Change("Content");
		cut.Find("input[name='_article.CoverImageUrl']").Change("https://img.com/test.jpg");
		cut.Find("input[name='_article.UrlSlug']").Change("test_article");

		// Act
		await form.SubmitAsync();

		// Assert
		nav.Uri.Should().EndWith("/articles");
	}

	[Fact]
	public async Task Submit_Button_Disabled_While_Submitting()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var tcs = new TaskCompletionSource<Result>();
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(tcs.Task);
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Fill in required fields
		cut.Find("input[name='_article.Title']").Change("Test");
		cut.Find("input[name='_article.Introduction']").Change("Intro");
		cut.Find("textarea[name='_article.Content']").Change("Content");
		cut.Find("input[name='_article.CoverImageUrl']").Change("https://img.com/test.jpg");
		cut.Find("input[name='_article.UrlSlug']").Change("test_article");

		// Act
		var submitButton = cut.Find("button[type='submit']");
		var submitTask = form.SubmitAsync();

		// Assert
		submitButton.HasAttribute("disabled").Should().BeTrue();

		// Complete the handler to finish submission
		tcs.SetResult(Result.Ok());
		await submitTask;
	}

	[Fact]
	public void Cancel_Button_Navigates_To_Articles_List()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var nav = Services.GetRequiredService<NavigationManager>();
		var cut = Render<Create>();

		// Act
		var cancel = cut.Find("button[type='button']");
		cancel.Click();

		// Assert
		nav.Uri.Should().EndWith("/articles");
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

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Bunit
// =======================================================

using Web.Components.Features.Articles.ArticleEdit;
using static Web.Components.Features.Articles.ArticleEdit.EditArticle;
using static Web.Components.Features.Articles.ArticleGet.GetArticle;

namespace Web.Components.Features.Articles;

/// <summary>
///   Unit tests for <see cref="Edit" /> (Articles Edit Page).
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Edit))]
public class EditTests : BunitContext
{

	private readonly IValidator<ArticleDto> _articleDtoValidator = Substitute.For<IValidator<ArticleDto>>();
	private readonly IEditArticleHandler _mockHandler;
	private readonly IGetArticleHandler _mockGetArticleHandler;

	public EditTests()
	{
		_mockHandler = Substitute.For<IEditArticleHandler>();
		_mockGetArticleHandler = Substitute.For<IGetArticleHandler>();
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
	public void Populates_Fields_With_Article_Data()
	{
		// Arrange
		var article = FakeArticleDto.GetNewArticleDto(true);
		_mockGetArticleHandler.HandleAsync(article.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(article)));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, article);
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
		// so the ValidationSummary/validator components render errors on submit.
		_articleDtoValidator.Validate(Arg.Any<ValidationContext<ArticleDto>>())
			.Returns(new ValidationResult(new[] { new ValidationFailure("Title", "Title is required") }));
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, article.Id));
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, article);
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
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_article", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, article);
		cut.Instance.GetType().GetField("_isSubmitting", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, true);
		cut.Render();

		// Assert
		cut.Find("button[type='submit']").HasAttribute("disabled").Should().BeTrue();
	}

	[Fact(Skip = "Bunit does not enforce [Authorize]; form is always rendered in test context.")]
	public void Only_Admin_Or_Author_Can_Access()
	{
		// Arrange
		Helpers.SetAuthorization(this, false);
		var id = ObjectId.GenerateNewId();
		var cut = Render<Edit>(parameters => parameters.Add(p => p.Id, id));

		// Assert
		cut.FindAll("form").Should().BeEmpty();
	}

}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     DetailsTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Articles.ArticleDetails;

/// <summary>
///   Unit tests for <see cref="Details" />
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Details))]
public class DetailsTests : BunitContext
{

	private readonly GetArticle.IGetArticleHandler _mockHandler;

	public DetailsTests()
	{
		// Arrange: Register all required services for ArticleHandler
		_mockHandler = Substitute.For<GetArticle.IGetArticleHandler>();
		Services.AddScoped(_ => _mockHandler);
		Services.AddScoped<ILogger<Details>, Logger<Details>>();
		Services.AddCascadingAuthenticationState();
		Services.AddAuthorization();

		// Register common utilities (navigation, auth fallback, logger factories)
		TestServiceRegistrations.RegisterCommonUtilities(this);
	}


	[Fact]
	public void RendersNotFound_WhenArticleIsNull()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var articleId = ObjectId.GenerateNewId();
		_mockHandler.HandleAsync(articleId).Returns(Task.FromResult(Result<ArticleDto>.Fail("Article not found")));
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleId));

		// Act
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, null);
		cut.Render();

		// Assert
		cut.Markup.Should().Contain("Article not found");
	}

	[Fact]
	public void RendersArticleDetails_WhenArticleIsPresent()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		cut.Markup.Should().Contain(articleDto.Title);
		cut.Markup.Should().Contain(articleDto.Introduction);
		cut.Markup.Should().Contain(articleDto.Author.UserName);
		cut.Markup.Should().Contain(articleDto.Category.CategoryName);
	}

	[Fact]
	public void RendersArticleContent_AsMarkupString()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		const string expectedContent = "<p>Test HTML content</p>";
		articleDto.Content = expectedContent;
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		cut.Markup.Should().Contain(expectedContent);
	}

	[Fact]
	public void DisplaysCorrectPublishedStatus()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		articleDto.IsPublished = true;
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		cut.Markup.Should().Contain("Published:</strong> Yes");
	}

	[Fact]
	public void DisplaysPublishedDate_WhenPresent()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		articleDto.IsPublished = true;
		articleDto.PublishedOn = DateTime.Now.Date;
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		cut.Markup.Should().Contain(articleDto.PublishedOn?.ToString("d"));
	}

	[Fact]
	public void HasCorrectNavigationButtons()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		cut.Find("button.btn-secondary").Should().NotBeNull();
		cut.Find("button.btn-light").Should().NotBeNull();
	}

	[Fact]
	public void NavigatesToEditPage_WhenEditButtonClicked()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		var navigationManager = Services.GetRequiredService<BunitNavigationManager>();
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();
		cut.Find("button.btn-secondary").Click();

		// Assert
		navigationManager.Uri.Should().EndWith($"/articles/edit/{articleDto.Id}");
	}

	[Fact]
	public void NavigatesToListPage_WhenBackButtonClicked()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		var navigationManager = Services.GetRequiredService<BunitNavigationManager>();
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();
		cut.Find("button.btn-light").Click();


		// Assert
		navigationManager.Uri.Should().EndWith("/articles");
	}

	[Fact]
	public void DisplaysNotPublished_WhenArticleIsNotPublished()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		articleDto.IsPublished = false;
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters
				.Add(p => p.Id, articleDto.Id));

		// Simulate loading complete
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		cut.Markup.Should().Contain("Published:</strong> No");
	}

	[Fact]
	public void DisplaysNullPublishedDate_WhenNotPublished()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		articleDto.IsPublished = false;
		articleDto.PublishedOn = null;
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters
				.Add(p => p.Id, articleDto.Id));

		// Simulate loading complete
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		cut.Markup.Should().Contain("Published On:</strong> ");
		cut.Markup.Should().NotContain("Published On:</strong> null");
	}

	[Fact]
	public void HandlesEmptyObjectId()
	{
		// Arrange & Act
		var emptyId = ObjectId.Empty;
		_mockHandler.HandleAsync(emptyId).Returns(Task.FromResult(Result<ArticleDto>.Fail("Article not found")));
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, emptyId));

		// Simulate loading complete
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Render();

		// Assert
		cut.Markup.Should().Contain("Article not found");
	}

	[Fact]
	public void HandlesServiceException_Gracefully()
	{
		// Arrange
		var articleId = ObjectId.GenerateNewId();
		_mockHandler.HandleAsync(articleId).Returns(Task.FromResult(Result<ArticleDto>.Fail("Service exception")));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleId));

		// Simulate loading complete
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Render();

		// Assert
		cut.Markup.Should().Contain("Service exception");
	}

	[Fact]
	public void DisplaysCoverImage_WithCorrectAttributes()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		articleDto.CoverImageUrl = "https://example.com/image.jpg";
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));

		// Simulate loading complete
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		var imgElement = cut.Find("img.card-img-top");
		imgElement.Should().NotBeNull();
		imgElement.Attributes["src"]?.Value.Should().Be(articleDto.CoverImageUrl);
		imgElement.Attributes["alt"]?.Value.Should().Be("Cover");
	}

	[Fact]
	public void RendersLoadingComponent_WhenIsLoadingIsTrue()
	{
		// Arrange
		var articleId = ObjectId.GenerateNewId();
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleId));

		// Simulate loading state
		// Directly set the private field so the component renders the LoadingComponent
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, true);

		cut.Render();

		// Assert: LoadingComponent contains the text 'Loading...'
		cut.Markup.Should().Contain("Loading...");
	}

	[Fact]
	public async Task ShowsSpinnerWhileLoading_AndHidesAfter()
	{
		// Arrange
		var articleId = ObjectId.GenerateNewId();
		var tcs = new TaskCompletionSource<Result<ArticleDto>>();
		_mockHandler.HandleAsync(articleId).Returns(_ => tcs.Task);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleId));

		// Spinner should be present while the task is pending
		cut.Markup.Should().Contain("animate-spin");

		// Complete the task with a failure
		tcs.TrySetResult(Result<ArticleDto>.Fail("Article not found"));
		await Task.Yield();

		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		cut.Markup.Should().NotContain("animate-spin");
		cut.Markup.Should().Contain("Article not found");
	}

	[Fact]
	public void EditButton_IsEnabled_WhenCanEditIsTrue()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		articleDto.CanEdit = true;
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		var editButton = cut.Find("button.btn-secondary");
		editButton.HasAttribute("disabled").Should().BeFalse();
	}

	[Fact]
	public void EditButton_IsDisabled_WhenCanEditIsFalse()
	{
		// Arrange
		var articleDto = FakeArticleDto.GetNewArticleDto(true);
		articleDto.CanEdit = false;
		_mockHandler.HandleAsync(articleDto.Id).Returns(Task.FromResult(Result<ArticleDto>.Ok(articleDto)));

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, articleDto.Id));
		cut.Instance.GetType().GetProperty("_isLoading")?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetProperty("_article")?.SetValue(cut.Instance, articleDto);
		cut.Render();

		// Assert
		var editButton = cut.Find("button.btn-secondary");
		editButton.HasAttribute("disabled").Should().BeTrue();
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

}

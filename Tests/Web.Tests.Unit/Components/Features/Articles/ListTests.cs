// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ListTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using static Web.Components.Features.Articles.ArticleList.GetArticles;

namespace Web.Components.Features.Articles;

/// <summary>
///   Unit tests for <see cref="List" /> (Articles List).
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(List))]
public class ListTests : BunitContext
{
	private readonly IValidator<ArticleDto> _articleDtoValidator = Substitute.For<IValidator<ArticleDto>>();
	private readonly IGetArticlesHandler _mockHandler;

	public ListTests()
	{
		_mockHandler = Substitute.For<IGetArticlesHandler>();
		Services.AddScoped(_ => _mockHandler);
		Services.AddScoped<ILogger<List>>(_ => Substitute.For<ILogger<List>>());
		Services.AddScoped(_ => _articleDtoValidator);
		Services.AddCascadingAuthenticationState();
		Services.AddAuthorization();

		TestServiceRegistrations.RegisterCommonUtilities(this);

	}

	// Helper to set up handler return for articles
	private void SetupHandlerArticles(IEnumerable<ArticleDto>? articles, bool success = true, string? error = null)
	{
		_mockHandler.HandleAsync().Returns(success
				? Task.FromResult(Result<IEnumerable<ArticleDto>>.Ok(articles ?? new List<ArticleDto>()))
				: Task.FromResult(Result<IEnumerable<ArticleDto>>.Fail(error ?? "Error")));
	}

	[Fact]
	public void Renders_Articles_List()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var articles = FakeArticleDto.GetArticleDtos(2, true);
		SetupHandlerArticles(articles);
		// Act
		var cut = Render<List>();
		cut.Markup.Should().Contain(articles[0].Title);
		cut.Markup.Should().Contain(articles[1].Title);
	}

	[Fact]
	public void Renders_Loading_State()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerArticles(new List<ArticleDto>());
		var cut = Render<List>();
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, true);
		cut.Render();
		cut.Markup.Should().Contain("Loading");
	}

	[Fact]
	public void Renders_Empty_State_When_No_Articles()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerArticles(null);
		var cut = Render<List>();
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Render();
		cut.Markup.Should().Contain("No articles available");
	}

	[Fact]
	public void Renders_Articles_List_With_Data()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var articles = FakeArticleDto.GetArticleDtos(2, true);
		SetupHandlerArticles(articles);
		var cut = Render<List>();
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_articles", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, articles.AsQueryable());
		cut.Render();
		foreach (var article in articles)
		{
			cut.Markup.Should().Contain(article.Title);
			cut.Markup.Should().Contain(article.Author.UserName);
		}
	}

	[Fact]
	public void Navigates_To_Create_New_Article()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		var nav = Services.GetRequiredService<BunitNavigationManager>();
		var articles = FakeArticleDto.GetArticleDtos(2, true);
		SetupHandlerArticles(articles);
		var cut = Render<List>();
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);

		cut.Render();
		cut.Find("button.btn-success").Click();
		nav.Uri.Should().EndWith("/articles/create");
	}

	[Fact]
	public void Renders_NotFound_When_Article_Is_Null()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerArticles(null);
		var cut = Render<List>();

		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, false);

		// Act
		cut.Render();

		// Assert
		cut.Markup.Should().Contain("No articles available");
	}

	[Fact]
	public void Edit_Button_Disabled_For_User_Role()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		var articles = FakeArticleDto.GetArticleDtos(2, true);
		SetupHandlerArticles(articles);

		// Act
		var cut = Render<List>();
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_articles", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, articles.AsQueryable());
		cut.Render();

		// Assert
		foreach (var button in cut.FindAll("button.btn-primary"))
		{
			button.HasAttribute("disabled").Should().BeTrue();
		}
	}

	[Fact]
	public void Renders_Error_State_When_Handler_Fails()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin", "Author");
		SetupHandlerArticles(null, success: false, error: "Failed to load articles");
		var cut = Render<List>();
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Render();
		// The component shows the empty-state markup when the handler fails (no articles available)
		cut.Markup.Should().Contain("No articles available");
	}

	[Fact]
	public void Edit_Button_Enabled_For_Admin_Or_Author()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var articles = FakeArticleDto.GetArticleDtos(2, true);
		foreach (var article in articles) article.CanEdit = true;
		SetupHandlerArticles(articles);
		var cut = Render<List>();
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_articles", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, articles.AsQueryable());
		cut.Render();
		foreach (var button in cut.FindAll("button.btn-primary"))
		{
			button.HasAttribute("disabled").Should().BeFalse();
		}
	}

	[Fact]
	public void Navigates_To_Details_Page()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var nav = Services.GetRequiredService<BunitNavigationManager>();
		var articles = FakeArticleDto.GetArticleDtos(1, true);
		SetupHandlerArticles(articles);
		var cut = Render<List>();
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_articles", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, articles.AsQueryable());
		cut.Render();
		cut.Find("button.btn-info").Click();
		nav.Uri.Should().Contain($"/articles/details/{articles[0].Id}");
	}

	[Fact]
	public void Renders_All_Table_Columns()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var articles = FakeArticleDto.GetArticleDtos(1, true);
		SetupHandlerArticles(articles);
		var cut = Render<List>();
		cut.Instance.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, false);
		cut.Instance.GetType().GetField("_articles", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, articles.AsQueryable());
		cut.Render();
		var expectedHeaders = new[] { "Title", "Release Date", "Content", "Author", "Category", "Created On", "Modified On", "Published", "Published On", "Archived", "Actions" };
		foreach (var header in expectedHeaders)
		{
			cut.Markup.Should().Contain(header);
		}
	}

	[Fact(Skip = "Bunit does not enforce [Authorize]; page is always rendered in test context.")]
	public void Only_Authorized_Users_Can_Access()
	{
		// Arrange
		Helpers.SetAuthorization(this, false);
		var cut = Render<List>();
		// Assert
		cut.FindAll("table").Should().BeEmpty();
	}

}
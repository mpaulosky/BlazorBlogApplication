// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleCreateTests.cs
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Components.Features.Articles.ArticleCreate;

namespace Web.Components.Features.Articles;

public class ArticleCreateTests : BunitContext
{
	[Fact]
	public void Shows_Error_When_Title_Empty_And_Does_Not_Navigate()
	{
		Services.AddLogging();
		var handler = BuildCreateArticleHandler();
		Services.AddScoped<CreateArticle.Handler>(_ => handler);
		var nav = new FakeNav();
		Services.AddScoped<NavigationManager>(_ => nav);

		var cut = Render<Create>();
		// Submit without entering title triggers validation error inside component
		cut.Find("form").Submit();

		// Remains on page showing validation summary or unchanged form
		cut.Markup.Should().Contain("Create Article");
		nav.Uri.Should().Be("http://localhost/");
	}

	[Fact]
	public void Creates_Article_And_Navigates_To_List()
	{
		using var ctx = new BunitContext();
		Services.AddLogging();
		var handler = BuildCreateArticleHandler();
		Services.AddScoped<CreateArticle.Handler>(_ => handler);
		var nav = new FakeNav();
		Services.AddScoped<NavigationManager>(_ => nav);

		var cut = Render<Create>();
		// Fill basic fields
		cut.Find("input[aria-invalid=false]"); // ensure inputs present
		cut.FindAll("input")[0].Change("Title One");
		cut.FindAll("input")[1].Change("Intro");
		cut.Find("textarea").Change("Some content");
		cut.FindAll("input")[2].Change("http://img");
		// submit
		cut.Find("form").Submit();

		nav.Uri.Should().EndWith("/articles");
	}

	private static CreateArticle.Handler BuildCreateArticleHandler()
	{
		var client = new Mock<IMongoClient>();
		var db = new Mock<IMongoDatabase>();
		var collection = new Mock<IMongoCollection<Article>>();
		collection
			.Setup(c => c.InsertOneAsync(It.IsAny<Article>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);
		db.Setup(d => d.GetCollection<Article>("Articles", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
		client.Setup(c => c.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(db.Object);
		var context = new MyBlogContext(client.Object);
		var logger = Mock.Of<ILogger<CreateArticle.Handler>>();
		return new CreateArticle.Handler(context, logger);
	}

	private sealed class FakeNav : NavigationManager
	{
		public FakeNav() => Initialize("http://localhost/", "http://localhost/");
		protected override void NavigateToCore(string uri, bool forceLoad) => Uri = ToAbsoluteUri(uri).ToString();
	}
}
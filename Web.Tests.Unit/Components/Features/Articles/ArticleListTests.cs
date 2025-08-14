// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleListTests.cs
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Articles;

public class ArticleListTests
{
	[Fact]
	public void Shows_NoArticles_Message_When_Handler_Finds_None()
	{
		using var ctx = new BunitContext();
		ctx.Services.AddLogging();
		var handler = BuildArticlesHandlerWithData([]); // the empty list simulates none found => warning and fail
		ctx.Services.AddScoped<GetArticles.Handler>(_ => handler);
		ctx.Services.AddScoped<NavigationManager, FakeNav>();

		var cut = ctx.Render<Web.Components.Features.Articles.ArticleList.List>();

		cut.Markup.Should().Contain("No articles available.");
	}

	[Fact]
	public void Renders_Grid_When_Articles_Returned_And_Navigate_Create_On_Click()
	{
		using var ctx = new BunitContext();
		ctx.Services.AddLogging();
		var articles = new List<ArticleDto>
		{
			new ArticleDto
			{
				Id = ObjectId.GenerateNewId(),
				Title = "A",
				Introduction = "Intro",
				Content = "Content",
				CoverImageUrl = "http://img",
				UrlSlug = "a",
				Author = new AppUserDto { Id = "1", UserName = "User", Email = "u@example.com" },
				Category = new CategoryDto { Id = ObjectId.GenerateNewId(), CategoryName = "Cat", CreatedOn = System.DateTime.UtcNow },
				CreatedOn = System.DateTime.UtcNow
			}
		};
		var handler = BuildArticlesHandlerWithData(articles);
		ctx.Services.AddScoped<GetArticles.Handler>(_ => handler);
		var nav = new FakeNav();
		ctx.Services.AddScoped<NavigationManager>(_ => nav);

		var cut = ctx.Render<Web.Components.Features.Articles.ArticleList.List>();

		cut.Markup.Should().Contain("QuickGrid");
		cut.FindAll("button").First(b => b.TextContent.Contains("Create New Article")).Click();
		nav.Uri.Should().EndWith("/articles/create");
	}

	private static GetArticles.Handler BuildArticlesHandlerWithData(IEnumerable<ArticleDto> data)
	{
		// Build a mocked Mongo pipeline that returns data
		var clientMock = new Mock<IMongoClient>();
		var dbMock = new Mock<IMongoDatabase>();
		var collectionMock = new Mock<IMongoCollection<Article>>();
		var cursorMock = new Mock<IAsyncCursor<Article>>();

		var entities = data.Select(d => new Article(
				d.Title ?? string.Empty,
				d.Introduction ?? string.Empty,
				d.Content ?? string.Empty,
				d.CoverImageUrl ?? string.Empty,
				d.UrlSlug ?? string.Empty,
				new AppUserDto { Id = d.Author.Id, UserName = d.Author.UserName, Email = d.Author.Email },
				new CategoryDto { Id = d.Category.Id, CategoryName = d.Category.CategoryName, CreatedOn = d.Category.CreatedOn },
				d.IsPublished,
				d.PublishedOn,
				skipValidation: true)
			{
				Archived = d.Archived
			}).ToList();

		var batchQueue = new Queue<bool>([entities.Count > 0, false]);
		cursorMock.SetupSequence(c => c.MoveNext(It.IsAny<System.Threading.CancellationToken>()))
			.Returns(() => batchQueue.Dequeue());
		cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<System.Threading.CancellationToken>()))
			.ReturnsAsync(() => batchQueue.Count == 0 ? false : true);
		cursorMock.SetupGet(c => c.Current).Returns(entities);

		collectionMock
			.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Article>>(), It.IsAny<FindOptions<Article, Article>>(), It.IsAny<System.Threading.CancellationToken>()))
			.ReturnsAsync(cursorMock.Object);

		dbMock.Setup(d => d.GetCollection<Article>("Articles", It.IsAny<MongoCollectionSettings>()))
			.Returns(collectionMock.Object);

		clientMock.Setup(c => c.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
			.Returns(dbMock.Object);

		var context = new MyBlogContext(clientMock.Object);
		var logger = Mock.Of<ILogger<GetArticles.Handler>>();
		return new GetArticles.Handler(context, logger);
	}

	private sealed class FakeNav : NavigationManager
	{
		public bool Forced { get; private set; }
		public FakeNav() => Initialize("http://localhost/", "http://localhost/");
		protected override void NavigateToCore(string uri, bool forceLoad)
		{
			Forced = forceLoad;
			Uri = ToAbsoluteUri(uri).ToString();
		}
	}
}
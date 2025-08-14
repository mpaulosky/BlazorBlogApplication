// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryCreateTests.cs
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Components.Features.Categories.CategoryCreate;

namespace Web.Components.Features.Categories;

public class CategoryCreateTests
{
	[Fact]
	public void Shows_Error_When_Name_Empty_And_Does_Not_Navigate()
	{
		using var ctx = new BunitContext();
		ctx.Services.AddLogging();
		// Set up context and handler with insert succeeding (won't be called due to validation)
		var handler = BuildCreateCategoryHandler();
		ctx.Services.AddScoped<CreateCategory.Handler>(_ => handler);
		var nav = new FakeNav();
		ctx.Services.AddScoped<NavigationManager>(_ => nav);

		var cut = ctx.Render<Web.Components.Features.Categories.CategoryCreate.Create>();
		// Ensure default input empty, click submit
		var form = cut.Find("form");
		form.Submit();

		cut.Markup.Should().Contain("CategoryName cannot be empty.");
		nav.Uri.Should().Be("http://localhost/");
	}

	[Fact]
	public void Creates_Category_And_Navigates_To_List()
	{
		using var ctx = new BunitContext();
		ctx.Services.AddLogging();
		var handler = BuildCreateCategoryHandler();
		ctx.Services.AddScoped<CreateCategory.Handler>(_ => handler);
		var nav = new FakeNav();
		ctx.Services.AddScoped<NavigationManager>(_ => nav);

		var cut = ctx.Render<Web.Components.Features.Categories.CategoryCreate.Create>();
		// Fill in name
		cut.Find("input#name").Change("New Cat");
		cut.Find("form").Submit();

		nav.Uri.Should().EndWith("/categories");
	}

	private static CreateCategory.Handler BuildCreateCategoryHandler()
	{
		var client = new Mock<IMongoClient>();
		var db = new Mock<IMongoDatabase>();
		var collection = new Mock<IMongoCollection<Category>>();
		collection
			.Setup(c => c.InsertOneAsync(It.IsAny<Category>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);
		db.Setup(d => d.GetCollection<Category>("Categories", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
		client.Setup(c => c.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(db.Object);
		var context = new MyBlogContext(client.Object);
		var logger = Mock.Of<ILogger<CreateCategory.Handler>>();
		return new CreateCategory.Handler(context, logger);
	}

	private sealed class FakeNav : NavigationManager
	{
		public FakeNav() => Initialize("http://localhost/", "http://localhost/");
		protected override void NavigateToCore(string uri, bool forceLoad) => Uri = ToAbsoluteUri(uri).ToString();
	}
}
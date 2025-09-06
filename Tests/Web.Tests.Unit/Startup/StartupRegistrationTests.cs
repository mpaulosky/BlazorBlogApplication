// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     StartupRegistrationTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class StartupRegistrationTests : IClassFixture<TestWebApplicationFactory>
{

	private readonly TestWebApplicationFactory _factory;

	public StartupRegistrationTests(TestWebApplicationFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public void Validators_Are_Registered()
	{
		using var scope = _factory.Services.CreateScope();
		var sp = scope.ServiceProvider;

		sp.GetRequiredService<IValidator<ArticleDto>>()
				.Should().BeOfType<ArticleDtoValidator>();

		sp.GetRequiredService<IValidator<CategoryDto>>()
				.Should().BeOfType<CategoryDtoValidator>();
	}

	[Fact]
	public void Feature_Handlers_Are_Registered()
	{
		using var scope = _factory.Services.CreateScope();
		var sp = scope.ServiceProvider;

		sp.GetRequiredService<GetArticles.Handler>().Should().NotBeNull();
		sp.GetRequiredService<GetCategories.Handler>().Should().NotBeNull();
		sp.GetRequiredService<GetArticle.IGetArticleHandler>().Should().BeOfType<GetArticle.Handler>();
		sp.GetRequiredService<EditArticle.IEditArticleHandler>().Should().BeOfType<EditArticle.Handler>();
		sp.GetRequiredService<GetArticles.IGetArticlesHandler>().Should().BeOfType<GetArticles.Handler>();
		sp.GetRequiredService<CreateArticle.ICreateArticleHandler>().Should().BeOfType<CreateArticle.Handler>();
		sp.GetRequiredService<EditCategory.IEditCategoryHandler>().Should().BeOfType<EditCategory.Handler>();
		sp.GetRequiredService<GetCategory.IGetCategoryHandler>().Should().BeOfType<GetCategory.Handler>();
	}

	[Fact]
	public void Mongo_Registrations_And_Lifetimes_Are_Correct()
	{
		using var scopeRoot = _factory.Services.CreateScope();
		var spRoot = scopeRoot.ServiceProvider;

		// IMongoClient is a singleton
		var client1 = spRoot.GetRequiredService<IMongoClient>();
		var client2 = spRoot.GetRequiredService<IMongoClient>();
		client1.Should().BeSameAs(client2);

		// IMyBlogContext is scoped: same within a scope, different across scopes
		var ctxFactory = spRoot.GetRequiredService<IMyBlogContextFactory>();
		ctxFactory.Should().NotBeNull();

		var ctx1A = spRoot.GetRequiredService<IMyBlogContext>();
		var ctx1B = spRoot.GetRequiredService<IMyBlogContext>();
		ctx1A.Should().BeSameAs(ctx1B);

		using var scope2 = _factory.Services.CreateScope();
		var sp2 = scope2.ServiceProvider;
		var ctx2 = sp2.GetRequiredService<IMyBlogContext>();
		ctx2.Should().NotBeSameAs(ctx1A);
	}

}

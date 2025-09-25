// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     RegisterServicesCollections.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Extensions;

/// <summary>
///   IServiceCollectionExtensions
/// </summary>
public static partial class ServiceCollectionExtensions
{

	/// <summary>
	///   Register DI Collections
	/// </summary>
	/// <param name="services">IServiceCollection</param>
	public static void RegisterServicesCollections(this IServiceCollection services)
	{

		services.AddScoped<GetArticle.IGetArticleHandler, GetArticle.Handler>();
		services.AddScoped<EditArticle.IEditArticleHandler, EditArticle.Handler>();
		services.AddScoped<GetArticles.IGetArticlesHandler, GetArticles.Handler>();
		services.AddScoped<CreateArticle.ICreateArticleHandler, CreateArticle.Handler>();
		services.AddScoped<CreateCategory.ICreateCategoryHandler, CreateCategory.Handler>();
		services.AddScoped<GetCategories.IGetCategoriesHandler, GetCategories.Handler>();
		services.AddScoped<EditCategory.IEditCategoryHandler, EditCategory.Handler>();
		services.AddScoped<GetCategory.IGetCategoryHandler, GetCategory.Handler>();

		// Also register concrete handlers for tests that resolve by concrete type
		services.AddScoped<GetArticles.Handler>();
		services.AddScoped<GetCategories.Handler>();

		// Register FluentValidation validators for Blazor forms
		services.AddScoped<IValidator<ArticleDto>, ArticleDtoValidator>();
		services.AddScoped<IValidator<CategoryDto>, CategoryDtoValidator>();

	}

}
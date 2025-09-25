// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateArticle.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================
namespace Web.Components.Features.Articles.ArticleCreate;

/// <summary>
///   Static class providing functionality for category creation.
/// </summary>
public static class CreateArticle
{

	/// <summary>
	///   Interface for creating articles in the database.
	/// </summary>
	public interface ICreateArticleHandler
	{

		Task<Result> HandleAsync(ArticleDto? request);

	}

	/// <summary>
	///   Represents a handler for creating new categories in the database.
	/// </summary>
	public class Handler : ICreateArticleHandler
	{

		private readonly IApplicationDbContextFactory _factory;

		private readonly ILogger<Handler> _logger;

		/// <summary>
		///   Initializes a new instance of the <see cref="Handler" /> class.
		/// </summary>
		/// <param name="factory">The context factory.</param>
		/// <param name="logger">The logger instance.</param>
		public Handler(IApplicationDbContextFactory factory, ILogger<Handler> logger)
		{
			_factory = factory;
			_logger = logger;
		}

		/// <summary>
		///   Handles the creation of a new category asynchronously.
		/// </summary>
		/// <param name="request">The category DTO.</param>
		/// <returns>A <see cref="Result" /> indicating success or failure.</returns>
		public async Task<Result> HandleAsync(ArticleDto? request)
		{

			if (request is null)
			{
				return Result.Fail("The request is null.");
			}

			try
			{
				ApplicationDbContext context = _factory.CreateDbContext();

				// Manually map DTO to an entity to avoid relying on Mapster in unit tests
				Article article = new (
						request.Title,
						request.Introduction,
						request.Content,
						request.CoverImageUrl,
						request.UrlSlug,
						request.Author.Id,
						request.Category.Id,
						request.IsPublished,
						request.PublishedOn,
						request.IsArchived);

				context.Articles.Add(article);
				await context.SaveChangesAsync();

				_logger.LogInformation("Article created successfully: {Title}", request.Title);

				return Result.Ok();
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Failed to create article");

				return Result.Fail("An error occurred while creating the article: " + ex.Message);

			}

		}

	}

}

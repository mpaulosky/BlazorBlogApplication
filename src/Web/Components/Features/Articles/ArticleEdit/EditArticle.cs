// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditArticle.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Components.Features.Articles.ArticleEdit;

/// <summary>
///   Contains functionality for editing an article within the system.
/// </summary>
public static class EditArticle
{

	/// <summary>
	///   Interface for editing articles in the database.
	/// </summary>
	public interface IEditArticleHandler
	{

		Task<Result> HandleAsync(ArticleDto? request);

	}

	public class Handler : IEditArticleHandler
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

		public async Task<Result> HandleAsync(ArticleDto? request)
		{

			if (request is null)
			{

				_logger.LogError("The request is null.");

				return Result.Fail("The request is null.");

			}

			try
			{

				var context = _factory.CreateDbContext();

				// First check if the article exists
				var existingArticle = await context.Articles.FirstOrDefaultAsync(x => x.Id == request.Id);

				if (existingArticle is null)
				{
					_logger.LogWarning("Article not found for update: {ArticleId}", request.Id);
					return Result.Fail("Article not found.");
				}

				// Update the existing article
				existingArticle.Title = request.Title;
				existingArticle.Introduction = request.Introduction;
				existingArticle.Content = request.Content;
				existingArticle.CoverImageUrl = request.CoverImageUrl;
				existingArticle.UrlSlug = request.UrlSlug;
				existingArticle.AuthorId = request.Author.Id;
				existingArticle.CategoryId = request.Category.Id;
				existingArticle.IsPublished = request.IsPublished;
				existingArticle.PublishedOn = request.PublishedOn;
				existingArticle.IsArchived = request.IsArchived;
				existingArticle.ModifiedOn = DateTime.UtcNow;

				await context.SaveChangesAsync();

				_logger.LogInformation("Article updated successfully: {Title}", request.Title);

				return Result.Ok();

			}
			catch (Exception ex)
			{

				// Avoid dereferencing the request in the error path (it may be null).
				_logger.LogError(ex, "Failed to update article: {Title}", request?.Title ?? "Unknown");

				return Result.Fail(ex.Message);

			}

		}

	}


}
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

		private readonly IMyBlogContextFactory _factory;

		private readonly ILogger<Handler> _logger;

		/// <summary>
		///   Initializes a new instance of the <see cref="Handler" /> class.
		/// </summary>
		/// <param name="factory">The context factory.</param>
		/// <param name="logger">The logger instance.</param>
		public Handler(IMyBlogContextFactory factory, ILogger<Handler> logger)
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

				var context = await _factory.CreateContext(CancellationToken.None);

				var category = new Article
				{
						Title = request.Title,
						Introduction = request.Introduction,
						Content = request.Content,
						CoverImageUrl = request.CoverImageUrl,
						UrlSlug = request.UrlSlug,
						Author = request.Author,
						Category = request.Category,
						IsPublished = request.IsPublished,
						PublishedOn = request.PublishedOn,
						IsArchived = request.IsArchived,
						ModifiedOn = DateTime.UtcNow
				};

				await context.Articles.ReplaceOneAsync(
						Builders<Article>.Filter.Eq(x => x.Id, request.Id),
						category,
						new ReplaceOptions { IsUpsert = false }
				);

				_logger.LogInformation("Article updated successfully: {Title}", request.Title);

				return Result.Ok();

			}
			catch (Exception ex)
			{

				// Avoid dereferencing the request in the error path (it may be null).
				_logger.LogError(ex, "Failed to update category: {Title}", request.Title);

				return Result.Fail(ex.Message);

			}

		}

	}


}

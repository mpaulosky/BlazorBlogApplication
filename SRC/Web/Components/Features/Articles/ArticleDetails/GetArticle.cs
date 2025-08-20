// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryGet.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorApp
// Project Name :  Web
// =======================================================

namespace Web.Components.Features.Articles.ArticleDetails;

/// <summary>
/// Static class providing functionality for article creation.
/// </summary>
public static class GetArticle
{
	/// <summary>
	/// Interface for retrieving articles from the database.
	/// </summary>
	public interface IGetArticleHandler
	{
		Task<Result<ArticleDto>> HandleAsync(ObjectId id);
	}

	/// <summary>
	/// Represents a handler for retrieving articles from the database.
	/// </summary>
	public class Handler : IGetArticleHandler
	{
		private readonly IMyBlogContext _context;
		private readonly ILogger<Handler> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="Handler"/> class.
		/// </summary>
		/// <param name="context">The database context.</param>
		/// <param name="logger">The logger instance.</param>
		public Handler(IMyBlogContext context, ILogger<Handler> logger)
		{
			_context = context;
			_logger = logger;
		}

		/// <summary>
		/// Handles retrieving an article asynchronously by its ObjectId.
		/// </summary>
		/// <param name="id">The article ObjectId.</param>
		/// <returns>A <see cref="Result"/> representing the outcome of the operation.</returns>
		public async Task<Result<ArticleDto>> HandleAsync(ObjectId id)
		{
			try
			{
				if (id == ObjectId.Empty)
				{
					_logger.LogError("The ID is empty.");
					return Result.Fail<ArticleDto>("The ID cannot be empty.");
				}

				var filter = Builders<Article>.Filter.Eq("_id", id);
				var cursor = await _context.Articles.FindAsync(filter);
				var article = await cursor.FirstOrDefaultAsync();

				if (article is null)
				{
					_logger.LogWarning("Article not found: {CategoryId}", id);
					return Result.Fail<ArticleDto>("Article not found.");
				}

				_logger.LogInformation("Article was found successfully: {CategoryId}", id);
				var dto = ArticleDto.FromEntity(article);
				return Result<ArticleDto>.Ok(dto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find the article: {CategoryId}", id);
				return Result<ArticleDto>.Fail(ex.Message);
			}
		}
	}

}
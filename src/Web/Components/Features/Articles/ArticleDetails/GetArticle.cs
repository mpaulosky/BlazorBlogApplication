// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetArticle.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Components.Features.Articles.ArticleDetails;

/// <summary>
///   Static class providing functionality for article creation.
/// </summary>
public static class GetArticle
{

	/// <summary>
	///   Interface for retrieving articles from the database.
	/// </summary>
	public interface IGetArticleHandler
	{

		Task<Result<ArticleDto>> HandleAsync(Guid id);

	}

	/// <summary>
	///   Represents a handler for retrieving articles from the database.
	/// </summary>
	public class Handler : IGetArticleHandler
	{

		private readonly IArticleDbContextFactory _factory;

		private readonly ILogger<Handler> _logger;

		/// <summary>
		///   Initializes a new instance of the <see cref="Handler" /> class.
		/// </summary>
		/// <param name="factory">The context factory.</param>
		/// <param name="logger">The logger instance.</param>
		public Handler(IArticleDbContextFactory factory, ILogger<Handler> logger)
		{
			_factory = factory;
			_logger = logger;
		}

		/// <summary>
		///   Handles retrieving an article asynchronously by its ObjectId.
		/// </summary>
		/// <param name="id">The article ObjectId.</param>
		/// <returns>A <see cref="Result" /> representing the outcome of the operation.</returns>
		public async Task<Result<ArticleDto>> HandleAsync(Guid id)
		{

			if (id == Guid.Empty)
			{

				_logger.LogError("The ID is empty.");

				return Result.Fail<ArticleDto>("The ID cannot be empty.");
			}

			try
			{

				using var context = _factory.CreateDbContext();

				var article = await context.Articles
					.Include(a => a.Author)
					.Include(a => a.Category)
					.FirstOrDefaultAsync(a => a.Id == id);

				if (article is null)
				{
					_logger.LogWarning("Article not found: {ArticleId}", id);

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

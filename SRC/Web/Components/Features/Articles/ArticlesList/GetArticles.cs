// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetArticles.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Components.Features.Articles.ArticlesList;

/// <summary>
///   Static class providing functionality for article creation.
/// </summary>
public static class GetArticles
{

	/// <summary>
	///   Interface for retrieving articles from the database.
	/// </summary>
	public interface IGetArticlesHandler
	{

		Task<Result<IEnumerable<ArticleDto>>> HandleAsync(bool excludeArchived = false);

	}

	/// <summary>
	///   Represents a handler for retrieving articles from the database.
	/// </summary>
	public class Handler : IGetArticlesHandler
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

		/// <summary>
		///   Handles retrieving all articles asynchronously, with an option to exclude archived articles.
		/// </summary>
		/// <param name="excludeArchived">If true, excludes articles where Archived is true.</param>
		/// <returns>A <see cref="Result" /> representing the outcome of the operation.</returns>
		public async Task<Result<IEnumerable<ArticleDto>>> HandleAsync(bool excludeArchived = false)
		{
			try
			{

				var context = await _factory.CreateContext(CancellationToken.None);

				var filter = excludeArchived
						? Builders<Article>.Filter.Eq(x => x.Archived, false)
						: Builders<Article>.Filter.Empty;

				var articlesCursor = await context.Articles.FindAsync(filter);
				var articles = await articlesCursor.ToListAsync();

				if (articles is null || articles.Count == 0)
				{
					_logger.LogWarning("No articles found.");

					return Result<IEnumerable<ArticleDto>>.Fail("No articles found.");
				}

				_logger.LogInformation("Categories retrieved successfully. Count: {Count}", articles.Count);

				return Result<IEnumerable<ArticleDto>>.Ok(articles.Adapt<IEnumerable<ArticleDto>>());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to retrieve articles.");

				return Result<IEnumerable<ArticleDto>>.Fail(ex.Message);
			}
		}

	}

}

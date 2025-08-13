// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryGet.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorApp
// Project Name :  Web
// =======================================================

using Mapster;

using Web.Data.Abstractions;
using Web.Data.Entities;
using Web.Data.Models;

namespace Web.Components.Features.Articles.ArticleList;

/// <summary>
/// Static class providing functionality for article creation.
/// </summary>
public static class GetArticles
{
	/// <summary>
	/// Represents a handler for retrieving articles from the database.
	/// </summary>
	public class Handler
	{
		private readonly MyBlogContext _context;
		private readonly ILogger<Handler> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="Handler"/> class.
		/// </summary>
		/// <param name="context">The database context.</param>
		/// <param name="logger">The logger instance.</param>
		public Handler(MyBlogContext context, ILogger<Handler> logger)
		{
			_context = context;
			_logger = logger;
		}

		/// <summary>
		/// Handles retrieving all articles asynchronously, with an option to exclude archived articles.
		/// </summary>
		/// <param name="excludeArchived">If true, excludes articles where Archived is true.</param>
		/// <returns>A <see cref="Result"/> representing the outcome of the operation.</returns>
		public async Task<Result<IEnumerable<ArticleDto>>> HandleAsync(bool excludeArchived = false)
		{
			try
			{
				var filter = excludeArchived
					? Builders<Article>.Filter.Eq(x => x.Archived, false)
					: Builders<Article>.Filter.Empty;

				var articlesCursor = await _context.Articles.FindAsync(filter);
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
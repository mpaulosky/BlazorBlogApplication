// =======================================================
// Copyright (c) 2025. All rights reserved.
// File CategoryName :     CategoryGet.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution CategoryName : BlazorApp
// Project CategoryName :  Web
// =======================================================

using Web.Data.Abstractions;
using Web.Data.Entities;
using Web.Data.Models;

namespace Web.Components.Features.Articles.ArticleCreate;

/// <summary>
/// Static class providing functionality for category creation.
/// </summary>
public static class CreateArticle
{

	/// <summary>
	/// Represents a handler for creating new categories in the database.
	/// </summary>
	public class Handler
	{

		private readonly MyBlogContext _context;
		private readonly ILogger<Handler> _logger;

		/// <summary>
		///   Initializes a new instance of the <see cref="Handler"/> class.
		/// </summary>
		/// <param name="context">The database context.</param>
		/// <param name="logger">The logger instance.</param>
		public Handler(MyBlogContext context, ILogger<Handler> logger)
		{
			_context = context;
			_logger = logger;
		}

		/// <summary>
		///   Handles the creation of a new category asynchronously.
		/// </summary>
		/// <param name="request">The category DTO.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public async Task<Result> HandleAsync(ArticleDto request)
		{
			try
			{
				var article = new Article
				{
					Title = request.Title,
					Introduction = request.Introduction,
					Content = request.Content,
					CoverImageUrl = request.CoverImageUrl,
					UrlSlug = request.UrlSlug,
					Author = request.Author,
					Category = request.Category,
					IsPublished = request.IsPublished,
					PublishedOn = request.PublishedOn ?? DateTimeOffset.UtcNow,
					Archived = request.Archived
				};

				await _context.Articles.InsertOneAsync(article);

				_logger.LogInformation("Category created successfully: {Title}", request.Title);

				return Result.Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create category: {Title}", request.Title);
				return Result.Fail(ex.Message);
			}
		}
	}

}
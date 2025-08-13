// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryEdit.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorApp
// Project Name :  Web
// =======================================================

using Web.Data.Abstractions;
using Web.Data.Entities;
using Web.Data.Models;

namespace Web.Components.Features.Articles.ArticleEdit;

/// <summary>
/// Contains functionality for editing an article within the system.
/// </summary>
public static class EditArticle
{

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

		public async Task<Result> HandleAsync(ArticleDto request)
		{

			try
			{

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
					PublishedOn = request.PublishedOn ?? DateTimeOffset.UtcNow,
					Archived = request.Archived,
					ModifiedOn = DateTimeOffset.UtcNow
				};

				await _context.Articles.ReplaceOneAsync(
						Builders<Article>.Filter.Eq(x => x.Id, request.Id),
						category,
						new ReplaceOptions { IsUpsert = false }
				);

				_logger.LogInformation("Article updated successfully: {Title}", request.Title);

				return Result.Ok();

			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Failed to update category: {Title}", request.Title);

				return Result.Fail(ex.Message);

			}

		}

	}


}
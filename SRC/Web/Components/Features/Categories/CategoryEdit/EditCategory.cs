// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryEdit.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorApp
// Project Name :  Web
// =======================================================

namespace Web.Components.Features.Categories.CategoryEdit;

/// <summary>
/// Contains functionality for editing a category within the system.
/// </summary>
public static class EditCategory
{
	public interface IEditCategoryHandler
	{
		Task<Result> HandleAsync(CategoryDto request);
	}

	public class Handler : IEditCategoryHandler
	{

		private readonly IMyBlogContextFactory _contextFactory;
		private readonly ILogger<Handler> _logger;

		/// <summary>
		///   Initializes a new instance of the <see cref="Handler"/> class.
		/// </summary>
		/// <param name="context">The database context.</param>
		/// <param name="logger">The logger instance.</param>
		public Handler(IMyBlogContextFactory contextFactory, ILogger<Handler> logger)
		{
			_contextFactory = contextFactory;
			_logger = logger;
		}

		public async Task<Result> HandleAsync(CategoryDto request)
		{

			try
			{

				var category = new Category
				{
					CategoryName = request.CategoryName,
					ModifiedOn = DateTime.UtcNow,
				};

				var context = await _contextFactory.CreateAsync();
				await context.Categories.ReplaceOneAsync(
					Builders<Category>.Filter.Eq(x => x.Id, request.Id),
					category,
					new ReplaceOptions { IsUpsert = false }
				);

				_logger.LogInformation("Category updated successfully: {CategoryName}", request.CategoryName);

				return Result.Ok();

			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Failed to update category: {CategoryName}", request.CategoryName);

				return Result.Fail(ex.Message);

			}

		}

	}


}

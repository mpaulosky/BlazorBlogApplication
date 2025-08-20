// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryGet.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorApp
// Project Name :  Web
// =======================================================

namespace Web.Components.Features.Categories.CategoryDetails;

/// <summary>
/// Static class providing functionality for category creation.
/// </summary>
public static class GetCategory
{
	public interface IGetCategoryHandler
	{
		Task<Result<CategoryDto>> HandleAsync(ObjectId id);
	}

	/// <summary>
	/// Represents a handler for retrieving categories from the database.
	/// </summary>
	public class Handler : IGetCategoryHandler
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
		/// Handles retrieving a category asynchronously by its ObjectId.
		/// </summary>
		/// <param name="id">The category ObjectId.</param>
		/// <returns>A <see cref="Result"/> representing the outcome of the operation.</returns>
		public async Task<Result<CategoryDto>> HandleAsync(ObjectId id)
		{

			try
			{

				if (id == ObjectId.Empty)
				{
					_logger.LogError("The ID cannot be empty.");
					return Result.Fail<CategoryDto>("The ID cannot be empty.");
				}

				var filter = Builders<Category>.Filter.Eq("_id", id);
				var cursor = await _context.Categories.FindAsync<Category>(filter);
				var categories = await cursor.ToListAsync();
				var category = categories.FirstOrDefault();

				if (category is null)
				{
					_logger.LogWarning("Category not found: {CategoryId}", id);
					return Result.Fail<CategoryDto>("Category not found.");
				}

				_logger.LogInformation("Category was found successfully: {CategoryId}", id);
				return Result<CategoryDto>.Ok(category.Adapt<CategoryDto>());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find the category: {CategoryId}", id);
				return Result<CategoryDto>.Fail(ex.Message);
			}
		}
	}

}
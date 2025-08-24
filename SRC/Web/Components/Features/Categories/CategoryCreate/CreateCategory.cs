// =======================================================
// Copyright (c) 2025. All rights reserved.
// File CategoryName :     CategoryGet.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution CategoryName : BlazorApp
// Project CategoryName :  Web
// =======================================================

namespace Web.Components.Features.Categories.CategoryCreate;

/// <summary>
/// Static class providing functionality for category creation.
/// </summary>
public static class CreateCategory
{
	public interface ICreateCategoryHandler
	{
		Task<Result> HandleAsync(CategoryDto? request);
	}

	/// <summary>
	/// Represents a handler for creating new categories in the database.
	/// </summary>
	public class Handler : ICreateCategoryHandler
	{

		private readonly IMyBlogContext _context;
		private readonly ILogger<Handler> _logger;

		/// <summary>
		///   Initializes a new instance of the <see cref="Handler"/> class.
		/// </summary>
		/// <param name="context">The database context.</param>
		/// <param name="logger">The logger instance.</param>
		public Handler(IMyBlogContext context, ILogger<Handler> logger)
		{
			_context = context;
			_logger = logger;
		}

		/// <summary>
		///   Handles the creation of a new category asynchronously.
		/// </summary>
		/// <param name="request">The category DTO.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public async Task<Result> HandleAsync(CategoryDto? request)
		{
			try
			{
				var category = new Category
				{
					CategoryName = request!.CategoryName,
				};

				await _context.Categories.InsertOneAsync(category);

				_logger.LogInformation("Category created successfully: {CategoryName}", request.CategoryName);

				return Result.Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create category: {CategoryName}", request?.CategoryName ?? string.Empty);
				return Result.Fail(ex.Message);
			}
		}
	}

}

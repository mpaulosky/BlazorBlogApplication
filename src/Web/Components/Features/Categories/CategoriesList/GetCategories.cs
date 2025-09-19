// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GetCategories.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Components.Features.Categories.CategoriesList;

/// <summary>
///   Static class providing functionality for category creation.
/// </summary>
public static class GetCategories
{

	/// <summary>
	///   Represents a handler for retrieving categories from the database.
	/// </summary>
	public interface IGetCategoriesHandler
	{

		Task<Result<IEnumerable<CategoryDto>>> HandleAsync(bool excludeArchived = false);

	}

	public class Handler : IGetCategoriesHandler
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
		///   Handles retrieving all categories asynchronously, with an option to exclude archived categories.
		/// </summary>
		/// <param name="excludeArchived">If true, excludes categories where Archived is true.</param>
		/// <returns>A <see cref="Result" /> representing the outcome of the operation.</returns>
		public async Task<Result<IEnumerable<CategoryDto>>> HandleAsync(bool excludeArchived = false)
		{
		try
		{

			var context = _factory.CreateDbContext();

			var categories = excludeArchived
					? await context.Categories.Where(x => !x.IsArchived).ToListAsync()
					: await context.Categories.ToListAsync();				if (categories is null || categories.Count == 0)
				{
					_logger.LogWarning("No categories found.");

					return Result<IEnumerable<CategoryDto>>.Fail("No categories found.");
				}

				_logger.LogInformation("Categories retrieved successfully. Count: {Count}", categories.Count);

				return Result<IEnumerable<CategoryDto>>.Ok(categories.Adapt<IEnumerable<CategoryDto>>());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to retrieve categories.");

				return Result<IEnumerable<CategoryDto>>.Fail(ex.Message);
			}
		}

	}

}

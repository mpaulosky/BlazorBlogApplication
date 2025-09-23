// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateCategory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Components.Features.Categories.CategoryCreate;

/// <summary>
///   Static class providing functionality for category creation.
/// </summary>
public static class CreateCategory
{

	public interface ICreateCategoryHandler
	{

		Task<Result> HandleAsync(CategoryDto? request);

	}

	/// <summary>
	///   Represents a handler for creating new categories in the database.
	/// </summary>
	public class Handler : ICreateCategoryHandler
	{

		private readonly IApplicationDbContextFactory _factory;

		private readonly ILogger<Handler> _logger;

		/// <summary>
		///   Initializes a new instance of the <see cref="Handler" /> class.
		/// </summary>
		/// <param name="factory">The context factory.</param>
		/// <param name="logger">The logger instance.</param>
		public Handler(IApplicationDbContextFactory factory, ILogger<Handler> logger)
		{
			_factory = factory;
			_logger = logger;
		}

		/// <summary>
		///   Handles the creation of a new category asynchronously.
		/// </summary>
		/// <param name="request">The category DTO.</param>
		/// <returns>A <see cref="Result" /> indicating success or failure.</returns>
		public async Task<Result> HandleAsync(CategoryDto? request)
		{

			if (request is null)
			{
				_logger.LogError("The request is null.");
				return Result.Fail("The request is null.");
			}

			try
			{

				var context = _factory.CreateDbContext();

				var category = new Category
				{
					CategoryName = request.CategoryName
				};

				context.Categories.Add(category);
				await context.SaveChangesAsync();

				_logger.LogInformation("Category created successfully: {CategoryName}", request.CategoryName);

				return Result.Ok();
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Failed to create category: {CategoryName}", request.CategoryName);

				return Result.Fail("An error occurred while creating the category: " + ex.Message);

			}
		}

	}

}
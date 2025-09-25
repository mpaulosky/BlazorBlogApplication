// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditCategory.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Components.Features.Categories.CategoryEdit;

/// <summary>
///   Contains functionality for editing a category within the system.
/// </summary>
public static class EditCategory
{

	public interface IEditCategoryHandler
	{

		Task<Result> HandleAsync(CategoryDto? request);

	}

	public class Handler : IEditCategoryHandler
	{

		private readonly IApplicationDbContextFactory _contextFactory;

		private readonly ILogger<Handler> _logger;

		/// <summary>
		///   Initializes a new instance of the <see cref="Handler" /> class.
		/// </summary>
		/// <param name="contextFactory"></param>
		/// <param name="logger">The logger instance.</param>
		public Handler(IApplicationDbContextFactory contextFactory, ILogger<Handler> logger)
		{
			_contextFactory = contextFactory;
			_logger = logger;
		}

		public async Task<Result> HandleAsync(CategoryDto? request)
		{

			if (request is null)
			{
				_logger.LogError("The request is null.");

				return Result.Fail("The request is null.");
			}

			if (string.IsNullOrWhiteSpace(request.CategoryName))
			{
				_logger.LogError("Category name cannot be empty or whitespace.");

				return Result.Fail("Category name cannot be empty or whitespace.");
			}

			if (request.Id == Guid.Empty)
			{
				_logger.LogError("Category ID cannot be empty.");

				return Result.Fail("Category ID cannot be empty.");
			}

			try
			{

				ApplicationDbContext context = _contextFactory.CreateDbContext();

				Category? category = await context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id);

				if (category == null)
				{
					_logger.LogWarning("No category found with ID: {CategoryId}", request.Id);

					return Result.Fail("Category not found.");
				}

				category.CategoryName = request.CategoryName;
				category.ModifiedOn = DateTime.UtcNow;

				await context.SaveChangesAsync();

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
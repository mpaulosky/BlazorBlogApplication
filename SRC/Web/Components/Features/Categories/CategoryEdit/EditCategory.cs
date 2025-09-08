// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EditCategory.cs
// Company :       mpaulosky
// Author :        Matthew
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

		private readonly IMyBlogContextFactory _contextFactory;

		private readonly ILogger<Handler> _logger;

		/// <summary>
		///   Initializes a new instance of the <see cref="Handler" /> class.
		/// </summary>
		/// <param name="contextFactory"></param>
		/// <param name="logger">The logger instance.</param>
		public Handler(IMyBlogContextFactory contextFactory, ILogger<Handler> logger)
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

			if (request.Id == ObjectId.Empty)
			{
				_logger.LogError("Category ID cannot be empty.");

				return Result.Fail("Category ID cannot be empty.");
			}

			try
			{

				var context = await _contextFactory.CreateContext(CancellationToken.None);

				var update = Builders<Category>.Update
					.Set(c => c.CategoryName, request.CategoryName)
					.Set(c => c.ModifiedOn, DateTime.UtcNow);

				var result = await context.Categories.UpdateOneAsync(
					Builders<Category>.Filter.Eq(x => x.Id, request.Id),
					update,
					new UpdateOptions { IsUpsert = false },
					CancellationToken.None);

				if (result.ModifiedCount == 0)
				{
					_logger.LogWarning("No category found with ID: {CategoryId}", request.Id);
					return Result.Fail("Category not found.");
				}

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

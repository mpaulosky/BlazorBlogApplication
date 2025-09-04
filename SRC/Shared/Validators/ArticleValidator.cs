// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleValidator.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Validators;

/// <summary>
///   Validator for the <see cref="Article" /> entity.
/// </summary>
public class ArticleValidator : AbstractValidator<Article>
{

	public ArticleValidator()
	{
		RuleFor(x => x.Title)
				.NotEmpty()
				.WithMessage("Title is required")
				.MaximumLength(100);

		RuleFor(x => x.Introduction)
				.NotEmpty()
				.WithMessage("Introduction is required")
				.MaximumLength(200);

		RuleFor(x => x.CoverImageUrl)
				.NotEmpty()
				.WithMessage("Cover image is required")
				.MaximumLength(200);

		RuleFor(x => x.UrlSlug)
				.NotEmpty()
				.WithMessage("URL slug is required")
				.MaximumLength(200)
				.Matches(@"^[a-z0-9_]+$")
				.WithMessage("URL slug can only contain lowercase letters, numbers, and underscores");

		RuleFor(x => x.Author)
				.NotNull()
				.WithMessage("Author is required");

		RuleFor(x => x.Category)
				.NotNull()
				.WithMessage("Categories is required");

		RuleFor(x => x.PublishedOn)
				.NotNull()
				.When(x => x.IsPublished)
				.WithMessage("PublishedOn is required when IsPublished is true");

		RuleFor(x => x.Content)
				.NotEmpty()
				.WithMessage("Content is required")
				.MaximumLength(4000)
				.WithMessage("Content cannot exceed 4000 characters");
	}

}

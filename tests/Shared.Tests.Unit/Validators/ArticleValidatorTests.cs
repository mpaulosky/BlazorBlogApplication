// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Validators;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(ArticleValidator))]
public class ArticleValidatorTests
{

	private readonly ArticleValidator _validator = new();

	[Fact]
	public void Should_Have_Error_When_Title_Is_Empty()
	{
		Article article = new()  { Title = "" };
		TestValidationResult<Article>? result = _validator.TestValidate(article);
		result.ShouldHaveValidationErrorFor(x => x.Title);
	}

	[Fact]
	public void Should_Not_Have_Error_When_Title_Is_Not_Empty()
	{
		Article article = new()  { Title = "Test Title" };
		TestValidationResult<Article>? result = _validator.TestValidate(article);
		result.ShouldNotHaveValidationErrorFor(x => x.Title);
	}

}
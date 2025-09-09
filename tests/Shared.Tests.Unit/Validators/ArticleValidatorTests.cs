// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew
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
		var article = new Article { Title = "" };
		var result = _validator.TestValidate(article);
		result.ShouldHaveValidationErrorFor(x => x.Title);
	}

	[Fact]
	public void Should_Not_Have_Error_When_Title_Is_Not_Empty()
	{
		var article = new Article { Title = "Test Title" };
		var result = _validator.TestValidate(article);
		result.ShouldNotHaveValidationErrorFor(x => x.Title);
	}

}

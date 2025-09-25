// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleDtoValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Validators;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(ArticleDtoValidator))]
public class ArticleDtoValidatorTests
{

	private readonly ArticleDtoValidator _validator = new();

	[Fact]
	public void Should_Have_Error_When_Title_Is_Empty()
	{
		ArticleDto dto = new()  { Title = "" };
		TestValidationResult<ArticleDto>? result = _validator.TestValidate(dto);
		result.ShouldHaveValidationErrorFor(x => x.Title);
	}

	[Fact]
	public void Should_Not_Have_Error_When_Title_Is_Not_Empty()
	{
		ArticleDto dto = new()  { Title = "Test Title" };
		TestValidationResult<ArticleDto>? result = _validator.TestValidate(dto);
		result.ShouldNotHaveValidationErrorFor(x => x.Title);
	}

}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Validators;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(CategoryValidator))]
public class CategoryValidatorTests
{

	private readonly CategoryValidator _validator = new();

	[Fact]
	public void Should_Have_Error_When_CategoryName_Is_Empty()
	{
		var category = new Category { CategoryName = "" };
		var result = _validator.TestValidate(category);
		result.ShouldHaveValidationErrorFor(x => x.CategoryName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_CategoryName_Is_Not_Empty()
	{
		var category = new Category { CategoryName = "Test Category" };
		var result = _validator.TestValidate(category);
		result.ShouldNotHaveValidationErrorFor(x => x.CategoryName);
	}

}

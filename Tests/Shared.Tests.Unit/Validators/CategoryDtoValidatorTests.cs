// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryDtoValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Validators;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(CategoryDtoValidator))]
public class CategoryDtoValidatorTests
{

	private readonly CategoryDtoValidator _validator = new();

	[Fact]
	public void Should_Have_Error_When_CategoryName_Is_Empty()
	{
		var dto = new CategoryDto { CategoryName = "" };
		var result = _validator.TestValidate(dto);
		result.ShouldHaveValidationErrorFor(x => x.CategoryName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_CategoryName_Is_Not_Empty()
	{
		var dto = new CategoryDto { CategoryName = "Test Category" };
		var result = _validator.TestValidate(dto);
		result.ShouldNotHaveValidationErrorFor(x => x.CategoryName);
	}

}

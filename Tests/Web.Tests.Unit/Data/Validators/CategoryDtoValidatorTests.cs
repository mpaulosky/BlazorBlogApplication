// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryDtoValidatorTests.cs
// Company :       mpaulosky
// Author :        Copilot
// Solution Name : MyBlog
// Project Name :  Web.Tests.Unit
// =======================================================

using FluentValidation.TestHelper;

namespace Web.Data.Validators;

[ExcludeFromCodeCoverage]
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

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUserDtoValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Validators;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(ApplicationUserDtoValidator))]
public class ApplicationUserDtoValidatorTests
{

	private readonly ApplicationUserDtoValidator _validator = new();

	[Fact]
	public void Should_Have_Error_When_UserName_Is_Empty()
	{
		var dto = new ApplicationUserDto { UserName = "" };
		var result = _validator.TestValidate(dto);
		result.ShouldHaveValidationErrorFor(x => x.UserName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_UserName_Is_Not_Empty()
	{
		var dto = new ApplicationUserDto { UserName = "TestUser" };
		var result = _validator.TestValidate(dto);
		result.ShouldNotHaveValidationErrorFor(x => x.UserName);
	}

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppUserDtoValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Validators;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(AppUserDtoValidator))]
public class AppUserDtoValidatorTests
{

	private readonly AppUserDtoValidator _validator = new();

	[Fact]
	public void Should_Have_Error_When_UserName_Is_Empty()
	{
		var dto = new AppUserDto { UserName = "" };
		var result = _validator.TestValidate(dto);
		result.ShouldHaveValidationErrorFor(x => x.UserName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_UserName_Is_Not_Empty()
	{
		var dto = new AppUserDto { UserName = "TestUser" };
		var result = _validator.TestValidate(dto);
		result.ShouldNotHaveValidationErrorFor(x => x.UserName);
	}

}

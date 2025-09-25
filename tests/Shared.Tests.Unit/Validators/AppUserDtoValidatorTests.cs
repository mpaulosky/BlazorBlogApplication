// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppUserDtoValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
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
		ApplicationUserDto dto = new()  { UserName = "" };
		TestValidationResult<ApplicationUserDto>? result = _validator.TestValidate(dto);
		result.ShouldHaveValidationErrorFor(x => x.UserName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_UserName_Is_Not_Empty()
	{
		ApplicationUserDto dto = new()  { UserName = "TestUser" };
		TestValidationResult<ApplicationUserDto>? result = _validator.TestValidate(dto);
		result.ShouldNotHaveValidationErrorFor(x => x.UserName);
	}

}
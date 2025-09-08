// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppUserValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Validators;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(AppUserValidator))]
public class AppUserValidatorTests
{

	private readonly AppUserValidator _validator = new();

	[Fact]
	public void Should_Have_Error_When_UserName_Is_Empty()
	{
		var user = new AppUser { UserName = "" };
		var result = _validator.TestValidate(user);
		result.ShouldHaveValidationErrorFor(x => x.UserName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_UserName_Is_Not_Empty()
	{
		var user = new AppUser { UserName = "TestUser" };
		var result = _validator.TestValidate(user);
		result.ShouldNotHaveValidationErrorFor(x => x.UserName);
	}

}

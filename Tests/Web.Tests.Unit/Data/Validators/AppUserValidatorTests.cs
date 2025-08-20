// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppUserValidatorTests.cs
// Company :       mpaulosky
// Author :        Copilot
// Solution Name : MyBlog
// Project Name :  Web.Tests.Unit
// =======================================================

using FluentValidation.TestHelper;

namespace Web.Data.Validators;

[ExcludeFromCodeCoverage]
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

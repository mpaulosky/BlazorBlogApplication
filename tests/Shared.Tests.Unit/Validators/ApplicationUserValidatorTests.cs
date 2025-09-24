// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUserValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Validators;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(ApplicationUserValidator))]
public class ApplicationUserValidatorTests
{

	private readonly ApplicationUserValidator _validator = new();

	[Fact]
	public void Should_Have_Error_When_UserName_Is_Empty()
	{
		var user = new ApplicationUser { UserName = "" };
		var result = _validator.TestValidate(user);
		result.ShouldHaveValidationErrorFor(x => x.UserName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_UserName_Is_Not_Empty()
	{
		var user = new ApplicationUser { UserName = "TestUser" };
		var result = _validator.TestValidate(user);
		result.ShouldNotHaveValidationErrorFor(x => x.UserName);
	}

	[Fact]
	public void Should_Have_Error_When_Email_Is_Empty()
	{
		var user = new ApplicationUser { Email = "" };
		var result = _validator.TestValidate(user);
		result.ShouldHaveValidationErrorFor(x => x.Email);
	}
	[Fact]
	public void Should_Not_Have_Error_When_Email_Is_Not_Empty()
	{
		var user = new ApplicationUser { Email = "test@example.com" };
		var result = _validator.TestValidate(user);
		result.ShouldNotHaveValidationErrorFor(x => x.Email);
	}

	[Fact]
	public void Should_Have_Error_When_Email_Is_Invalid()
	{
		var user = new ApplicationUser { Email = "invalid-email" };
		var result = _validator.TestValidate(user);
		result.ShouldHaveValidationErrorFor(x => x.Email);
	}

	[Fact]
	public void Should_Not_Have_Error_When_Email_Is_Valid()
	{
		var user = new ApplicationUser { Email = "test@example.com" };
		var result = _validator.TestValidate(user);
		result.ShouldNotHaveValidationErrorFor(x => x.Email);
	}

	[Fact]
	public void Should_Have_Error_When_DisplayName_Is_Empty()
	{
		var user = new ApplicationUser { DisplayName = "" };
		var result = _validator.TestValidate(user);
		result.ShouldHaveValidationErrorFor(x => x.DisplayName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_DisplayName_Is_Not_Empty()
	{
		var user = new ApplicationUser { DisplayName = "Test User" };
		var result = _validator.TestValidate(user);
		result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
	}

}


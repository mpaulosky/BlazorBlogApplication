// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUserValidatorTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
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
		ApplicationUser user = new()  { UserName = "" };
		TestValidationResult<ApplicationUser>? result = _validator.TestValidate(user);
		result.ShouldHaveValidationErrorFor(x => x.UserName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_UserName_Is_Not_Empty()
	{
		ApplicationUser user = new()  { UserName = "TestUser" };
		TestValidationResult<ApplicationUser>? result = _validator.TestValidate(user);
		result.ShouldNotHaveValidationErrorFor(x => x.UserName);
	}

	[Fact]
	public void Should_Have_Error_When_Email_Is_Empty()
	{
		ApplicationUser user = new()  { Email = "" };
		TestValidationResult<ApplicationUser>? result = _validator.TestValidate(user);
		result.ShouldHaveValidationErrorFor(x => x.Email);
	}

	[Fact]
	public void Should_Not_Have_Error_When_Email_Is_Not_Empty()
	{
		ApplicationUser user = new()  { Email = "test@example.com" };
		TestValidationResult<ApplicationUser>? result = _validator.TestValidate(user);
		result.ShouldNotHaveValidationErrorFor(x => x.Email);
	}

	[Fact]
	public void Should_Have_Error_When_Email_Is_Invalid()
	{
		ApplicationUser user = new()  { Email = "invalid-email" };
		TestValidationResult<ApplicationUser>? result = _validator.TestValidate(user);
		result.ShouldHaveValidationErrorFor(x => x.Email);
	}

	[Fact]
	public void Should_Not_Have_Error_When_Email_Is_Valid()
	{
		ApplicationUser user = new()  { Email = "test@example.com" };
		TestValidationResult<ApplicationUser>? result = _validator.TestValidate(user);
		result.ShouldNotHaveValidationErrorFor(x => x.Email);
	}

	[Fact]
	public void Should_Have_Error_When_DisplayName_Is_Empty()
	{
		ApplicationUser user = new()  { DisplayName = "" };
		TestValidationResult<ApplicationUser>? result = _validator.TestValidate(user);
		result.ShouldHaveValidationErrorFor(x => x.DisplayName);
	}

	[Fact]
	public void Should_Not_Have_Error_When_DisplayName_Is_Not_Empty()
	{
		ApplicationUser user = new()  { DisplayName = "Test User" };
		TestValidationResult<ApplicationUser>? result = _validator.TestValidate(user);
		result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
	}

}
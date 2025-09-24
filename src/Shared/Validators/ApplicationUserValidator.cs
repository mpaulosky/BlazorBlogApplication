// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUserValidator.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Validators;

/// <summary>
///   Validator for the ApplicationUser entity.
/// </summary>
public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
{

	/// <summary>
	///   Initializes a new instance of the <see cref="ApplicationUserValidator" /> class.
	/// </summary>
	public ApplicationUserValidator()
	{
		RuleFor(x => x.Id)
				.NotEmpty().WithMessage("User ID is required");

		RuleFor(x => x.UserName)
				.NotEmpty().WithMessage("Username is required")
				.Length(3, 50).WithMessage("Username must be between 3 and 50 characters");

		RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email address format");

		RuleFor(x => x.DisplayName)
				.NotEmpty().WithMessage("Display Name is required")
				.Length(3, 50).WithMessage("Display Name must be between 3 and 50 characters");
	}

}	

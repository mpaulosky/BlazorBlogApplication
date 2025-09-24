// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUserDtoValidator.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Validators;

/// <summary>
///   Validator for the ApplicationUserDto class.
/// </summary>
public class ApplicationUserDtoValidator : AbstractValidator<ApplicationUserDto>
{

	/// <summary>
	///   Initializes a new instance of the <see cref="ApplicationUserDtoValidator" /> class.
	/// </summary>
	public ApplicationUserDtoValidator()
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
				.NotNull().WithMessage("Display Name is required")
				.Length(3, 50).WithMessage("Display Name must be between 3 and 50 characters");
	}

}

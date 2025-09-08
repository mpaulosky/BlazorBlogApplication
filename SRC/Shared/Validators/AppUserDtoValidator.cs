// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppUserDtoValidator.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Validators;

/// <summary>
///   Validator for the AppUserDto class.
/// </summary>
public class AppUserDtoValidator : AbstractValidator<AppUserDto>
{

	/// <summary>
	///   Initializes a new instance of the <see cref="AppUserDtoValidator" /> class.
	/// </summary>
	public AppUserDtoValidator()
	{
		RuleFor(x => x.Id)
				.NotEmpty().WithMessage("User ID is required");

		RuleFor(x => x.UserName)
				.NotEmpty().WithMessage("Username is required")
				.Length(3, 50).WithMessage("Username must be between 3 and 50 characters");

		RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email address format");

		RuleFor(x => x.Roles)
				.NotNull().WithMessage("Roles collection cannot be null")
				.NotEmpty().WithMessage("Roles collection cannot be empty");
	}

}

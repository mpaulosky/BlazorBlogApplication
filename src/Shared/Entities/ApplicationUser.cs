// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUser.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUser.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

using Microsoft.AspNetCore.Identity;

namespace Shared.Entities;

public sealed class ApplicationUser : IdentityUser
{

	/// <summary>
	///   Parameterless constructor to ensure IdentityUser string properties
	///   are initialized to empty values for predictable test behavior.
	/// </summary>
	public ApplicationUser()
	{
		Id = string.Empty;
		UserName = string.Empty;
		NormalizedUserName = string.Empty;
		Email = string.Empty;
		NormalizedEmail = string.Empty;
		PhoneNumber = string.Empty;
		SecurityStamp = string.Empty;
		ConcurrencyStamp = string.Empty;
	}

	public string DisplayName { get; init; } = string.Empty;

}

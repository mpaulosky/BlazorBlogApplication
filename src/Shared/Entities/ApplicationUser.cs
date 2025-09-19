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

	public string DisplayName { get; init; } = string.Empty;

}

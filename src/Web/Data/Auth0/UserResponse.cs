// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     UserResponse.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================
namespace Web.Data.Auth0;

/// <summary>
///   Minimal DTO that represents a user response from Auth0 used by the UI.
///   Kept minimal to satisfy compile-time references in Razor components and tests.
/// </summary>
public sealed class UserResponse
{

	public string? UserId { get; set; }

	public string? Email { get; set; }

	public string? GivenName { get; set; }

	public string? FamilyName { get; set; }

	// Properties used by the UI
	public string? Name { get; set; }

	public List<string>? Roles { get; set; }

	public bool EmailVerified { get; set; }

	public DateTime? CreatedAt { get; set; }

	public DateTime? UpdatedAt { get; set; }

	public string? Picture { get; set; }

}

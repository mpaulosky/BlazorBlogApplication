// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUserDto.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Models;

/// <summary>
///   Data transfer object representing an application user.
///   Contains a subset of properties from <c>Shared.Entities.ApplicationUser</c> used by the UI and APIs.
/// </summary>
public sealed record ApplicationUserDto
{

	/// <summary>
	///   Gets the user id.
	/// </summary>
	public string Id { get; init; } = string.Empty;

	/// <summary>
	///   Gets the user name / login.
	/// </summary>
	public string UserName { get; init; } = string.Empty;

	/// <summary>
	///   Gets the email address.
	/// </summary>
	public string Email { get; init; } = string.Empty;

	/// <summary>
	///   Gets the display name set on the application user.
	/// </summary>
	public string DisplayName { get; init; } = string.Empty;

	/// <summary>
	///   Gets or sets whether the email address is confirmed.
	/// </summary>
	public bool EmailConfirmed { get; init; }


	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public ApplicationUserDto() : this(string.Empty, string.Empty, string.Empty, string.Empty, false) { }

	/// <summary>
	///   Initializes a new instance of the <see cref="ApplicationUserDto" /> class.
	/// </summary>
	/// <param name="id">The unique identifier for the user.</param>
	/// <param name="userName">The username of the user.</param>
	/// <param name="email">The email address of the user.</param>
	/// <param name="displayName">The display name of the user.</param>
	/// <param name="emailConfirmed">Whether the email address is confirmed.</param>
	private ApplicationUserDto(string id, string userName, string email, string displayName, bool emailConfirmed)
	{
		Id = id;
		UserName = userName;
		Email = email;
		DisplayName = displayName;
		EmailConfirmed = emailConfirmed;
	}

	/// <summary>
	///   Gets an empty singleton instance of ApplicationUserDto with default values.
	/// </summary>
	public static ApplicationUserDto Empty { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty, false);

}
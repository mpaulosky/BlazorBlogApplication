// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppUser.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Entities;

/// <summary>
///   Domain entity representing an application user.
/// </summary>
public class AppUser
{

	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public AppUser() : this(string.Empty, string.Empty, string.Empty, []) { }

	/// <summary>
	///   Initializes a new instance of the <see cref="AppUser" /> class.
	/// </summary>
	/// <param name="id">The users Id</param>
	/// <param name="userName">The username of the user.</param>
	/// <param name="email">The email address of the user.</param>
	/// <param name="roles">The list of roles assigned to the user.</param>
	private AppUser(string id, string userName, string email, List<string> roles)
	{
		Id = id;
		UserName = userName;
		Email = email;
		Roles = roles;
	}

	/// <summary>
	///   Gets or sets the id of the user.
	/// </summary>
	public string Id { get; set; }

	/// <summary>
	///   Gets or sets the username of the user.
	/// </summary>
	public string UserName { get; set; }

	/// <summary>
	///   Gets or sets the email address of the user.
	/// </summary>
	public string Email { get; set; }

	/// <summary>
	///   Gets or sets the list of roles assigned to the user.
	/// </summary>
	public List<string> Roles { get; set; }

	/// <summary>
	///   Gets an empty instance of AppUser with default values.
	/// </summary>
	public static AppUser Empty => new(string.Empty, string.Empty, string.Empty, []);

}

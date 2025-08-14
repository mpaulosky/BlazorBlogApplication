// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IAuth0Service.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  TailwindBlog.Web
// =======================================================

namespace Web.Data.Auth0;

/// <summary>
/// Defines the contract for Auth0Service.
/// </summary>
public interface IAuth0Service
{
	/// <summary>
	/// Gets the list of users from Auth0.
	/// </summary>
	/// <returns>A list of user responses, or null if none found.</returns>
	Task<List<UserResponse>?> GetUsersAsync();
}
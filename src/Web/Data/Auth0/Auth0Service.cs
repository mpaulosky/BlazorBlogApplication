// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Auth0Service.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================
namespace Web.Data.Auth0;

/// <summary>
///   Minimal stub of an Auth0 service used by the UI. Provides an async method to fetch users.
///   This is intentionally lightweight and synchronous for compile-time satisfaction; replace with real implementation as
///   needed.
/// </summary>
public class Auth0Service
{

	public Task<List<UserResponse>> GetUsersAsync()
	{
		return Task.FromResult(new List<UserResponse>());
	}

}

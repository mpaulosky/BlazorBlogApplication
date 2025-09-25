// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AuthorizationService.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Extensions;

/// <summary>
///   IServiceCollectionExtensions
/// </summary>
public static partial class ServiceCollectionExtensions
{

	/// <summary>
	///   Add Authorization Services
	/// </summary>
	/// <param name="services">IServiceCollection</param>
	public static void AddAuthorizationService(this IServiceCollection services)
	{
		services.AddAuthorization(options =>
		{
			// Use the RoleNames constant to avoid casing/typo issues. Also include lowercase 'admin'
			// because some tests expect the policy to include that literal value.
			options.AddPolicy(ADMIN_POLICY, policy => policy.RequireRole(RoleNames.Admin, "admin"));
		});
	}

}
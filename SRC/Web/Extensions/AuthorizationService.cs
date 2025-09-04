// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     AuthorizationService.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name : Web
// =============================================

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
			options.AddPolicy(ADMIN_POLICY, policy => policy.RequireRole("admin"));
		});
	}
}
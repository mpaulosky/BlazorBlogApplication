// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Helpers.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web;

[ExcludeFromCodeCoverage]
public static class Helpers
{

	/// <summary>
	///   Sets up the authorization context for testing components with flexible role assignment.
	/// </summary>
	/// <param name="context">A BunitContext</param>
	/// <param name="isAuthorized">If true, authorizes the test user; if false, sets unauthorized state</param>
	/// <param name="roles">Optional list of roles to assign to the test user. If empty, no roles are assigned.</param>
	/// <remarks>
	///   This helper method configures the authentication state for component testing:
	///   - When authorized, sets up a "Test User" identity
	///   - Adds any combination of roles (Admin, Author, User) as claims
	///   - When unauthorized, explicitly sets not authorized state
	/// </remarks>
	public static void SetAuthorization(BunitContext context, bool isAuthorized = true, params string[] roles)
	{
		// Register the full set of common test services used across the suite.
		// This includes NavigationManager, loggers, a lightweight Auth0Service,
		// a test MyzBlogContext, and handler substitutes/mappings. Individual
		// tests may still override or register concrete handlers as needed.
		//TestServiceRegistrations.RegisterAll(context);

		BunitAuthorizationContext authContext = context.AddAuthorization();

		if (isAuthorized)
		{
			// Keep the default test identity name for broad test compatibility
			authContext.SetAuthorized("Test User");

			// Build claims: include email and a sample profile picture URL as string, and optional roles
			const string testEmail = "test@example.com";

			List<Claim> claims = new()
			{
					new Claim(ClaimTypes.Email, testEmail), new Claim("picture", "https://example.com/picture.jpg")
			};

			if (roles.Length > 0)
			{
				claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
			}

			authContext.SetClaims(claims.ToArray());
		}
		else
		{
			authContext.SetNotAuthorized();
		}

	}

}
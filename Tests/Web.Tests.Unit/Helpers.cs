// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Helpers.cs
// Company :       mpaulosky
// Author :        Matthew
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
		// a test MyBlogContext, and handler substitutes/mappings. Individual
		// tests may still override or register concrete handlers as needed.
		//TestServiceRegistrations.RegisterAll(context);

		var authContext = context.AddAuthorization();

		if (isAuthorized)
		{
			// Keep the default test identity name for broad test compatibility
			authContext.SetAuthorized("Test User");

			// Build claims: include email and a sample profile picture URL as string, and optional roles
			const string testEmail = "test@example.com";

			var claims = new List<Claim>
			{
					new(ClaimTypes.Email, testEmail),
					new("picture", "https://example.com/picture.jpg")
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

[ExcludeFromCodeCoverage]
public static class TestFixtures
{

	public static Mock<IAsyncCursor<TEntity>> GetMockCursor<TEntity>(IEnumerable<TEntity> list) where TEntity : class?
	{
		Mock<IAsyncCursor<TEntity>> cursor = new();
		cursor.Setup(a => a.Current).Returns(list);

		cursor
				.SetupSequence(a => a.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(true)
				.Returns(false);

		cursor
				.SetupSequence(a => a.MoveNextAsync(It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(true))
				.Returns(Task.FromResult(false));

		return cursor;
	}

}

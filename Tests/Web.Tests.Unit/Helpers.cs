// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Helpers.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  Web.Tests.Bunit
// =======================================================

using MongoDB.Driver;

using Moq;
using System.Linq;
using System.Security.Claims;
using System.Diagnostics.CodeAnalysis;
// Ensure the TestServiceRegistrations helper is available to register common services
// into a BunitContext when tests call SetAuthorization.


namespace Web;

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
		TestServiceRegistrations.RegisterAll(context);

		var authContext = context.AddAuthorization();

		if (isAuthorized)
		{
			authContext.SetAuthorized("Test User");
		}
		else
		{
			authContext.SetNotAuthorized();
		}

		if (roles.Length > 0)
		{
			authContext.SetClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray());
		}

	}

}

[ExcludeFromCodeCoverage]
public static class TestFixtures
{

	public static Mock<IAsyncCursor<TEntity>> GetMockCursor<TEntity>(IEnumerable<TEntity> list) where TEntity : class?
	{
		Mock<IAsyncCursor<TEntity>> cursor = new();
		cursor.Setup(_ => _.Current).Returns(list);

		cursor
				.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(true)
				.Returns(false);

		cursor
				.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(true))
				.Returns(Task.FromResult(false));

		return cursor;
	}

}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     LoginComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

/// <summary>
///   Unit tests for the AccessControlComponent component.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(AccessControlComponent))]
public class AccessControlComponentTests : BunitContext
{

	[Theory]
	[InlineData(true, false, "Log out")]
	[InlineData(false, false, "Log in")]
	public void AccessControlComponent_RendersCorrectly(bool isAuthorized, bool hasRoles, string expectedText)
	{
		// Arrange
		SetAuthorization(isAuthorized, hasRoles);

		// Act
		var cut = Render<AccessControlComponent>();

		// Assert
		cut.MarkupMatches(
				$"""<a href="Account/{(isAuthorized ? "Logout" : "Login")}">{expectedText}</a>"""); // Update selector as needed

	}

	private void SetAuthorization(bool isAuthorized = true, bool hasRoles = false)
	{

		var authContext = AddAuthorization();

		// Set up the authentication state for the component
		if (isAuthorized)
		{

			// If authorized, set the context to authorize with a test user
			authContext.SetAuthorized("Test User");

		}
		else
		{

			// If not authorized, set the context to not authorized
			authContext.SetNotAuthorized();

		}

		// Optionally set roles if required
		if (hasRoles)
		{

			authContext.SetClaims(new Claim(ClaimTypes.Role, "Admin"), new Claim(ClaimTypes.Role, "User"));

		}

	}

}

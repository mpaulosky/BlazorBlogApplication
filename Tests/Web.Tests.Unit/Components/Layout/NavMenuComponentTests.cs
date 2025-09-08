// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     NavMenuComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Layout;

/// <summary>
///   bUnit tests for NavMenuComponent.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(NavMenuComponent))]
public class NavMenuComponentTests : BunitContext
{

	public NavMenuComponentTests()
	{

		Services.AddScoped<CascadingAuthenticationState>();

	}

	[Theory]
	[InlineData(true, false, "Log out")]
	[InlineData(false, false, "Log in")]
	public void Should_Render_NavMenu_Links(bool isAuthorized, bool hasRoles, string expectedText)
	{

		// Arrange
		SetAuthorization(isAuthorized, hasRoles);

		// Act
		var cut = Render<NavMenuComponent>();

		// Assert
		cut.Markup.Should().Contain("Articles");
		cut.Markup.Should().Contain("Categories");
		cut.Markup.Should().Contain("Contact");
		cut.Markup.Should().Contain("About");
		cut.Markup.Should().Contain("Article Service");
		cut.Markup.Should().Contain(expectedText);
		cut.Markup.Should().Contain("p-1 hover:text-blue-700");

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

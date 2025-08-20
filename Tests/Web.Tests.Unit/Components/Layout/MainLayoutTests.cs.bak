// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MainLayoutTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  Web.Tests.Bunit
// =======================================================

namespace Web.Components.Layout;

/// <summary>
///   bUnit tests for MainLayout.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(MainLayout))]
public class MainLayoutTests : BunitContext
{

	public MainLayoutTests()
	{

		Services.AddScoped<CascadingAuthenticationState>();

	}

	[Fact]
	public void Should_Render_NavMenu_And_Footer()
	{

		// Arrange
		SetAuthorization(false);

		// Act
		var cut = Render<MainLayout>();

		// Assert
		cut.Markup.Should().Contain("Tailwind Blogs");
		cut.Markup.Should().Contain("All rights reserved");
		cut.Markup.Should().Contain("©");
		cut.Markup.Should().Contain("MPaulosky Co. All rights reserved.");
		cut.Markup.Should().Contain("Articles");
		cut.Markup.Should().Contain("Categories");
		cut.Markup.Should().Contain("Contact");
		cut.Markup.Should().Contain("About");
		cut.Markup.Should().Contain("Log in");

	}

	[Fact]
	public void Should_Render_NavMenu_With_Authenticated_User()
	{

		// Arrange
		SetAuthorization(true, true);

		// Act
		var cut = Render<MainLayout>();

		// Assert
		cut.Markup.Should().Contain("Hey Test User!");
		// Profile link should be present for authenticated users
		cut.Markup.Should().Contain("Profile");
		// AccessControlComponent will render Log out when authenticated
		cut.Markup.Should().Contain("Log out");

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
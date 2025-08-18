// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ProfileTests.cs
// Company :       mpaulosky
// Author :        Copilot
// Solution Name : BlazorBlogApplications
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Data.Auth0;

namespace Web.Components.Features.UserInfo;

/// <summary>
///   Unit tests for <see cref="Profile" /> (User Profile Page).
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Profile))]
public class ProfileTests : BunitContext
{
	public ProfileTests()
	{
		Services.AddCascadingAuthenticationState();
		Services.AddAuthorization();

		TestServiceRegistrations.RegisterCommonUtilities(this);
	}

	[Fact]
	public void Renders_Loading_State()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		var cut = Render<Profile>();
		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, null);
		cut.Render();
		cut.Markup.Should().Contain("Loading user information...");
	}

	[Fact]
	public void Renders_User_Profile_With_Data()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		var user = new UserResponse
		{
			Name = "Alice",
			UserId = "auth0|123",
			Email = "alice@example.com",
			Roles = new List<string> { "Admin", "Editor" },
			EmailVerified = true,
			CreatedAt = "2025-08-15T00:00:00Z",
			UpdatedAt = "2025-08-15T00:00:00Z",
			Picture = "https://example.com/pic.jpg"
		};
		var cut = Render<Profile>();
		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, user);
		cut.Render();
		cut.Markup.Should().Contain("Alice");
		cut.Markup.Should().Contain("auth0|123");
		cut.Markup.Should().Contain("alice@example.com");
		cut.Markup.Should().Contain("Admin");
		cut.Markup.Should().Contain("Editor");
		// The component renders the email verified boolean as 'True' or 'False'
		cut.Markup.Should().Contain("Your email verified: True");
	}

	[Fact(Skip = "Bunit does not enforce [Authorize]; page is always rendered in test context.")]
	public void Only_Authenticated_User_Can_Access()
	{
		// Arrange
		Helpers.SetAuthorization(this, false);
		var cut = Render<Profile>();
		// Assert
		cut.FindAll("div").Should().BeEmpty();
	}
}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     UserProfileTests.cs
// Company :       mpaulosky
// Author :        Copilot
// Solution Name : BlazorBlogApplications
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Data.Auth0;

namespace Web.Components.Features.UserInfo.Profile;

/// <summary>
///   Unit tests for <see cref="UserProfile" /> (User UserProfile Page).
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(UserProfile))]
public class UserProfileTests : BunitContext
{
	public UserProfileTests()
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
		TestServiceRegistrations.RegisterAll(this);

		// Act
		var cut = Render<UserProfile>();
		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cut.Instance, null);

		// Assert
		cut.Render();
		cut.Markup.Should().Contain("Loading user information...");
	}

	[Fact]
	public void Renders_User_Profile_With_Data()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		TestServiceRegistrations.RegisterAll(this);
		var user = new UserResponse
		{
			Name = "Alice",
			UserId = "auth0|123",
			Email = "alice@example.com",
			Roles = ["Admin", "Editor"],
			EmailVerified = true,
			CreatedAt = "2025-08-15T00:00:00Z",
			UpdatedAt = "2025-08-15T00:00:00Z",
			Picture = "https://example.com/pic.jpg"
		};
		var cut = Render<UserProfile>();
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

	[Fact]
	public void Only_Authenticated_User_Can_Access()
	{
		// Arrange - simulate not authorized
		Helpers.SetAuthorization(this, false);
		TestServiceRegistrations.RegisterCommonUtilities(this);

		// Act - render an AuthorizeView with NotAuthorized content to avoid pulling in the whole Router
		RenderFragment<AuthenticationState> authorizedFragment = _ => builder => builder.AddMarkupContent(0, "<div>authorized</div>");
		RenderFragment<AuthenticationState> notAuthorizedFragment = _ => builder =>
		{
			builder.OpenComponent<ErrorPageComponent>(0);
			builder.AddAttribute(1, "ErrorCode", 401);
			builder.AddAttribute(2, "TextColor", "red-600");
			builder.AddAttribute(3, "ShadowStyle", "shadow-red-500");
			builder.CloseComponent();
		};

		var cut = Render<AuthorizeView>(parameters => parameters
			.Add(p => p.Authorized, authorizedFragment)
			.Add(p => p.NotAuthorized, notAuthorizedFragment)
		);

		// Assert - NotAuthorized content should show the 401 ErrorPageComponent message
		cut.Markup.Should().Contain("401 Unauthorized");
		cut.Markup.Should().Contain("You are not authorized to view this page.");
	}
}
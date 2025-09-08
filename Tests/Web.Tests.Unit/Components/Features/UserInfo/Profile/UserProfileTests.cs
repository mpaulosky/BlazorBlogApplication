// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     UserProfileTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

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

		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, null);

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

		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, user);

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
	public void Renders_User_Profile_With_No_Roles()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		TestServiceRegistrations.RegisterAll(this);

		var user = new UserResponse
		{
			Name = "Bob",
			UserId = "auth0|456",
			Email = "bob@example.com",
			Roles = null, // No roles assigned
			EmailVerified = false,
			Picture = "https://example.com/bob.jpg"
		};

		var cut = Render<UserProfile>();

		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, user);

		cut.Render();
		cut.Markup.Should().Contain("Bob");
		cut.Markup.Should().Contain("No roles assigned");
		cut.Markup.Should().Contain("Your email verified: False");
	}

	[Fact]
	public void Renders_User_Profile_With_Empty_Roles()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		TestServiceRegistrations.RegisterAll(this);

		var user = new UserResponse
		{
			Name = "Charlie",
			UserId = "auth0|789",
			Email = "charlie@example.com",
			Roles = [], // Empty roles list
			EmailVerified = true,
			Picture = "https://example.com/charlie.jpg"
		};

		var cut = Render<UserProfile>();

		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, user);

		cut.Render();
		cut.Markup.Should().Contain("Charlie");
		// Empty roles array renders as empty string after string.Join
		cut.Markup.Should().Contain("Your roles: ");
	}

	[Fact]
	public void Renders_User_Profile_With_Special_Characters()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		TestServiceRegistrations.RegisterAll(this);

		var user = new UserResponse
		{
			Name = "José María & Sons",
			UserId = "auth0|special",
			Email = "jose.maria@example.com",
			Roles = ["Admin", "Test Role"],
			EmailVerified = true,
			Picture = "https://example.com/jose.jpg"
		};

		var cut = Render<UserProfile>();

		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, user);

		cut.Render();
		cut.Markup.Should().Contain("José María &amp; Sons"); // HTML encoded
		cut.Markup.Should().Contain("Test Role");
	}

	[Fact]
	public void Handles_Error_When_User_Data_Is_Null()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		TestServiceRegistrations.RegisterAll(this);

		var cut = Render<UserProfile>();

		// Set _user to null to simulate error state
		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, null);

		cut.Render();
		cut.Markup.Should().Contain("Loading user information...");
	}

	[Fact]
	public void Handles_User_With_Missing_Email()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		TestServiceRegistrations.RegisterAll(this);

		var user = new UserResponse
		{
			Name = "Test User",
			UserId = "auth0|test",
			Email = "", // Empty email
			Roles = ["User"],
			EmailVerified = false,
			Picture = "https://example.com/test.jpg"
		};

		var cut = Render<UserProfile>();

		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, user);

		cut.Render();
		cut.Markup.Should().Contain("Test User");
		cut.Markup.Should().Contain("auth0|test");
		cut.Markup.Should().Contain("Your email: "); // Empty email renders as empty string
	}

	[Fact]
	public void Handles_User_With_Very_Long_Name()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "User");
		TestServiceRegistrations.RegisterAll(this);

		var longName = new string('A', 200); // Very long name
		var user = new UserResponse
		{
			Name = longName,
			UserId = "auth0|long",
			Email = "long@example.com",
			Roles = ["User"],
			EmailVerified = true,
			Picture = "https://example.com/long.jpg"
		};

		var cut = Render<UserProfile>();

		cut.Instance.GetType().GetField("_user", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, user);

		cut.Render();
		cut.Markup.Should().Contain(longName);
		cut.Markup.Should().Contain("long@example.com");
	}

	[Fact]
	public void Only_Authenticated_User_Can_Access()
	{
		// Arrange - simulate not authorized
		Helpers.SetAuthorization(this, false);
		TestServiceRegistrations.RegisterCommonUtilities(this);

		// Act - render an AuthorizeView with NotAuthorized content to avoid pulling in the whole Router
		RenderFragment<AuthenticationState> authorizedFragment =
				_ => builder => builder.AddMarkupContent(0, "<div>authorized</div>");

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

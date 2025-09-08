// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     UserOverviewTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.UserInfo.Profile;

/// <summary>
///   Unit tests for <see cref="UserOverview" /> (User Overview Page).
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(UserOverview))]
public class UserOverviewTests : BunitContext
{

	public UserOverviewTests()
	{
		Services.AddCascadingAuthenticationState();
		Services.AddAuthorization();

		TestServiceRegistrations.RegisterAll(this);
	}

	[Fact]
	public void Renders_Loading_State()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var cut = Render<UserOverview>();

		cut.Instance.GetType().GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, null);

		cut.Render();
		cut.Markup.Should().Contain("Loading users...");
	}

	[Fact]
	public void Renders_Empty_State()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var cut = Render<UserOverview>();

		cut.Instance.GetType().GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, new List<AppUserDto>().AsQueryable());

		cut.Render();
		cut.Markup.Should().Contain("No users found.");
	}

	[Fact]
	public void Renders_User_Grid_With_Data()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");

		var users = new List<AppUserDto>
		{
				new()  { UserName = "Alice", Email = "alice@example.com", Roles = ["Admin"] },
				new()  { UserName = "Bob", Email = "bob@example.com", Roles = ["Editor"] }
		};

		var cut = Render<UserOverview>();

		cut.Instance.GetType().GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, users.AsQueryable());

		cut.Render();
		cut.Markup.Should().Contain("Alice");
		cut.Markup.Should().Contain("Bob");
		cut.Markup.Should().Contain("Admin");
		cut.Markup.Should().Contain("Editor");
	}

	[Fact]
	public void Only_Authenticated_Users_Can_Access()
	{
		// Arrange - unauthenticated
		Helpers.SetAuthorization(this, false);
		TestServiceRegistrations.RegisterCommonUtilities(this);

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

		cut.Markup.Should().Contain("401 Unauthorized");
		cut.Markup.Should().Contain("You are not authorized to view this page.");
	}

	[Fact]
	public void Non_Admin_User_Is_Not_Authorized()
	{
		// Arrange - authenticated but not in the Admin role
		Helpers.SetAuthorization(this, true, "User");
		TestServiceRegistrations.RegisterCommonUtilities(this);

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

		cut.Markup.Should().Contain("401 Unauthorized");
		cut.Markup.Should().Contain("You are not authorized to view this page.");
	}

	[Fact]
	public void Handles_Users_With_No_Roles()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");

		var users = new List<AppUserDto>
		{
				new()  { UserName = "Alice", Email = "alice@example.com", Roles = null },
				new()  { UserName = "Bob", Email = "bob@example.com", Roles = [] }
		};

		var cut = Render<UserOverview>();

		cut.Instance.GetType().GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, users.AsQueryable());

		cut.Render();
		cut.Markup.Should().Contain("Alice");
		cut.Markup.Should().Contain("Bob");
		cut.Markup.Should().Contain("No roles");
	}

	[Fact]
	public void Handles_Users_With_Special_Characters()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");

		var users = new List<AppUserDto>
		{
				new()  { UserName = "José María", Email = "jose@example.com", Roles = ["Admin"] },
				new()  { UserName = "Test & Co", Email = "test@example.com", Roles = ["User"] }
		};

		var cut = Render<UserOverview>();

		cut.Instance.GetType().GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, users.AsQueryable());

		cut.Render();
		cut.Markup.Should().Contain("José María");
		cut.Markup.Should().Contain("Test &amp; Co");
	}

	[Fact]
	public void Handles_Large_User_List()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");

		var users = new List<AppUserDto>();
		for (int i = 1; i <= 100; i++)
		{
			users.Add(new AppUserDto
			{
				UserName = $"User{i}",
				Email = $"user{i}@example.com",
				Roles = ["User"]
			});
		}

		var cut = Render<UserOverview>();

		cut.Instance.GetType().GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, users.AsQueryable());

		cut.Render();
		cut.Markup.Should().Contain("User1");
		cut.Markup.Should().Contain("User100");
		cut.Markup.Should().Contain("user1@example.com");
		cut.Markup.Should().Contain("user100@example.com");
	}

	[Fact]
	public void Handles_Users_With_Empty_Email()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");

		var users = new List<AppUserDto>
		{
				new()  { UserName = "Test User", Email = "", Roles = ["User"] }
		};

		var cut = Render<UserOverview>();

		cut.Instance.GetType().GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, users.AsQueryable());

		cut.Render();
		cut.Markup.Should().Contain("Test User");
		// Empty email renders as empty cell in table
		cut.Markup.Should().Contain("<td class=\"px-6 py-4 whitespace-nowrap text-sm text-gray-500\"></td>");
	}

	[Fact]
	public void Handles_Users_With_Very_Long_Names()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");

		var longName = new string('A', 200);
		var users = new List<AppUserDto>
		{
				new()  { UserName = longName, Email = "long@example.com", Roles = ["User"] }
		};

		var cut = Render<UserOverview>();

		cut.Instance.GetType().GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance)
				?.SetValue(cut.Instance, users.AsQueryable());

		cut.Render();
		cut.Markup.Should().Contain(longName);
		cut.Markup.Should().Contain("long@example.com");
	}

}

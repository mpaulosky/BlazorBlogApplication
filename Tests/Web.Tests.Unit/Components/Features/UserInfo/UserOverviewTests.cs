// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     UserOverviewTests.cs
// Company :       mpaulosky
// Author :        Copilot
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Components.Features.UserInfo;
using Microsoft.AspNetCore.Components.Authorization;
using NSubstitute;
using FluentAssertions;
using Xunit;

namespace Web.Components.Features.UserInfo;

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

		TestServiceRegistrations.RegisterCommonUtilities(this);
	}

	[Fact]
	public void Renders_Loading_State()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var cut = Render<UserOverview>();
		cut.Instance.GetType().GetField("_users", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, null);
		cut.Render();
		cut.Markup.Should().Contain("Loading users...");
	}

	[Fact]
	public void Renders_Empty_State()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var cut = Render<UserOverview>();
		cut.Instance.GetType().GetField("_users", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, new List<AppUserDto>().AsQueryable());
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
			new AppUserDto { UserName = "Alice", Email = "alice@example.com", Roles = new List<string>{"Admin"} },
			new AppUserDto { UserName = "Bob", Email = "bob@example.com", Roles = new List<string>{"Editor"} }
		};
		var cut = Render<UserOverview>();
		cut.Instance.GetType().GetField("_users", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cut.Instance, users.AsQueryable());
		cut.Render();
		cut.Markup.Should().Contain("Alice");
		cut.Markup.Should().Contain("Bob");
		cut.Markup.Should().Contain("Admin");
		cut.Markup.Should().Contain("Editor");
	}

	[Fact(Skip = "Bunit does not enforce [Authorize]; page is always rendered in test context.")]
	public void Only_Admin_Can_Access()
	{
		// Arrange
		Helpers.SetAuthorization(this, false);
		var cut = Render<UserOverview>();
		// Assert
		cut.FindAll("table").Should().BeEmpty();
	}
}

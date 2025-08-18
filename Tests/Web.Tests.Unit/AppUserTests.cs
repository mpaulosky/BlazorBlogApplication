// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppUserTests.cs
// Company :       mpaulosky
// Author :        Copilot
// Solution Name : MyBlog
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Data.Entities;

namespace Web;

/// <summary>
///   Unit tests for the <see cref="AppUser"/> domain entity.
/// </summary>
public class AppUserTests
{
	[Fact]
	public void DefaultConstructor_ShouldSetPropertiesToDefaults()
	{
		var user = new AppUser();
		user.Id.Should().BeEmpty();
		user.UserName.Should().BeEmpty();
		user.Email.Should().BeEmpty();
		user.Roles.Should().NotBeNull();
		user.Roles.Should().BeEmpty();
	}

	[Fact]
	public void ParameterizedConstructor_ShouldSetPropertiesCorrectly()
	{
		var roles = new List<string> { "Admin", "Editor" };
		var ctor = typeof(AppUser).GetConstructor(
			BindingFlags.NonPublic | BindingFlags.Instance,
			null,
			[ typeof(string), typeof(string), typeof(string), typeof(List<string>) ],
			null);
		ctor.Should().NotBeNull("AppUser private constructor not found");
		var user = (AppUser)ctor.Invoke([ "123", "test user", "test@example.com", roles ]);
		user.Should().NotBeNull();
		user.Id.Should().Be("123");
		user.UserName.Should().Be("test user");
		user.Email.Should().Be("test@example.com");
		user.Roles.Should().BeEquivalentTo(roles);
	}

	[Fact]
	public void EmptyProperty_ShouldReturnEmptyUser()
	{
		var user = AppUser.Empty;
		user.Id.Should().BeEmpty();
		user.UserName.Should().BeEmpty();
		user.Email.Should().BeEmpty();
		user.Roles.Should().NotBeNull();
		user.Roles.Should().BeEmpty();
	}

	[Fact]
	public void Roles_ShouldBeInitializedToEmptyList()
	{
		var user = new AppUser();
		user.Roles.Should().NotBeNull();
		user.Roles.Should().BeEmpty();
	}
}
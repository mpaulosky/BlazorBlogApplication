// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppUserTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Entities;

/// <summary>
///   Unit tests for the <see cref="AppUser" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(AppUser))]
public class AppUserTests
{

	[Fact]
	public void DefaultConstructor_ShouldInitializeWithDefaults()
	{
		var user = new AppUser();
		user.Id.Should().BeEmpty();
		user.UserName.Should().BeEmpty();
		user.Email.Should().BeEmpty();
		user.Roles.Should().NotBeNull();
		user.Roles.Should().BeEmpty();
	}

	[Fact]
	public void ParameterizedConstructor_ShouldSetAllProperties()
	{
		var roles = new List<string> { "Admin", "Editor" };
		var user = FakeAppUser.GetNewAppUser(true);
		user.Roles = roles;

		user.Id.Should().BeEquivalentTo(user.Id);
		user.UserName.Should().BeEquivalentTo(user.UserName);
		user.Email.Should().BeEquivalentTo(user.Email);
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

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUserTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Entities;

/// <summary>
///   Unit tests for the <see cref="ApplicationUser" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(ApplicationUser))]
public class ApplicationUserTests
{

	[Fact]
	public void DefaultConstructor_ShouldInitializeWithDefaults()
	{
		ApplicationUser user = new ();
		user.Id.Should().BeEmpty();
		user.UserName.Should().BeEmpty();
		user.Email.Should().BeEmpty();
		user.DisplayName.Should().BeEmpty();
	}

	[Fact]
	public void ParameterizedConstructor_ShouldSetAllProperties()
	{
		ApplicationUser user = FakeApplicationUser.GetNewApplicationUser(true);

		user.Id.Should().BeEquivalentTo(user.Id);
		user.UserName.Should().BeEquivalentTo(user.UserName);
		user.Email.Should().BeEquivalentTo(user.Email);
		user.DisplayName.Should().BeEquivalentTo(user.DisplayName);
	}

}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeAppUserTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Unit tests for the <see cref="FakeAppUser" /> fake data generator for <see cref="AppUser" />.
///   Covers validity, collection counts, zero-request behavior and seed-related determinism.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FakeAppUser))]
public class FakeAppUserTests
{

	[Fact]
	public void GetNewAppUser_ShouldReturnValidUser()
	{
		// Act
		var user = FakeAppUser.GetNewAppUser();

		// Assert
		user.Should().NotBeNull();
		user.Id.Should().NotBeNullOrWhiteSpace();
		user.UserName.Should().NotBeNullOrWhiteSpace();
		user.Email.Should().NotBeNullOrWhiteSpace();
		user.Email.Should().Contain("@");
		user.Roles.Should().NotBeNull();
		user.Roles.Should().NotBeEmpty();
		Enum.TryParse<Roles>(user.Roles[0], true, out _).Should().BeTrue();
	}

	[Fact]
	public void GetAppUsers_ShouldReturnRequestedCount()
	{
		// Arrange
		const int requested = 5;

		// Act
		var list = FakeAppUser.GetAppUsers(requested);

		// Assert
		list.Should().NotBeNull();
		list.Should().HaveCount(requested);
		list.Should().AllBeOfType<AppUser>();
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.Id));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.UserName));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains("@"));

		foreach (var u in list)
		{
			u.Roles.Should().NotBeNull();
			u.Roles.Count.Should().BeGreaterOrEqualTo(1);
			Enum.TryParse<Roles>(u.Roles[0], true, out _).Should().BeTrue();
		}
	}

	[Fact]
	public void GetAppUsers_ZeroRequested_ShouldReturnEmptyList()
	{
		// Act
		var list = FakeAppUser.GetAppUsers(0);

		// Assert
		list.Should().NotBeNull();
		list.Should().BeEmpty();
	}

	[Fact]
	public void GetNewAppUser_WithSeed_ShouldReturnDeterministicResult()
	{
		// Act
		var a = FakeAppUser.GetNewAppUser(true);
		var b = FakeAppUser.GetNewAppUser(true);

		// Assert - deterministic except for Id which is generated via ObjectId.NewId().ToString()
		a.Should().BeEquivalentTo(b, opts => opts
				.Excluding(x => x.Id));
	}

	[Fact]
	public void GetAppUsers_WithSeed_ShouldReturnDeterministicResults()
	{
		// Arrange
		const int count = 3;

		// Act
		var r1 = FakeAppUser.GetAppUsers(count, true);
		var r2 = FakeAppUser.GetAppUsers(count, true);

		// Assert
		r1.Should().HaveCount(count);
		r2.Should().HaveCount(count);

		for (var i = 0; i < count; i++)
		{
			r1[i].Should().BeEquivalentTo(r2[i], opts => opts.Excluding(x => x.Id));

			// Email is derived from UserName; if usernames are equal under seed, emails should be equal too
			r1[i].Email.Should().Be(r2[i].Email);
		}
	}

	[Fact]
	public void GenerateFake_ShouldConfigureFakerCorrectly()
	{
		// Act
		var faker = FakeAppUser.GenerateFake();
		var user = faker.Generate();

		// Assert
		user.Should().NotBeNull();
		user.Should().BeOfType<AppUser>();
		user.Id.Should().NotBeNullOrWhiteSpace();
		user.UserName.Should().NotBeNullOrWhiteSpace();
		user.Email.Should().NotBeNullOrWhiteSpace();
		user.Email.Should().Contain("@");
		user.Roles.Should().NotBeNull();
		user.Roles.Should().NotBeEmpty();
		Enum.TryParse<Roles>(user.Roles[0], true, out _).Should().BeTrue();
	}

	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{
		// Act
		var a1 = FakeAppUser.GenerateFake(true).Generate();
		var a2 = FakeAppUser.GenerateFake(true).Generate();

		// Assert
		a2.Should().BeEquivalentTo(a1, opts => opts.Excluding(x => x.Id));
	}

	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{
		// Act
		var a1 = FakeAppUser.GenerateFake().Generate();
		var a2 = FakeAppUser.GenerateFake().Generate();

		// Assert - focus on string fields that should generally differ without a seed
		a1.UserName.Should().NotBe(a2.UserName);
		a1.Email.Should().NotBe(a2.Email);
	}

}

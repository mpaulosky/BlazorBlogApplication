// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeApplicationUserTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Unit tests for the <see cref="FakeApplicationUser" /> fake data generator for <see cref="ApplicationUser" />.
///   Covers validity, collection counts, zero-request behavior and seed-related determinism.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FakeApplicationUser))]
public class FakeApplicationUserTests
{

	[Fact]
	public void GetNewApplicationUser_ShouldReturnValidUser()
	{
		// Act
		var user = FakeApplicationUser.GetNewApplicationUser();

		// Assert
		user.Should().NotBeNull();
		user.Id.Should().NotBeNullOrWhiteSpace();
		user.UserName.Should().NotBeNullOrWhiteSpace();
		user.Email.Should().NotBeNullOrWhiteSpace();
		user.Email.Should().Contain("@");
		user.DisplayName.Should().NotBeNullOrWhiteSpace();
	}

	[Fact]
	public void GetApplicationUsers_ShouldReturnRequestedCount()
	{
		// Arrange
		const int requested = 5;

		// Act
		var list = FakeApplicationUser.GetApplicationUsers(requested);

		// Assert
		list.Should().NotBeNull();
		list.Should().HaveCount(requested);
		list.Should().AllBeOfType<ApplicationUser>();
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.Id));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.UserName));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains("@"));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.DisplayName));
	}

	[Fact]
	public void GetApplicationUsers_ZeroRequested_ShouldReturnEmptyList()
	{
		// Act
		var list = FakeApplicationUser.GetApplicationUsers(0);

		// Assert
		list.Should().NotBeNull();
		list.Should().BeEmpty();
	}

	[Fact]
	public void GetNewApplicationUser_WithSeed_ShouldReturnDeterministicResult()
	{
		// Act
		var a = FakeApplicationUser.GetNewApplicationUser(true);
		var b = FakeApplicationUser.GetNewApplicationUser(true);

		// Assert - deterministic except for Id which is generated via ObjectId.NewId().ToString()
		a.Should().BeEquivalentTo(b, opts => opts
				.Excluding(x => x.Id));
	}

	[Fact]
	public void GetApplicationUsers_WithSeed_ShouldReturnDeterministicResults()
	{
		// Arrange
		const int count = 3;

		// Act
		var r1 = FakeApplicationUser.GetApplicationUsers(count, true);
		var r2 = FakeApplicationUser.GetApplicationUsers(count, true);

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
		var faker = FakeApplicationUser.GenerateFake();
		var user = faker.Generate();

		// Assert
		user.Should().NotBeNull();
		user.Should().BeOfType<ApplicationUser>();
		user.Id.Should().NotBeNullOrWhiteSpace();
		user.UserName.Should().NotBeNullOrWhiteSpace();
		user.Email.Should().NotBeNullOrWhiteSpace();
		user.Email.Should().Contain("@");
		user.DisplayName.Should().NotBeNullOrWhiteSpace();
	}

	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{
		// Act
		var a1 = FakeApplicationUser.GenerateFake(true).Generate();
		var a2 = FakeApplicationUser.GenerateFake(true).Generate();

		// Assert
		a2.Should().BeEquivalentTo(a1, opts => opts.Excluding(x => x.Id));
	}

	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{
		// Act
		var a1 = FakeApplicationUser.GenerateFake().Generate();
		var a2 = FakeApplicationUser.GenerateFake().Generate();

		// Assert - focus on string fields that should generally differ without a seed
		a1.UserName.Should().NotBe(a2.UserName);
		a1.Email.Should().NotBe(a2.Email);
	}

}

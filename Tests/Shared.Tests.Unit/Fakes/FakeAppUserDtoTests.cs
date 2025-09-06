// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeAppUserDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Unit tests for the <see cref="FakeAppUserDto" /> fake data generator for <see cref="AppUserDto" />.
///   Covers validity, collection counts, zero-request behavior and seed-related determinism.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FakeAppUserDto))]
public class FakeAppUserDtoTests
{

	[Fact]
	public void GetNewAppUserDto_ShouldReturnValidDto()
	{
		// Act
		var dto = FakeAppUserDto.GetNewAppUserDto();

		// Assert
		dto.Should().NotBeNull();
		dto.Id.Should().NotBeNullOrWhiteSpace();
		dto.UserName.Should().NotBeNullOrWhiteSpace();
		dto.Email.Should().NotBeNullOrWhiteSpace();
		dto.Email.Should().Contain("@");
		dto.Roles.Should().NotBeNull();
		dto.Roles!.Should().NotBeEmpty();
		Enum.TryParse<Roles>(dto.Roles![0], true, out _).Should().BeTrue();
	}

	[Fact]
	public void GetAppUserDtos_ShouldReturnRequestedCount()
	{
		// Arrange
		const int requested = 5;

		// Act
		var list = FakeAppUserDto.GetAppUserDtos(requested);

		// Assert
		list.Should().NotBeNull();
		list.Should().HaveCount(requested);
		list.Should().AllBeOfType<AppUserDto>();
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.Id));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.UserName));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains("@"));

		foreach (var u in list)
		{
			u.Roles.Should().NotBeNull();
			u.Roles!.Count.Should().BeGreaterOrEqualTo(1);
			Enum.TryParse<Roles>(u.Roles![0], true, out _).Should().BeTrue();
		}
	}

	[Fact]
	public void GetAppUserDtos_ZeroRequested_ShouldReturnEmptyList()
	{
		// Act
		var list = FakeAppUserDto.GetAppUserDtos(0);

		// Assert
		list.Should().NotBeNull();
		list.Should().BeEmpty();
	}

	[Fact]
	public void GetNewAppUserDto_WithSeed_ShouldReturnDeterministicResult()
	{
		// Act
		var a = FakeAppUserDto.GetNewAppUserDto(true);
		var b = FakeAppUserDto.GetNewAppUserDto(true);

		// Assert - deterministic except for Id, which is generated via ObjectId.NewId()
		a.Should().BeEquivalentTo(b, opts => opts
				.Excluding(x => x.Id));
	}

	[Fact]
	public void GetAppUserDtos_WithSeed_ShouldReturnDeterministicResults()
	{
		// Arrange
		const int count = 3;

		// Act
		var r1 = FakeAppUserDto.GetAppUserDtos(count, true);
		var r2 = FakeAppUserDto.GetAppUserDtos(count, true);

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
		var faker = FakeAppUserDto.GenerateFake();
		var dto = faker.Generate();

		// Assert
		dto.Should().NotBeNull();
		dto.Should().BeOfType<AppUserDto>();
		dto.Id.Should().NotBeNullOrWhiteSpace();
		dto.UserName.Should().NotBeNullOrWhiteSpace();
		dto.Email.Should().NotBeNullOrWhiteSpace();
		dto.Email.Should().Contain("@");
		dto.Roles.Should().NotBeNull();
		dto.Roles!.Should().NotBeEmpty();
		Enum.TryParse<Roles>(dto.Roles![0], true, out _).Should().BeTrue();
	}

	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{
		// Act
		var a1 = FakeAppUserDto.GenerateFake(true).Generate();
		var a2 = FakeAppUserDto.GenerateFake(true).Generate();

		// Assert
		a2.Should().BeEquivalentTo(a1, opts => opts.Excluding(x => x.Id));
	}

	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{
		// Act
		var a1 = FakeAppUserDto.GenerateFake().Generate();
		var a2 = FakeAppUserDto.GenerateFake().Generate();

		// Assert - focus on string fields that should generally differ without a seed
		a1.UserName.Should().NotBe(a2.UserName);
		a1.Email.Should().NotBe(a2.Email);
	}

}

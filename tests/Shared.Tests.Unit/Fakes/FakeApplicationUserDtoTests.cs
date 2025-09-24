// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeApplicationUserDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Unit tests for the <see cref="FakeApplicationUserDto" /> fake data generator for <see cref="ApplicationUserDto" />.
///   Covers validity, collection counts, zero-request behavior and seed-related determinism.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FakeApplicationUserDto))]
public class FakeApplicationUserDtoTests
{

	[Fact]
	public void GetNewApplicationUserDto_ShouldReturnValidDto()
	{
		// Act
		var dto = FakeApplicationUserDto.GetNewApplicationUserDto();

		// Assert
		dto.Should().NotBeNull();
		dto.Id.Should().NotBeNullOrWhiteSpace();
		dto.UserName.Should().NotBeNullOrWhiteSpace();
		dto.Email.Should().NotBeNullOrWhiteSpace();
		dto.Email.Should().Contain("@");
		dto.DisplayName.Should().NotBeNullOrWhiteSpace();
	}

	[Fact]
	public void GetApplicationUserDtos_ShouldReturnRequestedCount()
	{
		// Arrange
		const int requested = 5;

		// Act
		var list = FakeApplicationUserDto.GetApplicationUserDtos(requested);

		// Assert
		list.Should().NotBeNull();
		list.Should().HaveCount(requested);
		list.Should().AllBeOfType<ApplicationUserDto>();
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.Id));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.UserName));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains("@"));
		list.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.DisplayName));

	}

	[Fact]
	public void GetApplicationUserDtos_ZeroRequested_ShouldReturnEmptyList()
	{
		// Act
		var list = FakeApplicationUserDto.GetApplicationUserDtos(0);

		// Assert
		list.Should().NotBeNull();
		list.Should().BeEmpty();
	}

	[Fact]
	public void GetNewApplicationUserDto_WithSeed_ShouldReturnDeterministicResult()
	{
		// Act
		var a = FakeApplicationUserDto.GetNewApplicationUserDto(true);
		var b = FakeApplicationUserDto.GetNewApplicationUserDto(true);

		// Assert - deterministic except for Id, which is generated via ObjectId.NewId()
		a.Should().BeEquivalentTo(b, opts => opts
				.Excluding(x => x.Id));
	}

	[Fact]
	public void GetApplicationUserDtos_WithSeed_ShouldReturnDeterministicResults()
	{
		// Arrange
		const int count = 3;

		// Act
		var r1 = FakeApplicationUserDto.GetApplicationUserDtos(count, true);
		var r2 = FakeApplicationUserDto.GetApplicationUserDtos(count, true);

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
		var faker = FakeApplicationUserDto.GenerateFake();
		var dto = faker.Generate();

		// Assert
		dto.Should().NotBeNull();
		dto.Should().BeOfType<ApplicationUserDto>();
		dto.Id.Should().NotBeNullOrWhiteSpace();
		dto.UserName.Should().NotBeNullOrWhiteSpace();
		dto.Email.Should().NotBeNullOrWhiteSpace();
		dto.Email.Should().Contain("@");
		dto.DisplayName.Should().NotBeNullOrWhiteSpace();
	}

	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{
		// Act
		var a1 = FakeApplicationUserDto.GenerateFake(true).Generate();
		var a2 = FakeApplicationUserDto.GenerateFake(true).Generate();

		// Assert
		a2.Should().BeEquivalentTo(a1, opts => opts.Excluding(x => x.Id));
	}

	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{
		// Act
		var a1 = FakeApplicationUserDto.GenerateFake().Generate();
		var a2 = FakeApplicationUserDto.GenerateFake().Generate();

		// Assert - focus on string fields that should generally differ without a seed
		a1.UserName.Should().NotBe(a2.UserName);
		a1.Email.Should().NotBe(a2.Email);
	}

}

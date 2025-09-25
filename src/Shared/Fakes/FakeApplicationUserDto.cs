// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeApplicationUserDto.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Provides methods to generate fake data for the <see cref="ApplicationUserDto" /> class.
/// </summary>
public static class FakeApplicationUserDto
{

	private const int SEED = 621;

	/// <summary>
	///   Generates a new fake <see cref="ApplicationUserDto" /> object.
	/// </summary>
	/// <param name="useSeed">Determines whether a fixed seed should be used for consistent outputs.</param>
	/// <returns>A single fake <see cref="ApplicationUserDto" /> object.</returns>
	public static ApplicationUserDto GetNewApplicationUserDto(bool useSeed = false)
	{

		return GenerateFake(useSeed).Generate();

	}

	/// <summary>
	///   Generates a list of fake <see cref="ApplicationUserDto" /> objects.
	/// </summary>
	/// <param name="numberRequested">The number of <see cref="ApplicationUserDto" /> objects to generate.</param>
	/// <param name="useSeed">Determines whether a fixed seed should be used for consistent outputs.</param>
	/// <returns>A list of fake <see cref="ApplicationUserDto" /> objects.</returns>
	public static List<ApplicationUserDto> GetApplicationUserDtos(int numberRequested, bool useSeed = false)
	{

		return GenerateFake(useSeed).Generate(numberRequested);

	}

	/// <summary>
	///   Configures a Faker instance to generate fake <see cref="ApplicationUserDto" /> objects.
	/// </summary>
	/// <param name="useSeed">Determines whether a fixed seed should be used for consistent outputs.</param>
	/// <returns>A configured <see cref="Faker" /> instance for <see cref="ApplicationUserDto" /> objects.</returns>
	internal static Faker<ApplicationUserDto> GenerateFake(bool useSeed = false)
	{

		Faker<ApplicationUserDto>? fake = new Faker<ApplicationUserDto>()
				.RuleFor(x => x.Id, Guid.CreateVersion7().ToString())
				.RuleFor(x => x.UserName, f => f.Name.FullName())
				.RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.UserName))
				.RuleFor(x => x.DisplayName, (f, u) => f.Name.FirstName())
				.RuleFor(x => x.EmailConfirmed, f => f.Random.Bool());

		return useSeed ? fake.UseSeed(SEED) : fake;

	}

}
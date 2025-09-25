// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeApplicationUser.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Provides fake data generation methods for the <see cref="ApplicationUser" /> entity.
/// </summary>
public static class FakeApplicationUser
{

	private const int SEED = 621;

	/// <summary>
	///   Generates a new fake <see cref="ApplicationUser" /> object.
	/// </summary>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A single fake <see cref="ApplicationUser" /> object.</returns>
	public static ApplicationUser GetNewApplicationUser(bool useSeed = false)
	{

		return GenerateFake(useSeed).Generate();

	}

	/// <summary>
	///   Generates a list of fake <see cref="ApplicationUser" /> objects.
	/// </summary>
	/// <param name="numberRequested">The number of <see cref="ApplicationUser" /> objects to generate.</param>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A list of fake <see cref="ApplicationUser" /> objects.</returns>
	public static List<ApplicationUser> GetApplicationUsers(int numberRequested, bool useSeed = false)
	{

		return GenerateFake(useSeed).Generate(numberRequested);

	}

	/// <summary>
	///   Generates a configured <see cref="Faker" /> for creating fake <see cref="ApplicationUser" /> objects.
	/// </summary>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A configured <see cref="Faker{ApplicationUser}" /> instance.</returns>
	internal static Faker<ApplicationUser> GenerateFake(bool useSeed = false)
	{

		Faker<ApplicationUser>? fake = new Faker<ApplicationUser>()
				.RuleFor(x => x.Id, Guid.CreateVersion7().ToString())
				.RuleFor(x => x.UserName, f => f.Name.FullName())
				.RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.UserName))
				.RuleFor(x => x.DisplayName, (f, u) => f.Name.FirstName())
				.RuleFor(x => x.EmailConfirmed, f => f.Random.Bool())

				// SecurityStamp and ConcurrencyStamp are normally initialized by IdentityUser
				// using non-deterministic GUIDs. To make seeded generation deterministic
				// (so tests that expect determinism can pass) we explicitly configure
				// them using the Faker's random generator which honors `UseSeed()`.
				.RuleFor(x => x.SecurityStamp, f => f.Random.Uuid().ToString())
				.RuleFor(x => x.ConcurrencyStamp, f => f.Random.Uuid().ToString());

		return useSeed ? fake.UseSeed(SEED) : fake;

	}

}
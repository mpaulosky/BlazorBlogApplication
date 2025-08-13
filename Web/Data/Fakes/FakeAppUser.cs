// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeAppUser.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlog
// Project Name :  Web
// =======================================================

using Bogus;

using MongoDB.Bson;

using Web.Data.Entities;
using Web.Data.Enums;

namespace Web.Data.Fakes;

/// <summary>
///   Provides fake data generation methods for the <see cref="AppUser" /> entity.
/// </summary>
public static class FakeAppUser
{

	/// <summary>
	///   Generates a new fake <see cref="AppUser" /> object.
	/// </summary>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A single fake <see cref="AppUser" /> object.</returns>
	public static AppUser GetNewAppUser(bool useSeed = false)
	{

		return GenerateFake(useSeed).Generate();

	}

	/// <summary>
	///   Generates a list of fake <see cref="AppUser" /> objects.
	/// </summary>
	/// <param name="numberRequested">The number of <see cref="AppUser" /> objects to generate.</param>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A list of fake <see cref="AppUser" /> objects.</returns>
	public static List<AppUser> GetAppUsers(int numberRequested, bool useSeed = false)
	{

		return GenerateFake(useSeed).Generate(numberRequested);

	}

	/// <summary>
	///   Generates a configured <see cref="Faker" /> for creating fake <see cref="AppUser" /> objects.
	/// </summary>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A configured <see cref="Faker{AppUser}" /> instance.</returns>
	internal static Faker<AppUser> GenerateFake(bool useSeed = false)
	{

		const int seed = 621;

		var fake = new Faker<AppUser>()
				.RuleFor(x => x.Id, ObjectId.GenerateNewId().ToString())
				.RuleFor(x => x.UserName, f => f.Name.FullName())
				.RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.UserName))
				.RuleFor(x => x.Roles, f => [f.Random.Enum<Roles>().ToString()]);

		return useSeed ? fake.UseSeed(seed) : fake;

	}

}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeCategoryDto.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlog
// Project Name :  Web
// =======================================================

using Bogus;

using MongoDB.Bson;

using Web.Data.Helpers;
using Web.Data.Models;

namespace Web.Data.Fakes;

/// <summary>
///   Provides fake data generation methods for the <see cref="CategoryDto" /> entity.
/// </summary>
public static class FakeCategoryDto
{

	/// <summary>
	///   Generates a new fake <see cref="CategoryDto" /> object.
	/// </summary>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A single fake <see cref="CategoryDto" /> object.</returns>
	public static CategoryDto GetNewCategoryDto(bool useSeed = false)
	{

		return GenerateFake(useSeed).Generate();

	}

	/// <summary>
	///   Generates a list of fake <see cref="CategoryDto" /> objects.
	/// </summary>
	/// <param name="numberRequested">The number of <see cref="CategoryDto" /> objects to generate.</param>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A list of fake <see cref="CategoryDto" /> objects.</returns>
	public static List<CategoryDto> GetCategoriesDto(int numberRequested, bool useSeed = false)
	{

		return GenerateFake(useSeed).Generate(numberRequested);

	}

	/// <summary>
	///   Generates a configured <see cref="Faker" /> instance to create fake <see cref="CategoryDto" /> objects.
	/// </summary>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A configured <see cref="Faker{CategoryDto}" /> instance.</returns>
	internal static Faker<CategoryDto> GenerateFake(bool useSeed = false)
	{

		const int seed = 621;

		var fake = new Faker<CategoryDto>()
				.RuleFor(x => x.Id, ObjectId.GenerateNewId())
				.RuleFor(x => x.CategoryName, _ => Helpers.Helpers.GetRandomCategoryName())
				.RuleFor(x => x.CreatedOn, _ => Helpers.Helpers.GetStaticDate())
				.RuleFor(f => f.ModifiedOn, _ => Helpers.Helpers.GetStaticDate());

		return useSeed ? fake.UseSeed(seed) : fake;

	}

}
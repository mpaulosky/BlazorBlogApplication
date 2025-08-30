// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeArticle.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlog
// Project Name :  Web
// =======================================================

using Shared.Helpers;

namespace Shared.Fakes;

/// <summary>
///   Provides fake data generation methods for the <see cref="Article" /> entity.
/// </summary>
public static class FakeArticle
{

	/// <summary>
	///   Generates a new fake <see cref="Article" /> object.
	/// </summary>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A single fake <see cref="Article" /> object.</returns>
	public static Article GetNewArticle(bool useSeed = false)
	{

		return GenerateFake(useSeed).Generate();

	}

	/// <summary>
	///   Generates a list of fake <see cref="Article" /> objects.
	/// </summary>
	/// <param name="numberRequested">The number of <see cref="Article" /> objects to generate.</param>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>A list of fake <see cref="Article" /> objects.</returns>
	public static List<Article> GetArticles(int numberRequested, bool useSeed = false)
	{

		var articles = new List<Article>();

		for (var i = 0; i < numberRequested; i++)
		{

			var article = GenerateFake(useSeed).Generate();
			articles.Add(article);

		}

		return articles;

	}

	/// <summary>
	///   Generates a Faker instance configured to generate fake <see cref="Article" /> objects.
	/// </summary>
	/// <param name="useSeed">Indicates whether to apply a fixed seed for deterministic results.</param>
	/// <returns>Configured Faker <see cref="Article" /> instance.</returns>
	internal static Faker<Article> GenerateFake(bool useSeed = false)
	{
		var fake = new Faker<Article>()
				.RuleFor(f => f.Id, ObjectId.GenerateNewId())
				.RuleFor(f => f.Title, f => f.WaffleTitle())
				.RuleFor(f => f.Introduction, f => f.Lorem.Sentence())
				.RuleFor(f => f.Content, f => f.WaffleMarkdown(5))
				.RuleFor(f => f.UrlSlug, (_, f) => f.Title.GetSlug())
				.RuleFor(f => f.CoverImageUrl, f => f.Image.PicsumUrl())
				.RuleFor(f => f.CreatedOn, _ => Helpers.Helpers.GetStaticDate())
				.RuleFor(f => f.ModifiedOn, _ => Helpers.Helpers.GetStaticDate())
				.RuleFor(f => f.IsPublished, f => f.Random.Bool())
				.RuleFor(f => f.PublishedOn, (_, f) => f.IsPublished ? Helpers.Helpers.GetStaticDate() : null)
				.RuleFor(f => f.IsArchived, f => f.Random.Bool())
				.RuleFor(f => f.Category, _ => FakeCategoryDto.GetNewCategoryDto(useSeed))
				.RuleFor(f => f.Author, _ => FakeAppUserDto.GetNewAppUserDto(useSeed));


		const int seed = 621;

		return useSeed ? fake.UseSeed(seed) : fake;

	}

}

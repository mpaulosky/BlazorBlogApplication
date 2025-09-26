// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeArticle.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Provides fake data generation methods for the <see cref="Article" /> entity.
/// </summary>
public static class FakeArticle
{

	private const int SEED = 621;

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
		List<Article> articles = new ();

		// Reuse a single Faker instance within this call to ensure unique items in the list.
		// For seeded runs, create a fresh seeded instance per call so repeated calls yield the same sequence.
		Faker<Article>? faker = GenerateFake(useSeed);

		// Ensure CreatedOn/ModifiedOn are deterministic for seeded list generation across separate calls
		if (useSeed)
		{
			faker = faker
					.RuleFor(f => f.CreatedOn, _ => GetStaticDate())
					.RuleFor(f => f.ModifiedOn, _ => null);
		}

		for (int i = 0; i < numberRequested; i++)
		{
			Article? article = faker.Generate();
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
		Faker<Article>? fake = new Faker<Article>()
				.RuleFor(a => a.Id, (_, __) => Guid.CreateVersion7())
				.RuleFor(a => a.Title, (f, _) => f.WaffleTitle())
				.RuleFor(a => a.Introduction, (f, _) => f.Lorem.Sentence())
				.RuleFor(a => a.Content, (f, _) => f.WaffleMarkdown(5))
				.RuleFor(a => a.UrlSlug, (f, a) => a.Title.GetSlug())
				.RuleFor(a => a.CoverImageUrl, (f, _) => f.Image.PicsumUrl())
				.RuleFor(a => a.IsPublished, (f, _) => f.Random.Bool())
				.RuleFor(a => a.PublishedOn, (f, a) => a.IsPublished ? DateTime.Now : null)
				.RuleFor(a => a.IsArchived, (f, _) => f.Random.Bool())
				.RuleFor(a => a.CategoryId, (f, _) => FakeCategory.GetNewCategory(useSeed).Id)
				.RuleFor(a => a.AuthorId, (f, _) => Guid.CreateVersion7().ToString())
				.RuleFor(a => a.Category, (f, _) => FakeCategory.GetNewCategory(useSeed))
				.RuleFor(a => a.Author, (f, _) => FakeApplicationUser.GetNewApplicationUser(useSeed))
				.RuleFor(a => a.CreatedOn, (_, __) => DateTime.Now)
				.RuleFor(a => a.ModifiedOn, (_, __) => DateTime.Now);

		return useSeed ? fake.UseSeed(SEED) : fake;

	}

}
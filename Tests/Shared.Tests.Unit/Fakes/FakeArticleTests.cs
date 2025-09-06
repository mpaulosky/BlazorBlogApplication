// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeArticleTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Unit tests for the <see cref="FakeArticle" /> fake data generator for <see cref="Article" />.
///   Covers validity, collection counts, zero-request behavior and seed-related determinism.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FakeArticle))]
public class FakeArticleTests
{

	[Fact]
	public void GetNewArticle_ShouldReturnValidArticle()
	{
		// Act
		var article = FakeArticle.GetNewArticle();

		// Assert
		article.Should().NotBeNull();
		article.Id.Should().NotBe(ObjectId.Empty);
		article.Title.Should().NotBeNullOrWhiteSpace();
		article.Introduction.Should().NotBeNullOrWhiteSpace();
		article.Content.Should().NotBeNullOrWhiteSpace();
		article.UrlSlug.Should().Be(article.Title.GetSlug());
		article.CoverImageUrl.Should().NotBeNullOrWhiteSpace();
		article.CreatedOn.Should().Be(GetStaticDate());
		article.ModifiedOn.Should().Be(GetStaticDate());
		article.Category.Should().NotBeNull();
		article.Author.Should().NotBeNull();

		if (article.IsPublished)
		{
			article.PublishedOn.Should().Be(GetStaticDate());
		}
		else
		{
			article.PublishedOn.Should().BeNull();
		}
	}

	[Fact]
	public void GetArticles_ShouldReturnRequestedCount()
	{
		// Arrange
		const int requested = 5;

		// Act
		var articles = FakeArticle.GetArticles(requested);

		// Assert
		articles.Should().NotBeNull();
		articles.Should().HaveCount(requested);

		foreach (var a in articles)
		{
			a.Id.Should().NotBe(ObjectId.Empty);
			a.Title.Should().NotBeNullOrWhiteSpace();
			a.UrlSlug.Should().Be(a.Title.GetSlug());
			a.CreatedOn.Should().Be(GetStaticDate());
			a.ModifiedOn.Should().Be(GetStaticDate());
			a.Category.Should().NotBeNull();
			a.Author.Should().NotBeNull();

			if (a.IsPublished)
			{
				a.PublishedOn.Should().Be(GetStaticDate());
			}
			else
			{
				a.PublishedOn.Should().BeNull();
			}
		}
	}

	[Fact]
	public void GetArticles_ZeroRequested_ShouldReturnEmptyList()
	{
		// Act
		var articles = FakeArticle.GetArticles(0);

		// Assert
		articles.Should().NotBeNull();
		articles.Should().BeEmpty();
	}

	[Fact]
	public void GetNewArticle_WithSeed_ShouldReturnDeterministicResult()
	{
		// Act
		var a = FakeArticle.GetNewArticle(true);
		var b = FakeArticle.GetNewArticle(true);

		// Assert - deterministic except for Id and some external random URL fields and nested complex types
		a.Should().BeEquivalentTo(b, opts => opts
				.Excluding(x => x.Id)
				.Excluding(x => x.Title)
				.Excluding(x => x.Introduction)
				.Excluding(x => x.Content)
				.Excluding(x => x.UrlSlug)
				.Excluding(x => x.CoverImageUrl)
				.Excluding(x => x.Category)
				.Excluding(x => x.Author));
	}

	[Theory]
	[InlineData(1)]
	[InlineData(5)]
	[InlineData(10)]
	public void GetArticles_ShouldReturnRequestedNumberOfArticles(int count)
	{
		// Act
		var results = FakeArticle.GetArticles(count);

		// Assert
		results.Should().NotBeNull();
		results.Should().HaveCount(count);
		results.Should().AllBeOfType<Article>();
		results.Should().OnlyContain(a => !string.IsNullOrWhiteSpace(a.Title));
		results.Should().OnlyContain(a => a.Id != ObjectId.Empty);
		results.Should().OnlyContain(a => a.CreatedOn == GetStaticDate());
		results.Should().OnlyContain(a => a.ModifiedOn == GetStaticDate());
	}

	[Fact]
	public void GetArticles_WithSeed_ShouldReturnDeterministicResults()
	{
		// Arrange
		const int count = 3;

		// Act
		var r1 = FakeArticle.GetArticles(count, true);
		var r2 = FakeArticle.GetArticles(count, true);

		// Assert
		r1.Should().HaveCount(count);
		r2.Should().HaveCount(count);

		for (var i = 0; i < count; i++)
		{
			r1[i].Should().BeEquivalentTo(r2[i], opts => opts
					.Excluding(x => x.Id)
					.Excluding(x => x.Title)
					.Excluding(x => x.Introduction)
					.Excluding(x => x.Content)
					.Excluding(x => x.UrlSlug)
					.Excluding(x => x.CoverImageUrl)
					.Excluding(x => x.Category)
					.Excluding(x => x.Author));

			// Dates should always match
			r1[i].CreatedOn.Should().Be(r2[i].CreatedOn);
			r1[i].ModifiedOn.Should().Be(r2[i].ModifiedOn);

			// PublishedOn should match under seeded generation
			(r1[i].PublishedOn == r2[i].PublishedOn).Should().BeTrue();
		}
	}

	[Fact]
	public void GenerateFake_ShouldConfigureFakerCorrectly()
	{
		// Act
		var faker = FakeArticle.GenerateFake();
		var article = faker.Generate();

		// Assert
		article.Should().NotBeNull();
		article.Id.Should().NotBe(ObjectId.Empty);
		article.Title.Should().NotBeNullOrWhiteSpace();
		article.UrlSlug.Should().Be(article.Title.GetSlug());
		article.CreatedOn.Should().Be(GetStaticDate());
		article.ModifiedOn.Should().Be(GetStaticDate());
		article.Category.Should().NotBeNull();
		article.Author.Should().NotBeNull();
	}

	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{
		// Act
		var a1 = FakeArticle.GenerateFake(true).Generate();
		var a2 = FakeArticle.GenerateFake(true).Generate();

		// Assert
		a2.Should().BeEquivalentTo(a1, opts => opts
				.Excluding(x => x.Id)
				.Excluding(x => x.Title)
				.Excluding(x => x.Introduction)
				.Excluding(x => x.Content)
				.Excluding(x => x.UrlSlug)
				.Excluding(x => x.CoverImageUrl)
				.Excluding(x => x.Category)
				.Excluding(x => x.Author));
	}

	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{
		// Act
		var a1 = FakeArticle.GenerateFake().Generate();
		var a2 = FakeArticle.GenerateFake().Generate();

		// Assert - focus on string fields that should generally differ without a seed
		a1.Title.Should().NotBe(a2.Title);
		a1.Introduction.Should().NotBe(a2.Introduction);
	}

}

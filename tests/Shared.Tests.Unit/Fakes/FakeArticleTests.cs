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
		article.Category.Should().NotBeNull();
		article.Author.Should().NotBeNull();

		if (article.IsPublished)
		{
			article.PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
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
			a.Category.Should().NotBeNull();
			a.Author.Should().NotBeNull();

			if (a.IsPublished)
			{
				a.PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
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
		article.Category.Should().NotBeNull();
		article.Author.Should().NotBeNull();

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
		a1.Content.Should().NotBe(a2.Content);
		a1.UrlSlug.Should().NotBe(a2.UrlSlug);
		a1.CoverImageUrl.Should().NotBe(a2.CoverImageUrl);
	}

}

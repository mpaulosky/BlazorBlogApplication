// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeArticleDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Unit tests for the <see cref="FakeArticleDto" /> fake data generator for <see cref="ArticleDto" />.
///   Covers validity, collection counts, zero-request behavior and seed-related determinism.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FakeArticleDto))]
public class FakeArticleDtoTests
{

	[Fact]
	public void GetNewArticleDto_ShouldReturnValidDto()
	{
		// Act
		var dto = FakeArticleDto.GetNewArticleDto();

		// Assert
		dto.Should().NotBeNull();
		dto.Id.Should().NotBe(ObjectId.Empty);
		dto.Title.Should().NotBeNullOrWhiteSpace();
		dto.Introduction.Should().NotBeNullOrWhiteSpace();
		dto.Content.Should().NotBeNullOrWhiteSpace();
		dto.UrlSlug.Should().Be(dto.Title.GetSlug());

		// CoverImageUrl is generated with Picsum and falls back to empty string; ensure it's not null
		dto.CoverImageUrl.Should().NotBeNull();
		dto.Category.Should().NotBeNull();
		dto.Author.Should().NotBeNull();

		if (dto.IsPublished)
		{
			dto.PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
		}
		else
		{
			dto.PublishedOn.Should().BeNull();
		}
	}

	[Fact]
	public void GetArticleDtos_ShouldReturnRequestedCount()
	{
		// Arrange
		const int requested = 5;

		// Act
		var list = FakeArticleDto.GetArticleDtos(requested);

		// Assert
		list.Should().NotBeNull();
		list.Should().HaveCount(requested);

		foreach (var dto in list)
		{
			dto.Id.Should().NotBe(ObjectId.Empty);
			dto.Title.Should().NotBeNullOrWhiteSpace();
			dto.Introduction.Should().NotBeNullOrWhiteSpace();
			dto.Content.Should().NotBeNullOrWhiteSpace();
			dto.UrlSlug.Should().Be(dto.Title.GetSlug());
			dto.CoverImageUrl.Should().NotBeNull();
			dto.Category.Should().NotBeNull();
			dto.Author.Should().NotBeNull();

			if (dto.IsPublished)
			{
				dto.PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			}
			else
			{
				dto.PublishedOn.Should().BeNull();
			}
		}
	}

	[Fact]
	public void GetArticleDtos_ZeroRequested_ShouldReturnEmptyList()
	{
		// Act
		var list = FakeArticleDto.GetArticleDtos(0);

		// Assert
		list.Should().NotBeNull();
		list.Should().BeEmpty();
	}

	[Fact]
	public void GetNewArticleDto_WithSeed_ShouldReturnDeterministicResult()
	{

		// Act
		const int count = 2;
		var results = FakeArticleDto.GetArticleDtos(count, true);

		// Assert

		for (var i = 0; i < count; i++)
		{
			results[i].Should().NotBeNull();
			results[i].Id.Should().NotBe(ObjectId.Empty);
			results[i].Title.Should().NotBeNullOrWhiteSpace();
			results[i].Introduction.Should().NotBeNullOrWhiteSpace();
			results[i].Content.Should().NotBeNullOrWhiteSpace();
			results[i].UrlSlug.Should().Be(results[i].Title.GetSlug());
			results[i].CoverImageUrl.Should().NotBeNull();
			results[i].Category.Should().NotBeNull();
			results[i].Author.Should().NotBeNull();
			results[i].CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			results[i].ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			results[i].CanEdit.Should().BeFalse();

			if (results[i].IsPublished)
			{
				results[i].PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			}
			else
			{
				results[i].PublishedOn.Should().BeNull();
			}

		}

		results[0].Title.Should().NotBe(results[1].Title);
		results[0].Introduction.Should().NotBe(results[1].Introduction);

	}

	[Theory]
	[InlineData(2)]
	[InlineData(5)]
	[InlineData(10)]
	public void GetArticleDtos_WithOutSeed_ShouldReturnRequestedNumberOfDtos(int count)
	{

		// Act
		var results = FakeArticleDto.GetArticleDtos(count);

		// Assert

		for (var i = 0; i < count; i++)
		{
			results[i].Should().NotBeNull();
			results[i].Id.Should().NotBe(ObjectId.Empty);
			results[i].Title.Should().NotBeNullOrWhiteSpace();
			results[i].Introduction.Should().NotBeNullOrWhiteSpace();
			results[i].Content.Should().NotBeNullOrWhiteSpace();
			results[i].UrlSlug.Should().Be(results[i].Title.GetSlug());
			results[i].CoverImageUrl.Should().NotBeNull();
			results[i].Category.Should().NotBeNull();
			results[i].Author.Should().NotBeNull();
			results[i].CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			results[i].ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			results[i].IsArchived.Should().BeFalse();
			results[i].CanEdit.Should().BeFalse();

			if (results[i].IsPublished)
			{
				results[i].PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			}
			else
			{
				results[i].PublishedOn.Should().BeNull();
			}
		}

		results[0].Title.Should().NotBe(results[1].Title);
		results[0].Introduction.Should().NotBe(results[1].Introduction);

	}

	[Fact]
	public void GetArticleDtos_WithSeed_Should_ReturnDeterministicResults()
	{

		// Act
		const int count = 2;
		var results = FakeArticleDto.GetArticleDtos(count, true);

		// Assert

		for (var i = 0; i < count; i++)
		{
			results[i].Should().NotBeNull();
			results[i].Id.Should().NotBe(ObjectId.Empty);
			results[i].Title.Should().NotBeNullOrWhiteSpace();
			results[i].Introduction.Should().NotBeNullOrWhiteSpace();
			results[i].Content.Should().NotBeNullOrWhiteSpace();
			results[i].UrlSlug.Should().Be(results[i].Title.GetSlug());
			results[i].CoverImageUrl.Should().NotBeNull();
			results[i].Category.Should().NotBeNull();
			results[i].Author.Should().NotBeNull();
			results[i].CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			results[i].ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			results[i].IsArchived.Should().BeFalse();
			results[i].CanEdit.Should().BeFalse();

			if (results[i].IsPublished)
			{
				results[i].PublishedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
			}
			else
			{
				results[i].PublishedOn.Should().BeNull();
			}
		}

		results[0].Title.Should().NotBe(results[1].Title);
		results[0].Introduction.Should().NotBe(results[1].Introduction);

	}

	[Fact]
	public void GenerateFake_ShouldConfigureFakerCorrectly()
	{
		// Act
		var faker = FakeArticleDto.GenerateFake();
		var dto = faker.Generate();

		// Assert
		dto.Should().NotBeNull();
		dto.Should().BeOfType<ArticleDto>();
		dto.Id.Should().NotBe(ObjectId.Empty);
		dto.Title.Should().NotBeNullOrWhiteSpace();
		dto.UrlSlug.Should().Be(dto.Title.GetSlug());
		dto.Category.Should().NotBeNull();
		dto.Author.Should().NotBeNull();
	}

	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{

		// Act
		var articles = FakeArticleDto.GenerateFake(true).Generate(2);

		// Assert - focus on string fields that should generally differ without a seed
		articles[0].Title.Should().NotBe(articles[1].Title);
		articles[0].Introduction.Should().NotBe(articles[1].Introduction);
		articles[0].Content.Should().NotBe(articles[1].Content);
		articles[0].UrlSlug.Should().NotBe(articles[1].UrlSlug);
		articles[0].CoverImageUrl.Should().NotBe(articles[1].CoverImageUrl);
		articles[0].Category.Should().NotBe(articles[1].Category);
		articles[0].Author.Should().NotBe(articles[1].Author);

	}

	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{

		// Act
		var articles = FakeArticleDto.GenerateFake().Generate(2);

		// Assert - focus on string fields that should generally differ without a seed
		articles[0].Title.Should().NotBe(articles[1].Title);
		articles[0].Introduction.Should().NotBe(articles[1].Introduction);
		articles[0].Content.Should().NotBe(articles[1].Content);
		articles[0].UrlSlug.Should().NotBe(articles[1].UrlSlug);
		articles[0].CoverImageUrl.Should().NotBe(articles[1].CoverImageUrl);
		articles[0].Category.Should().NotBe(articles[1].Category);
		articles[0].Author.Should().NotBe(articles[1].Author);

	}

}

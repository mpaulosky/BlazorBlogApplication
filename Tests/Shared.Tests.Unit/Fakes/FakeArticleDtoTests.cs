// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeArticleDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Shared.Fakes;

/// <summary>
/// Unit tests for the <see cref="FakeArticleDto"/> fake data generator for <see cref="ArticleDto"/>.
/// Covers validity, collection counts, zero-request behavior and seed-related determinism.
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
		dto.CreatedOn.Should().Be(GetStaticDate());
		dto.ModifiedOn.Should().Be(GetStaticDate());
		dto.Category.Should().NotBeNull();
		dto.Author.Should().NotBeNull();

		if (dto.IsPublished)
		{
			dto.PublishedOn.Should().Be(GetStaticDate());
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
			dto.CreatedOn.Should().Be(GetStaticDate());
			dto.ModifiedOn.Should().Be(GetStaticDate());
			dto.Category.Should().NotBeNull();
			dto.Author.Should().NotBeNull();
			if (dto.IsPublished)
			{
				dto.PublishedOn.Should().Be(GetStaticDate());
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
		var a = FakeArticleDto.GetNewArticleDto(true);
		var b = FakeArticleDto.GetNewArticleDto(true);

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
	public void GetArticleDtos_ShouldReturnRequestedNumberOfDtos(int count)
	{
		// Act
		var results = FakeArticleDto.GetArticleDtos(count);

		// Assert
		results.Should().NotBeNull();
		results.Should().HaveCount(count);
		results.Should().AllBeOfType<ArticleDto>();
		results.Should().OnlyContain(a => !string.IsNullOrWhiteSpace(a.Title));
		results.Should().OnlyContain(a => a.Id != ObjectId.Empty);
		results.Should().OnlyContain(a => a.CreatedOn == GetStaticDate());
		results.Should().OnlyContain(a => a.ModifiedOn == GetStaticDate());
	}

	[Fact]
	public void GetArticleDtos_WithSeed_ShouldReturnDeterministicResults()
	{
		// Arrange
		const int count = 3;

		// Act
		var r1 = FakeArticleDto.GetArticleDtos(count, true);
		var r2 = FakeArticleDto.GetArticleDtos(count, true);

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
		var faker = FakeArticleDto.GenerateFake();
		var dto = faker.Generate();

		// Assert
		dto.Should().NotBeNull();
		dto.Should().BeOfType<ArticleDto>();
		dto.Id.Should().NotBe(ObjectId.Empty);
		dto.Title.Should().NotBeNullOrWhiteSpace();
		dto.UrlSlug.Should().Be(dto.Title.GetSlug());
		dto.CreatedOn.Should().Be(GetStaticDate());
		dto.ModifiedOn.Should().Be(GetStaticDate());
		dto.Category.Should().NotBeNull();
		dto.Author.Should().NotBeNull();
	}

	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{
		// Act
		var a1 = FakeArticleDto.GenerateFake(true).Generate();
		var a2 = FakeArticleDto.GenerateFake(true).Generate();

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
		var a1 = FakeArticleDto.GenerateFake().Generate();
		var a2 = FakeArticleDto.GenerateFake().Generate();

		// Assert - focus on string fields that should generally differ without a seed
		a1.Title.Should().NotBe(a2.Title);
		a1.Introduction.Should().NotBe(a2.Introduction);
	}
}
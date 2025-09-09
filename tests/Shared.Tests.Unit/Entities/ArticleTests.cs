// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Entities;

/// <summary>
///   Unit tests for the <see cref="Article" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Article))]
public class ArticleTests
{

	[Fact]
	public void DefaultConstructor_ShouldInitializeWithDefaults()
	{
		// Arrange & Act
		var article = new Article();

		// Assert
		article.Title.Should().Be("");
		article.Introduction.Should().Be("");
		article.Content.Should().Be("");
		article.CoverImageUrl.Should().Be("");
		article.UrlSlug.Should().Be("");
		article.Author.Should().Be(AppUserDto.Empty);
		article.Category.Should().Be(CategoryDto.Empty);
		article.IsPublished.Should().BeFalse();
		article.PublishedOn.Should().BeNull();
		article.IsArchived.Should().BeFalse();
	}

	[Fact]
	public void ParameterizedConstructor_ShouldSetAllProperties()
	{
		// Arrange
		var fake = FakeArticle.GetNewArticle(true);

		// Act
		var article = new Article(
				fake.Title,
				fake.Introduction,
				fake.Content,
				fake.CoverImageUrl,
				fake.UrlSlug,
				fake.Author,
				fake.Category,
				fake.IsPublished,
				fake.PublishedOn,
				fake.IsArchived);

		// Assert
		article.Title.Should().Be(fake.Title);
		article.Introduction.Should().Be(fake.Introduction);
		article.Content.Should().Be(fake.Content);
		article.CoverImageUrl.Should().Be(fake.CoverImageUrl);
		article.UrlSlug.Should().Be(fake.UrlSlug);
		article.Author.Should().Be(fake.Author);
		article.Category.Should().Be(fake.Category);
		article.IsPublished.Should().Be(fake.IsPublished);
		article.PublishedOn.Should().Be(fake.PublishedOn);
		article.IsArchived.Should().Be(fake.IsArchived);
	}

	[Fact]
	public void Update_ShouldChangeAllProperties()
	{
		// Arrange
		var article = FakeArticle.GetNewArticle(true);
		const string newTitle = "Updated Title";
		const string newIntro = "Updated Intro";
		const string newContent = "Updated Content";
		const string newCover = "https://newcover.com/image.png";
		const string newSlug = "updated-slug";
		var newCategory = FakeCategoryDto.GetNewCategoryDto(true);
		const bool newPublished = true;
		var newPublishedOn = DateTime.UtcNow;
		const bool newArchived = true;

		// Act
		article.Update(newTitle, newIntro, newContent, newCover, newSlug, newCategory, newPublished, newPublishedOn,
				newArchived);

		// Assert
		article.Title.Should().Be(newTitle);
		article.Introduction.Should().Be(newIntro);
		article.Content.Should().Be(newContent);
		article.CoverImageUrl.Should().Be(newCover);
		article.UrlSlug.Should().Be(newSlug);
		article.Category.Should().Be(newCategory);
		article.IsPublished.Should().Be(newPublished);
		article.PublishedOn.Should().Be(newPublishedOn);
		article.IsArchived.Should().Be(newArchived);
	}

	[Fact]
	public void Publish_ShouldSetIsPublishedAndPublishedOn()
	{
		// Arrange
		var article = FakeArticle.GetNewArticle(true);
		var publishDate = DateTime.UtcNow;

		// Act
		article.Publish(publishDate);

		// Assert
		article.IsPublished.Should().BeTrue();
		article.PublishedOn.Should().Be(publishDate);
		article.ModifiedOn.Should().BeAfter(publishDate.AddSeconds(-1));
	}

	[Fact]
	public void Unpublish_ShouldSetIsPublishedFalseAndPublishedOnNull()
	{
		// Arrange
		var article = FakeArticle.GetNewArticle(true);
		article.Publish(DateTime.UtcNow);

		// Act
		article.Unpublish();

		// Assert
		article.IsPublished.Should().BeFalse();
		article.PublishedOn.Should().BeNull();
		article.ModifiedOn.Should().BeAfter(DateTime.UtcNow.AddSeconds(-2));
	}

	[Fact]
	public void Empty_ShouldReturnEmptyArticle()
	{
		// Arrange & Act
		var empty = Article.Empty;

		// Assert
		empty.Title.Should().Be("");
		empty.Introduction.Should().Be("");
		empty.Content.Should().Be("");
		empty.CoverImageUrl.Should().Be("");
		empty.UrlSlug.Should().Be("");
		empty.Author.Should().Be(AppUserDto.Empty);
		empty.Category.Should().Be(CategoryDto.Empty);
		empty.IsPublished.Should().BeFalse();
		empty.PublishedOn.Should().BeNull();
		empty.IsArchived.Should().BeFalse();
	}

}

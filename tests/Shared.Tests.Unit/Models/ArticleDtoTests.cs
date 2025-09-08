// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Models;

/// <summary>
///   Unit tests for the <see cref="ArticleDto" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(ArticleDto))]
public class ArticleDtoTests
{

	[Fact]
	public void DefaultConstructor_ShouldInitializeWithDefaults()
	{
		var dto = new ArticleDto();
		dto.Id.Should().NotBeNull();
		dto.Title.Should().BeEmpty();
		dto.Introduction.Should().BeEmpty();
		dto.Content.Should().BeEmpty();
		dto.CoverImageUrl.Should().BeEmpty();
		dto.UrlSlug.Should().BeEmpty();
		dto.Author.Should().NotBeNull();
		dto.Category.Should().NotBeNull();
		dto.IsPublished.Should().BeFalse();
		dto.IsArchived.Should().BeFalse();
		dto.CanEdit.Should().BeFalse();
	}

	[Fact]
	public void EmptyProperty_ShouldReturnEmptyDto()
	{
		var dto = ArticleDto.Empty;
		dto.Id.Should().NotBeNull();
		dto.Title.Should().BeEmpty();
		dto.Introduction.Should().BeEmpty();
		dto.Content.Should().BeEmpty();
		dto.CoverImageUrl.Should().BeEmpty();
		dto.UrlSlug.Should().BeEmpty();
		dto.Author.Should().NotBeNull();
		dto.Category.Should().NotBeNull();
		dto.IsPublished.Should().BeFalse();
		dto.IsArchived.Should().BeFalse();
		dto.CanEdit.Should().BeFalse();
	}

}

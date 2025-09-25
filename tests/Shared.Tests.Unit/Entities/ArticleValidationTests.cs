// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleValidationTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================
namespace Shared.Tests.Unit.Entities;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Article))]
public class ArticleValidationTests
{

	[Fact]
	public void Constructor_Throws_When_Title_Empty()
	{
		Assert.Throws<ArgumentException>(() =>
				new Article(string.Empty, "intro", "content", "", "slug", "author", Guid.Empty));
	}

	[Fact]
	public void Update_Throws_When_Content_Empty()
	{
		Article article = new ("t", "i", "c", "", "s", "a", Guid.Empty);

		Assert.Throws<ArgumentException>(() =>
				article.Update("t", "i", string.Empty, "", "s", Guid.Empty, false, null, false));
	}

}

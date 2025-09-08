// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     PostInfoComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

// namespace Web.Components.Shared;
//
// /// <summary>
// ///   bUnit tests for PostInfoComponent.
// /// </summary>
// [ExcludeFromCodeCoverage]
// [TestSubject(typeof(PostInfoComponent))]
// public class PostInfoComponentTests : BunitContext
// {
//
// 	[Fact]
// 	public void Should_Render_Author_And_Category()
// 	{
// 		// Arrange
// 		var article = FakeArticle.GetNewArticle(true).Adapt<ArticleDto>();
// 		article.Author.UserName = "TestUser";
// 		article.CreatedOn = new DateTime(2025, 5, 5);
// 		article.PublishedOn = new DateTime(2025, 5, 4);
// 		article.Category.CategoryName = "UnitTest";
//
// 		const string expectedHtml =
// 				"""
// 				<div class="flex gap-4 border-t border-gray-200 pt-4">
// 				  <div>Author: TestUser</div>
// 				  <div>Created: 5/5/2025</div>
// 				  <div>Published: 5/4/2025</div>
// 				  <div>Categories: UnitTest</div>
// 				</div>
// 				""";
//
// 		// Act
// 		var cut = Render<PostInfoComponent>(parameters =>
// 						parameters.Add<PostInfoComponent>(nameof(PostInfoComponent.Article), article));
//
// 		// Assert
// 		cut.MarkupMatches(expectedHtml);
//
// 	}
//
// 	[Fact]
// 	public void Renders_With_Default_Parameters()
// 	{
// 		var cut = Render<PostInfoComponent>();
// 		cut.Markup.Should().NotBeNullOrWhiteSpace();
// 	}
//
// }

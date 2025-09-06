// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     PageHeaderComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

/// <summary>
///   bUnit tests for PageHeaderComponent.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(PageHeaderComponent))]
public class PageHeaderComponentTests : BunitContext
{

	[Theory]
	[InlineData("1", "Test Header", "<h1 class=\"text-2xl font-bold tracking-tight text-gray-50\">Test Header</h1>")]
	[InlineData("2", "Test Header", "<h2 class=\"text-3xl font-bold tracking-tight text-gray-50\">Test Header</h2>")]
	[InlineData("3", "Test Header", "<h3 class=\"text-2xl font-bold tracking-tight text-gray-50\">Test Header</h3>")]
	public void Should_Render_Correct_Header_Level(string level, string headerText, string expectedHtml)
	{
		// Arrange & Act
		var cut = Render<PageHeaderComponent>(parameters => parameters
				.Add(p => p.Level, level)
				.Add(p => p.HeaderText, headerText));

		// Assert
		cut.Markup.Should().Contain(headerText);
		cut.Markup.Should().Contain(expectedHtml);
	}

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     PageHeadingComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

/// <summary>
///   bUnit tests for PageHeadingComponent.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(PageHeadingComponent))]
public class PageHeadingComponentTests : BunitContext
{

	[Theory]
	[InlineData("1", "Test Heading 1", "text-danger", "text-3xl")]
	[InlineData("2", "Test Heading 2", "text-gray-20", "text-2xl")]
	[InlineData("3", "Test Heading 3", "text-gray-30", "text-1xl")]
	public void Should_Render_Correct_Heading_Level(
			string level,
			string headerText,
			string headerColor,
			string expectedHtml)
	{

		// Arrange
		ComponentFactories.AddStub<PageTitle>();

		// Act
		var cut = Render<PageHeadingComponent>(parameters => parameters
				.Add(p => p.Level, level)
				.Add(p => p.HeaderText, headerText)
				.Add(p => p.TextColorClass, headerColor));

		// Assert
		cut.Markup.Should().Contain(headerText);
		cut.Markup.Should().Contain($"h{level}");
		cut.Markup.Should().Contain(headerColor);
		cut.Markup.Should().Contain(expectedHtml);

		// Assert PageTitle
		var pageTitleStub = cut.FindComponent<Stub<PageTitle>>();
		var pageTitle = Render(pageTitleStub.Instance.Parameters.Get(p => p.ChildContent)!);
		pageTitle.Markup.Should().Be(headerText);

	}

}

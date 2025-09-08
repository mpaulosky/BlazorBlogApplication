// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ComponentHeadingComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(ComponentHeadingComponent))]
public class ComponentHeadingComponentTests : BunitContext
{

	[Theory]
	[InlineData("1", "Test Heading 1", "text-gray-10", "text-2xl")]
	[InlineData("2", "Test Heading 2", "text-gray-20", "text-xl")]
	[InlineData("3", "Test Heading 3", "text-gray-30", "text-lg")]
	[InlineData("4", "Test Heading 4", "text-gray-40", "text-md")]
	[InlineData("5", "Test Heading 5", "text-gray-50", "text-sm")]
	public void Should_Render_Correct_Heading_Level(
			string level,
			string headerText,
			string headerColor,
			string expectedHtml)
	{

		// Arrange & Act
		var cut = Render<ComponentHeadingComponent>(parameters => parameters
				.Add(p => p.Level, level)
				.Add(p => p.HeaderText, headerText)
				.Add(p => p.TextColorClass, headerColor));

		// Assert
		cut.Markup.Should().Contain(headerText);
		cut.Markup.Should().Contain($"h{level}");
		cut.Markup.Should().Contain(headerColor);
		cut.Markup.Should().Contain(expectedHtml);

	}

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FooterComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Layout;

/// <summary>
///   bUnit tests for FooterComponent.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FooterComponent))]
public class FooterComponentTests : BunitContext
{

	[Fact]
	public void Should_Render_Footer_Text()
	{

		// Arrange
		int currentYear = DateTime.Now.Year;

		string expectedHtml =
				$"""
				<div class="text-center px-6 py-2 mx-auto xl:max-w-5xl border-t-blue-700">
				  <a href="/">©{currentYear} MPaulosky Co. All rights reserved.</a>
				</div>
				""";

		// Act
		IRenderedComponent<FooterComponent> cut = Render<FooterComponent>();

		// Assert
		cut.MarkupMatches(expectedHtml);

	}

	[Fact]
	public void Renders_Footer()
	{

		IRenderedComponent<FooterComponent> cut = Render<FooterComponent>();
		cut.Markup.Should().Contain("©");
		cut.Markup.Should().Contain("MPaulosky Co. All rights reserved.");
		cut.Markup.Should().Contain("text-center px-6 py-2 mx-auto xl:max-w-5xl border-t-blue-700");

	}

}
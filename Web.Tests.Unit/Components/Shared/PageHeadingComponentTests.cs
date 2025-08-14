// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     PageHeadingComponentTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Solution Name : BlazorBlogApplication.sln
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

public class PageHeadingComponentTests
{
	[Fact]
	public void Renders_H1_With_HeaderText_And_Class_When_Level_1()
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.PageHeadingComponent>(ps => ps
			.Add(p => p.HeaderText, "Hello World")
			.Add(p => p.Level, "1")
			.Add(p => p.TextColorClass, "text-red-500"));

		var h1 = cut.Find("h1");
		h1.TextContent.Should().Be("Hello World");
		h1.GetAttribute("class").Should().Contain("text-red-500").And.Contain("text-3xl");
	}

	[Theory]
	[InlineData("2", "h2", "text-2xl")]
	[InlineData("3", "h3", "text-1xl")]
	public void Renders_Correct_Tag_And_Class_For_Levels_2_And_3(string level, string tag, string expectedSizeClass)
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.PageHeadingComponent>(ps => ps
			.Add(p => p.HeaderText, "Title")
			.Add(p => p.Level, level)
			.Add(p => p.TextColorClass, "text-blue-400"));

		var el = cut.Find(tag);
		el.TextContent.Should().Be("Title");
		el.GetAttribute("class").Should().Contain(expectedSizeClass).And.Contain("text-blue-400");
	}

	[Fact]
	public void Uses_Defaults_When_No_Params_Provided()
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.PageHeadingComponent>();
		var h1 = cut.Find("h1");
		h1.TextContent.Should().Be("My Blog");
		h1.GetAttribute("class").Should().Contain("text-3xl").And.Contain("text-gray-50");
	}
}
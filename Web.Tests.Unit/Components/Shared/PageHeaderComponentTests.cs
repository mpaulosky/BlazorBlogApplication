// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     PageHeaderComponentTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

public class PageHeaderComponentTests
{
	[Theory]
	[InlineData("1", "h1", "text-2xl")]
	[InlineData("2", "h2", "text-3xl")]
	[InlineData("3", "h3", "text-2xl")]
	public void Renders_Correct_Header_By_Level(string level, string tag, string expectedSizeClass)
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.PageHeaderComponent>(ps => ps
			.Add(p => p.HeaderText, "Header Text")
			.Add(p => p.Level, level));

		var header = cut.Find(tag);
		header.TextContent.Should().Be("Header Text");
		header.GetAttribute("class").Should().Contain(expectedSizeClass).And.Contain("text-gray-50");
	}

	[Fact]
	public void Uses_Defaults_When_No_Params_Provided()
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.PageHeaderComponent>();
		var h1 = cut.Find("h1");
		h1.TextContent.Should().Be("My Blog");
		h1.GetAttribute("class").Should().Contain("text-2xl");
	}
}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ComponentHeadingComponentTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

public class ComponentHeadingComponentTests
{
	[Theory]
	[InlineData("1", "h1", "text-2xl")]
	[InlineData("2", "h2", "text-xl")]
	[InlineData("3", "h3", "text-lg")]
	[InlineData("4", "h4", "text-md")]
	[InlineData("5", "h5", "text-sm")]
	public void Renders_Correct_Header_By_Level(string level, string tag, string expectedSizeClass)
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.ComponentHeadingComponent>(ps => ps
			.Add(p => p.HeaderText, "Heading Text")
			.Add(p => p.Level, level)
			.Add(p => p.TextColorClass, "text-cyan-400"));

		var header = cut.Find(tag);
		header.TextContent.Should().Be("Heading Text");
		header.GetAttribute("class").Should().Contain(expectedSizeClass).And.Contain("text-cyan-400");
	}

	[Fact]
	public void Uses_Defaults_When_No_Params_Provided()
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.ComponentHeadingComponent>();

		var h3 = cut.Find("h3");
		h3.TextContent.Should().Be("My Component");
		h3.GetAttribute("class").Should().Contain("text-lg").And.Contain("text-gray-50");
	}
}
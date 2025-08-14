// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ConnectWithUsComponentTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

public class ConnectWithUsComponentTests
{
	[Fact]
	public void Renders_Header_And_Links()
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.ConnectWithUsComponent>();

		// Header from nested ComponentHeadingComponent
		cut.Find("h1").TextContent.Should().Be("Connect With Us");

		var links = cut.FindAll("a");
		links.Should().HaveCount(3);
		links[0].GetAttribute("href").Should().Be("https://www.threads/");
		links[1].GetAttribute("href").Should().Be("https://www.instagram.com/");
		links[2].GetAttribute("href").Should().Be("https://www.youtube.com/");
		links.Should().OnlyContain(a => a.GetAttribute("target") == "_blank");
	}
}
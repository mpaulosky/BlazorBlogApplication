// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     LoadingComponentTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

public class LoadingComponentTests
{
	[Fact]
	public void Renders_Spinner_And_Text()
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.LoadingComponent>();

		cut.Find("svg").GetAttribute("class").Should().Contain("animate-spin");
		cut.Find("h3").TextContent.Should().Be("Loading...");
	}
}
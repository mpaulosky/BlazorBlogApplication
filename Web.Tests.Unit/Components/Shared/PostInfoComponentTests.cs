// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     PostInfoComponentTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

public class PostInfoComponentTests
{
	[Fact]
	public void Renders_Container_Even_With_No_Article_Data()
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.PostInfoComponent>();
		var container = cut.Find("div.flex.gap-4");
		container.Should().NotBeNull();
		// Currently, all content is commented-out; ensure no inner text is present
		cut.Markup.Should().NotContain("Author:").And.NotContain("Created:").And.NotContain("Draft").And.NotContain("Published:");
	}
}
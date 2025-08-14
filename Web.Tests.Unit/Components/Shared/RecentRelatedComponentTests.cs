// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     RecentRelatedComponentTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

public class RecentRelatedComponentTests
{
	[Fact]
	public void Renders_Header_And_Two_Links()
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.RecentRelatedComponent>();

		cut.Find("h1").TextContent.Should().Be("Recent Posts");
		var links = cut.FindAll("a");
		links.Should().HaveCount(2);
		links[0].TextContent.Trim().Should().Be("Run SQL Server on M1 or M2 Macbook");
		links[1].TextContent.Trim().Should().Be("Run a Postgres Database for Free on Google Cloud");
	}
}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ErrorPageComponentTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

public class ErrorPageComponentTests
{
	[Theory]
	[InlineData(401, "401 Unauthorized", "You are not authorized to view this page.")]
	[InlineData(403, "403 Forbidden", "Access to this resource is forbidden.")]
	[InlineData(404, "404 Not Found", "The page you are looking for does not exist.")]
	[InlineData(500, "500 Internal Server Error", "An unexpected error occured on the server.")]
	public void Renders_Title_And_Message_For_Known_Codes(int code, string expectedTitle, string expectedMessageStart)
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.ErrorPageComponent>(ps => ps
			.Add(p => p.ErrorCode, code)
			.Add(p => p.ShadowStyle, "shadow-green-500")
			.Add(p => p.TextColor, "text-green-700"));

		cut.Find("h1").TextContent.Should().Be(expectedTitle);
		cut.Find("p").TextContent.Should().StartWith(expectedMessageStart);
		cut.Find("header").GetAttribute("class").Should().Contain("shadow-green-500");
		cut.Find("h1").GetAttribute("class").Should().Contain("text-green-700");
		cut.Find("p").GetAttribute("class").Should().Contain("text-green-700");
		cut.Find("a").GetAttribute("href").Should().Be("/");
	}

	[Fact]
	public void Renders_Unknown_Error_For_Other_Codes()
	{
		using var ctx = new BunitContext();
		var cut = ctx.Render<Web.Components.Shared.ErrorPageComponent>(ps => ps.Add(p => p.ErrorCode, 418));
		cut.Find("h1").TextContent.Should().Be("Unknown Error");
		cut.Find("p").TextContent.Should().Be("An error occurred. Please try again later.");
	}
}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ErrorPageComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

/// <summary>
///   Unit tests for <see cref="ErrorPageComponent" />.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(ErrorPageComponent))]
public class ErrorPageComponentTests : BunitContext
{

	[Theory]
	[InlineData(401, "401 Unauthorized", "You are not authorized to view this page.", "text-yellow-500",
			"shadow-yellow-500")]
	[InlineData(403, "403 Forbidden", "Access to this resource is forbidden", "text-red-500", "shadow-red-500")]
	[InlineData(404, "404 Not Found", "The page you are looking for does not exist", "text-yellow-500",
			"shadow-yellow-500")]
	[InlineData(500, "500 Internal Server Error", "An unexpected error occured on the server", "text-red-500",
			"shadow-red-500")]
	[InlineData(0, "Unknown Error", "An error occurred. Please try again later.", "text-red-500", "shadow-red-500")]
	public void Should_Render_Correct_Error_Information(
			int errorCode,
			string expectedTitle,
			string expectedMessage,
			string expectedColor,
			string expectedShadowStyle)
	{
		var cut = Render<ErrorPageComponent>(parameters => parameters
				.Add(p => p.ErrorCode, errorCode)
				.Add(p => p.TextColor, expectedColor)
				.Add(p => p.ShadowStyle, expectedShadowStyle)
		);

		cut.Markup.Should().Contain(expectedTitle);
		cut.Markup.Should().Contain(expectedMessage);
		cut.Markup.Should().Contain(expectedColor);
		cut.Markup.Should().Contain(expectedShadowStyle);
	}

}

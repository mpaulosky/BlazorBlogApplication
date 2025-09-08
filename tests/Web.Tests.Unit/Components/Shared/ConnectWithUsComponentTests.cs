// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ConnectWithUsComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

/// <summary>
///   bUnit tests for ConnectWithUsComponent.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(ConnectWithUsComponent))]
public class ConnectWithUsComponentTests : BunitContext
{

	[Fact]
	public void Should_Render_ConnectWithUs_Title()
	{

		// Arrange

		// Act
		var cut = Render<ConnectWithUsComponent>();

		// Assert
		cut.Markup.Should()
				.Contain(
						"<header class=\"mb-6 py-4\"><h1 class=\"text-2xl font-semibold tracking-tight py-4 text-gray-50\">Connect With Us</h1></header>");

	}

	[Fact]
	public void Should_Render_ConnectWithUs_Threads()
	{

		// Arrange

		// Act
		var cut = Render<ConnectWithUsComponent>();

		// Assert
		cut.Markup.Should()
				.Contain(
						"<a href=\"https://www.threads/\" target=\"_blank\"><i class=\"ri-threads-line text-3xl hover:bg-gray-100 hover:rounded p-2\"></i></a>");

	}

	[Fact]
	public void Should_Render_ConnectWithUs_Instagram()
	{

		// Arrange

		// Act
		var cut = Render<ConnectWithUsComponent>();

		// Assert
		cut.Markup.Should()
				.Contain(
						"<a href=\"https://www.instagram.com/\" target=\"_blank\"><i class=\"ri-instagram-line text-3xl\"></i></a>");

	}

	[Fact]
	public void Should_Render_ConnectWithUs_YouTube()
	{

		// Arrange

		// Act
		var cut = Render<ConnectWithUsComponent>();

		// Assert
		cut.Markup.Should()
				.Contain(
						"<a href=\"https://www.youtube.com/\" target=\"_blank\"><i class=\"ri-youtube-line text-3xl\"></i></a>");

	}

}

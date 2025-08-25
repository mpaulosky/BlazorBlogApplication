// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     HomePageTest.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Pages;

/// <summary>
///   bUnit tests for the Home page.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Home))]
public class HomePageTest : BunitContext
{

	[Fact]
	public void HomePage_Should_Render_Youre_Service()
	{

		// Arrange
		const string expectedHtml =
				"""
				<header class="mx-auto
				  	max-w-7xl
				  	mb-6
				  	p-1
				  	sm:px-4
				  	md:px-6
				  	lg:px-8
				  	rounded-md
				  	shadow-md
				  	shadow-blue-500">
				<h1 class="text-3xl font-bold tracking-tight text-gray-50">Home - Article Service</h1>
				</header>
				<h1>Welcome, You're Article Service!</h1>
				You can only see this content if you're authenticated.
				<br>
				<img src="" alt="User UserProfile Picture" />
				""";

		// Act
		var cut = Render<Home>();

		// Assert
		cut.MarkupMatches(expectedHtml);

	}

	[Fact]
	public void HomePage_Should_Render_Authenticated_User()
	{

		// Arrange
		Helpers.SetAuthorization(this, true, "User");

		const string expectedHtml =
				"""
				<header class="mx-auto
					max-w-7xl
					mb-6
					p-1
					sm:px-4
					md:px-6
					lg:px-8
					rounded-md
					shadow-md
					shadow-blue-500">
				<h1 class="text-3xl font-bold tracking-tight text-gray-50">Home - Article Service</h1>
				</header>
				<h1>Welcome, test@example.com</h1>
				You can only see this content if you're authenticated.
				<br>
				<img src="https://example.com/picture.jpg" alt="User UserProfile Picture" />
				""";

		// Act
		var cut = Render<Home>();

		// Assert
		cut.MarkupMatches(expectedHtml);

	}

}

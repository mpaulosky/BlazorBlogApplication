// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AboutPageTest.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Pages;

/// <summary>
///   bUnit tests for the About page.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(About))]
public class AboutPageTest : BunitContext
{

	[Fact]
	public void AboutPage_Should_Render_Heading()
	{

		// Arrange
		const string expectedHtml =
				"""
				<header class="mx-auto max-w-7xl mb-6
								p-1 sm:px-4 md:px-6 lg:px-8
								rounded-md shadow-md
								shadow-blue-500">
				  <h1 class="text-3xl font-bold tracking-tight text-gray-50">About</h1>
				</header>
				<div class="prose max-w-2xl mx-auto text-gray-100 bg-gray-800 rounded-md shadow-md p-6 mt-6">
				  <h2 class="text-xl font-bold mb-2">About - BlazorBlogApplication</h2>
				  <p>
				    <strong>BlazorBlogApplication</strong>
				    is a modern, open-source blogging platform built with
				    <b>.NET Aspire</b>
				    and
				    <b>Blazor       Server</b>. It enables users to create, manage, and share blog posts with ease, leveraging a MongoDB backend for     document storage.
				  </p>
				  <ul class="list-disc pl-6 mt-2">
				    <li>Built with .NET 9, Aspire 9.2, C#, and Blazor Server</li>
				    <li>Styled using TailwindCSS for a clean, responsive UI</li>
				    <li>Supports authentication and authorization (Auth0)</li>
				    <li>Unit and integration tests included (xUnit, bUnit, Playwright)</li>
				    <li>Integration tests use Docker for isolated MongoDB instances</li>
				    <li>Implements Clean Architecture, CQRS, and SOLID principles</li>
				    <li>OpenAPI/Swagger documentation for APIs</li>
				  </ul>
				  <p class="mt-2">
				    <b>Source code:</b>
				    <a href="https://github.com/mpaulosky/BlazorBlogApplication" target="_blank" class="text-blue-400 underline">github.com/mpaulosky/BlazorBlogApplication</a>
				  </p>
				</div>
				""";

		// Act
		var cut = Render<About>();

		// Assert
		cut.MarkupMatches(expectedHtml);

	}

}

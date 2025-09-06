// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ContactPageTest.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Pages;

/// <summary>
///   bUnit tests for the Contact page.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Contact))]
public class ContactPageTest : BunitContext
{

	[Fact]
	public void ContactPage_Should_Render_Heading()
	{

		// Arrange
		const string expectedHtml =
				"""
				<header class="mx-auto max-w-7xl mb-6
								p-1 sm:px-4 md:px-6 lg:px-8
								rounded-md shadow-md
								shadow-blue-500">
				  <h1 class="text-3xl font-bold tracking-tight text-gray-50">Contacts</h1>
				</header>
				<div class="prose max-w-2xl mx-auto text-gray-100 bg-gray-800 rounded-md shadow-md p-6 mt-6">
				  <h2 class="text-xl font-bold mb-2">Contact - BlazorBlogApplication</h2>
				  <p>
				    For questions, support, or feedback about
				    <b>BlazorBlogApplication</b>, please use one of the following methods:
				  </p>
				  <ul class="list-disc pl-6 mt-2">
				    <li>Email:
				      <a href="mailto:mpaulosky@gmail.com" class="text-blue-400 underline">mpaulosky@gmail.com</a>
				    </li>
				    <li>GitHub Issues:
				      <a href="https://github.com/mpaulosky/BlazorBlogApplication/issues" target="_blank" class="text-blue-400 underline">github.com/mpaulosky/BlazorBlogApplication/issues</a>
				    </li>
				  </ul>
				  <p class="mt-4">
				    For more details, see the
				    <a href="/docs/CONTRIBUTING.md" class="text-blue-400 underline">contributing guide</a>.
				  </p>
				</div>
				""";

		// Act
		var cut = Render<Contact>();

		// Assert
		cut.MarkupMatches(expectedHtml);

	}

}

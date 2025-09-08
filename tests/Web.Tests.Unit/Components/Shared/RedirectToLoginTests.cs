// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     RedirectToLoginTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

/// <summary>
///   Unit tests for the RedirectToLogin component.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(RedirectToLogin))]
public class RedirectToLoginTests : BunitContext
{

	[Fact]
	public void RedirectToLoginComponent_RendersCorrectly()
	{

		// Arrange
		var navMan = Services.GetRequiredService<BunitNavigationManager>();

		// Act
		Render<RedirectToLogin>();

		// Assert
		navMan.Uri.Should().EndWith("/Account/LoginComponent");

	}

}

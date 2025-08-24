// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MainLayoutTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Layout;

/// <summary>
///   bUnit tests for MainLayout.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(MainLayout))]
public class MainLayoutTests : BunitContext
{

	public MainLayoutTests()
	{

		Services.AddScoped<CascadingAuthenticationState>();

	}

	[Fact]
	public void Should_Render_NavMenu_And_Footer()
	{

		// Arrange
		SetAuthorization(false);

		// Act
		var cut = Render<MainLayout>();

		// Assert
		cut.Markup.Should().Contain("Article Service");
		cut.Markup.Should().Contain("All rights reserved");
		cut.Markup.Should().Contain("©");
		cut.Markup.Should().Contain("MPaulosky Co. All rights reserved.");
		cut.Markup.Should().Contain("Articles");
		cut.Markup.Should().Contain("Categories");
		cut.Markup.Should().Contain("Contact");
		cut.Markup.Should().Contain("About");
		cut.Markup.Should().Contain("Log in");

	}

	[Fact]
	public void Should_Render_NavMenu_With_Authenticated_User()
	{

		// Arrange
		SetAuthorization(true, true);

		// Act
		var cut = Render<MainLayout>();

		// Assert
		cut.Markup.Should().Contain("Hey Test User!");
		// Profile link should be present for authenticated users
		cut.Markup.Should().Contain("Profile");
		// AccessControlComponent will render Log out when authenticated
		cut.Markup.Should().Contain("Log out");

	}

	private void SetAuthorization(bool isAuthorized = true, bool hasRoles = false)
	{

		var authContext = AddAuthorization();

		// Set up the authentication state for the component
		if (isAuthorized)
		{

			// If authorized, set the context to authorize with a test user
			authContext.SetAuthorized("Test User");

		}

		// Optionally set roles if required
		if (hasRoles)
		{

			authContext.SetClaims(new Claim(ClaimTypes.Role, "Admin"), new Claim(ClaimTypes.Role, "User"));

		}

	}

	[Theory]
	[InlineData("Unauthorized")]
	[InlineData("Argument")]
	[InlineData("NotFound")]
	[InlineData("Generic")]
	public void Should_Display_ErrorPage_When_Child_Throws(string throwType)
	{
		// Arrange
		SetAuthorization(false);

		// Act
		RenderFragment rf = builder =>
		{
			builder.OpenComponent<ThrowingChild>(0);
			builder.AddAttribute(1, nameof(ThrowingChild.ThrowType), throwType);
			builder.CloseComponent();
		};

		var cut = Render<MainLayout>(parameters => parameters
			.Add(p => p.Body, rf)
		);

		// Assert - the ErrorPageComponent renders a title depending on the code
		string expectedTitle = throwType switch
		{
			"Unauthorized" => "401 Unauthorized",
			"Argument" => "Unknown Error",
			"NotFound" => "404 Not Found",
			_ => "500 Internal Server Error",
		};

		cut.Markup.Should().Contain(expectedTitle);
	}

	[Fact]
	public void Should_Render_Child_Content_When_No_Exception()
	{
		// Arrange
		SetAuthorization(false);

		// Act
		RenderFragment rf = builder =>
		{
			builder.OpenComponent<ThrowingChild>(0);
			builder.AddAttribute(1, nameof(ThrowingChild.ThrowType), "");
			builder.CloseComponent();
		};

		var cut = Render<MainLayout>(parameters => parameters
			.Add(p => p.Body, rf)
		);

		// Assert
		cut.Markup.Should().Contain("Child content rendered normally");
	}

}

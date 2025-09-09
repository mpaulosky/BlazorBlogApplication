// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ErrorPageTest.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Pages;

/// <summary>
///   bUnit tests for the Error page.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Error))]
public class ErrorPageTest : BunitContext
{

	[Fact]
	public void ErrorPage_Should_Render_ErrorTitle()
	{

		// Arrange
		ComponentFactories.AddStub<PageTitle>();

		// Act
		var cut = Render<Error>();

		// Assert
		cut.Markup.Should().Contain("Error");
		cut.Markup.Should().Contain("An error occurred while processing your request.");

		// Assert PageTitle
		var pageTitleStub = cut.FindComponent<Stub<PageTitle>>();
		var pageTitle = Render(pageTitleStub.Instance.Parameters.Get(p => p.ChildContent)!);
		pageTitle.Markup.Should().Be("Error");

	}

	[Fact]
	public void ErrorPage_Should_Render_RequestId_When_Present()
	{

		// Arrange & Act
		var cut = Render<Error>();

		// Simulate a request ID being set
		cut.Instance.GetType().GetField("_requestId",
						BindingFlags.NonPublic | BindingFlags.Instance)!
				.SetValue(cut.Instance, "REQ-12345");

		cut.Render();

		// Assert
		cut.Markup.Should().Contain("Request ID:");
		cut.Markup.Should().Contain("REQ-12345");

	}

	[Fact]
	public void ErrorPage_Uses_HttpContext_If_Provided()
	{

		// Arrange
		var httpContext = new DefaultHttpContext
		{
				TraceIdentifier = "Test-Trace-Id"
		};

		// Act
		var cut = Render<Error>(parameters => parameters
				.AddCascadingValue(httpContext)
		);

		// Assert
		cut.Markup.Should().Contain("Test-Trace-Id");

	}


	[Fact]
	public void ErrorPage_Should_Not_Render_RequestId_When_Absent()
	{

		// Arrange & Act
		var cut = Render<Error>();

		// Simulate no request ID
		cut.Instance.GetType().GetField("_requestId",
						BindingFlags.NonPublic | BindingFlags.Instance)!
				.SetValue(cut.Instance, null);

		cut.Render();

		// Assert
		cut.Markup.Should().NotContain("Request ID:");

	}

	[Fact]
	public void ErrorPage_Should_Render_DevelopmentMode_Warning()
	{

		// Arrange & Act
		var cut = Render<Error>();

		// Assert
		cut.Markup.Should().Contain("Development Mode");
		cut.Markup.Should().Contain("The Development environment shouldn't be enabled for deployed applications.");
		cut.Markup.Should().Contain("ASPNETCORE_ENVIRONMENT");

	}

}

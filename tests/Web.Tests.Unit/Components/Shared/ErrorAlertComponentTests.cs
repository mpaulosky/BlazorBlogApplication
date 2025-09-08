// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ErrorAlertComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(ErrorAlertComponent))]
public class ErrorAlertComponentTests : BunitContext
{

	[Fact]
	public void RendersTitleAndChildContent_WhenChildProvided()
	{
		// Arrange
		var title = "Test Title";
		var child = (RenderFragment)(builder => builder.AddContent(0, "Child message here."));

		// Act
		var cut = Render<ErrorAlertComponent>(parameters => parameters
				.Add(p => p.Title, title)
				.Add<RenderFragment>(p => p.ChildContent!, child)
		);

		// Assert
		cut.Markup.Should().Contain(title);
		cut.Markup.Should().Contain("Child message here.");
	}

	[Fact]
	public void UsesMessageFallback_WhenNoChildContentProvided()
	{
		// Arrange
		var title = "Fallback Title";
		var message = "Fallback message.";

		// Act
		var cut = Render<ErrorAlertComponent>(parameters => parameters
				.Add(p => p.Title, title)
				.Add(p => p.Message, message)
		);

		// Assert
		cut.Markup.Should().Contain(title);
		cut.Markup.Should().Contain(message);
	}

	[Fact]
	public void HasCorrectVisualStructure()
	{
		// Arrange
		var title = "Visual Title";
		var message = "Visual message.";

		// Act
		var cut = Render<ErrorAlertComponent>(parameters => parameters
				.Add(p => p.Title, title)
				.Add(p => p.Message, message)
		);

		// Assert - ensure the tailwind container and header are present
		cut.Markup.Should().Contain("bg-red-50");
		cut.Markup.Should().Contain("text-red-800");
		cut.Markup.Should().Contain(title);
	}

}

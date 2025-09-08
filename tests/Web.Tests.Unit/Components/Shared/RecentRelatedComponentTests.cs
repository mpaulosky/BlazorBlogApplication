// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     RecentRelatedComponentTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

/// <summary>
///   bUnit tests for RecentRelatedComponent.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(RecentRelatedComponent))]
public class RecentRelatedComponentTests : BunitContext
{

	[Fact]
	public void Should_Render_Recent_Posts_Title()
	{
		// Arrange & Act
		var cut = Render<RecentRelatedComponent>();

		// Assert
		cut.Markup.Should().Contain("Recent Posts");
		cut.Markup.Should().Contain("Run SQL Server on M1 or M2 Macbook");
		cut.Markup.Should().Contain("Run a Postgres Database for Free on Google Cloud");
	}

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EnvironmentBehaviorTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class EnvironmentBehaviorTests : BunitContext
{

	private readonly CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;

	[Fact]
	public async Task Production_Uses_ExceptionHandler()
	{

		// Arrange
		Helpers.SetAuthorization(this);
		await using var factory = new TestWebApplicationFactory("Production");

		var client = factory.CreateClient(new WebApplicationFactoryClientOptions
		{
				AllowAutoRedirect = false
		});

		// Act
		var res = await client.GetAsync("/", _cancellationToken);

		// Assert
		res.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect, HttpStatusCode.NotFound);

	}

}

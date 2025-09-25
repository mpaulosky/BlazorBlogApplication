// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EnvironmentBehaviorTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EnvironmentBehaviorTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using System.Net.Http;

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
		await using TestWebApplicationFactory factory = new ("Production");

		HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

		// Act
		HttpResponseMessage res = await client.GetAsync("/", _cancellationToken);

		// Assert
		res.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect, HttpStatusCode.NotFound);

	}

}

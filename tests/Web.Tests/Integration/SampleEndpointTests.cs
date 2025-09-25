// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     SampleEndpointTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================

namespace Web.Tests.Integration;

[Collection("IntegrationTests")]
public class SampleEndpointTests
{

	private readonly TestPostgresFixture _dbFixture = new ();

	[Fact]
	public async Task GetRoot_ReturnsSuccess()
	{
		// Arrange
		TestAppFactory factory = new ();
		HttpClient client = factory.CreateClient();

		// Act
		HttpResponseMessage res = await client.GetAsync("/", TestContext.Current.CancellationToken);

		// Assert
		res.EnsureSuccessStatusCode();
	}

}
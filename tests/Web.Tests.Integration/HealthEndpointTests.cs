// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     HealthEndpointTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================

using System.Net;

using Web.Extensions;

namespace Web;

[Collection("Test Collection")]
public class HealthEndpointTests
{

	private readonly WebTestFactory _factory;

	public HealthEndpointTests(WebTestFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task Get_Root_ReturnsSuccess()
	{
		// Arrange
		using HttpClient client = _factory.CreateClientWithJson();

		// Act
		HttpResponseMessage res = await client.GetAsync("/", TestContext.Current.CancellationToken);

		// Assert
		res.StatusCode.Should().Be(HttpStatusCode.OK);
	}

}
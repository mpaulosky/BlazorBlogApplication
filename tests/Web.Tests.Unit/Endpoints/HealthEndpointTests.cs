// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     HealthEndpointTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

#region

using System.Net.Http;

using TestContext = Xunit.TestContext;

#endregion

namespace Web.Endpoints;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class HealthEndpointTests : IClassFixture<TestWebApplicationFactory>
{

	private readonly TestWebApplicationFactory _factory;

	public HealthEndpointTests(TestWebApplicationFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task Health_Returns_Healthy_Text()
	{
		HttpClient client = _factory.CreateClient();
		HttpResponseMessage res = await client.GetAsync("/health", TestContext.Current.CancellationToken);
		res.StatusCode.Should().Be(HttpStatusCode.OK);

		string body = await res.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
		body.Should().Be("Healthy");
	}

}
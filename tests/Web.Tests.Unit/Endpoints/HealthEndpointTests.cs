// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     HealthEndpointTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

#region

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
		var client = _factory.CreateClient();
		var res = await client.GetAsync("/health", TestContext.Current.CancellationToken);
		res.StatusCode.Should().Be(HttpStatusCode.OK);

		var body = await res.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
		body.Should().Be("Healthy");
	}

}

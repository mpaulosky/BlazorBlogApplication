// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ProgramSmokeTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class ProgramSmokeTests
{

	private readonly CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;

	[Fact]
	public async Task App_Starts_And_Health_Endpoint_Works()
	{
		await using var factory = new TestWebApplicationFactory();

		var client = factory.CreateClient(new WebApplicationFactoryClientOptions
		{
				AllowAutoRedirect = true
		});

		var res = await client.GetAsync("/health", _cancellationToken);
		res.IsSuccessStatusCode.Should().BeTrue();

		var content = await res.Content.ReadAsStringAsync(_cancellationToken);
		content.Should().Contain("Healthy");
	}

	[Fact]
	public async Task Root_Does_Not_Throw_On_Request()
	{
		await using var factory = new TestWebApplicationFactory();
		var client = factory.CreateClient();

		var res = await client.GetAsync("/", _cancellationToken);
		res.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.Redirect);
	}

}

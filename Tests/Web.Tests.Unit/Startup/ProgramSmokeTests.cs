// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ProgramSmokeTests.cs
// Company :       mpaulosky
// Author :        Coverage PR1
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using Microsoft.AspNetCore.Mvc.Testing;
using Web.Infrastructure;

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class ProgramSmokeTests
{
	[Fact]
	public async Task App_Starts_And_Health_Endpoint_Works()
	{
		await using var factory = new TestWebApplicationFactory();
		var client = factory.CreateClient(new WebApplicationFactoryClientOptions
		{
			AllowAutoRedirect = true
		});

		var res = await client.GetAsync("/health", Xunit.TestContext.Current.CancellationToken);
		res.IsSuccessStatusCode.Should().BeTrue();

		var content = await res.Content.ReadAsStringAsync();
		content.Should().Contain("Healthy");
	}

	[Fact]
	public async Task Root_Does_Not_Throw_On_Request()
	{
		await using var factory = new TestWebApplicationFactory();
		var client = factory.CreateClient();

		var res = await client.GetAsync("/", Xunit.TestContext.Current.CancellationToken);
		res.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound, System.Net.HttpStatusCode.Redirect);
	}
}
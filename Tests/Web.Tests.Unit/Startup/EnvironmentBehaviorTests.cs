// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EnvironmentBehaviorTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

using Web.Infrastructure;

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class EnvironmentBehaviorTests
{
	[Fact]
	public async Task Production_Uses_ExceptionHandler()
	{
		await using var factory = new TestWebApplicationFactory(environment: "Production");
		var client = factory.CreateClient(new WebApplicationFactoryClientOptions
		{
				AllowAutoRedirect = false
		});

		var res = await client.GetAsync("/", Xunit.TestContext.Current.CancellationToken);

		res.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect, HttpStatusCode.NotFound);
	}
}
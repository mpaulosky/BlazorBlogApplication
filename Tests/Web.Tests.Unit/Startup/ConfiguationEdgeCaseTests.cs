// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ConfigurationEdgeCaseTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Infrastructure;

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class ConfigurationEdgeCaseTests
{
	[Fact]
	public void Missing_Mongo_ConnectionString_Should_Cause_Startup_Failure()
	{
		var cfg = new Dictionary<string, string?>
		{
				["Auth0-Domain"] = "test.example.com",
				["Auth0-Client-Id"] = "client-id"
				// omit "mongoDb-connection"
		};

		using var factory = new TestWebApplicationFactory(config: cfg);

		Func<HttpClient> act = () => factory.CreateClient();

		act.Should().Throw<Exception>();
	}

	[Fact]
	public void Missing_Auth0_Config_Should_Cause_Startup_Failure()
	{
		var cfg = new Dictionary<string, string?>
		{
				["mongoDb-connection"] = "mongodb://localhost:27017"
		};

		using var factory = new TestWebApplicationFactory(config: cfg);

		Func<HttpClient> act = () => factory.CreateClient();

		act.Should().Throw<Exception>();
	}
}
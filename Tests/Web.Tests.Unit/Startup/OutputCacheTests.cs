// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     OutputCacheTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class OutputCacheTests : IClassFixture<TestWebApplicationFactory>
{

	private readonly TestWebApplicationFactory _factory;

	public OutputCacheTests(TestWebApplicationFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public void OutputCache_Services_Are_Registered()
	{
		using var scope = _factory.Services.CreateScope();
		var sp = scope.ServiceProvider;

		sp.GetService<IOutputCacheStore>().Should().NotBeNull();
		sp.GetService<IOptions<OutputCacheOptions>>().Should().NotBeNull();
	}

}

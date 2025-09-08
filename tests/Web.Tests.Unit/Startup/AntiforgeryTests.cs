// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AntiforgeryTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class AntiforgeryTests : IClassFixture<TestWebApplicationFactory>
{

	private readonly TestWebApplicationFactory _factory;

	public AntiforgeryTests(TestWebApplicationFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public void Antiforgery_HeaderName_Is_Configured()
	{
		using var scope = _factory.Services.CreateScope();
		var options = scope.ServiceProvider.GetRequiredService<IOptions<AntiforgeryOptions>>().Value;
		options.HeaderName.Should().Be("X-XSRF-TOKEN");
	}

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CorsPolicyTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class CorsPolicyTests : IClassFixture<TestWebApplicationFactory>
{

	private readonly TestWebApplicationFactory _factory;

	public CorsPolicyTests(TestWebApplicationFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task DefaultCorsPolicy_Exists_And_Has_Expected_Origins()
	{
		using var scope = _factory.Services.CreateScope();
		var provider = scope.ServiceProvider.GetRequiredService<ICorsPolicyProvider>();

		var httpContext = new DefaultHttpContext();
		var policy = await provider.GetPolicyAsync(httpContext, DEFAULT_CORS_POLICY);

		policy.Should().NotBeNull();

		policy.Origins.Should().Contain([
				"https://yourdomain.com",
				"https://localhost:7157"
		]);

		// AllowAnyHeader/Method usually means collections are empty ("allow any")
		policy.Headers.Should().BeEmpty();
		policy.Methods.Should().BeEmpty();
	}

}

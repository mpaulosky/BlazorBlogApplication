// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AuthorizationPolicyTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Startup;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class AuthorizationPolicyTests : IClassFixture<TestWebApplicationFactory>
{

	private readonly TestWebApplicationFactory _factory;

	public AuthorizationPolicyTests(TestWebApplicationFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task AdminOnly_Policy_Requires_Admin_Role()
	{
		using var scope = _factory.Services.CreateScope();
		var provider = scope.ServiceProvider.GetRequiredService<IAuthorizationPolicyProvider>();

		var policy = await provider.GetPolicyAsync(ADMIN_POLICY);
		policy.Should().NotBeNull();

		policy.Requirements.OfType<RolesAuthorizationRequirement>()
				.SelectMany(r => r.AllowedRoles)
				.Should().Contain("admin");
	}

}

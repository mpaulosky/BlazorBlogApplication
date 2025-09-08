// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AuthEndpointsTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Endpoints;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Program))]
public class AuthEndpointsTests : IClassFixture<TestWebApplicationFactory>
{

	private readonly TestWebApplicationFactory _factory;
	private readonly CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;


	public AuthEndpointsTests(TestWebApplicationFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task Login_Endpoint_Issues_Challenge_On_Auth0_Scheme()
	{
		// Arrange
		var authService = _factory.Services.GetRequiredService<IAuthenticationService>();
		authService.Should().NotBeNull();

		var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
		{
				AllowAutoRedirect = false
		});

		// Act
		var res = await client.GetAsync("/account/login", _cancellationToken);

		// Assert: Status should be redirect-like due to challenge
		res.StatusCode.Should().BeOneOf(
				HttpStatusCode.Redirect,
				HttpStatusCode.RedirectKeepVerb,
				HttpStatusCode.RedirectMethod,
				HttpStatusCode.SeeOther,
				HttpStatusCode.Found,
				HttpStatusCode.TemporaryRedirect);

		await authService.Received().ChallengeAsync(
				Arg.Any<HttpContext>(),
				Arg.Is(Auth0Constants.AuthenticationScheme),
				Arg.Any<AuthenticationProperties>());
	}

	[Fact]
	public async Task Logout_Endpoint_SignsOut_Auth0_And_Cookies()
	{
		// Arrange
		var authService = _factory.Services.GetRequiredService<IAuthenticationService>();
		authService.Should().NotBeNull();

		var client = _factory.CreateClient();

		// Act
		var res = await client.GetAsync("/account/logout", _cancellationToken);

		// Assert
		res.StatusCode.Should().Be(HttpStatusCode.OK);

		await authService.Received().SignOutAsync(
				Arg.Any<HttpContext>(),
				Arg.Is(Auth0Constants.AuthenticationScheme),
				Arg.Any<AuthenticationProperties>());

		await authService.Received().SignOutAsync(
				Arg.Any<HttpContext>(),
				Arg.Is(CookieAuthenticationDefaults.AuthenticationScheme),
				Arg.Any<AuthenticationProperties>());
	}

}

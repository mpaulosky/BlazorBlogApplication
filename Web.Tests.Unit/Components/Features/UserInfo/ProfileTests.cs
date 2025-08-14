// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ProfileTests.cs
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

using NSubstitute;

using Web.Data.Auth0;

namespace Web.Components.Features.UserInfo;

public class ProfileTests : BunitContext
{
	private readonly IAuth0Service _auth0Service = Substitute.For<IAuth0Service>();

	public ProfileTests()
	{

		Services.AddScoped<IAuth0Service>(_ => _auth0Service);

	}

	[Fact]
	public void Shows_Loading_When_No_AuthState()
	{
		var cut = Render<Profile>();
		cut.Markup.Should().Contain("Loading user information...");
	}
}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AccessControlComponentTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Web.Components.Shared;

public class AccessControlComponentTests
{
	[Fact]
	public void Shows_LogOut_When_Authorized()
	{
		using var ctx = new BunitContext();
		ctx.Services.AddAuthorizationCore();
		ctx.Services.AddScoped<AuthenticationStateProvider>(_ => new TestAuthStateProvider(true));
		ctx.Services.AddScoped<IAuthorizationService, FakeAuthorizationService>();

		var cut = ctx.Render<Microsoft.AspNetCore.Components.Authorization.CascadingAuthenticationState>(ps => ps
			.AddChildContent<Web.Components.Shared.AccessControlComponent>());

		// Should show Log out link
		cut.FindAll("a").Should().ContainSingle(a => a.TextContent.Contains("Log out"));
		// Should not show Log in link
		cut.Markup.Should().NotContain("Log in");
	}

	[Fact]
	public void Shows_LogIn_When_NotAuthorized()
	{
		using var ctx = new BunitContext();
		ctx.Services.AddAuthorizationCore();
		ctx.Services.AddScoped<AuthenticationStateProvider>(_ => new TestAuthStateProvider(false));
		ctx.Services.AddScoped<IAuthorizationService, FakeAuthorizationService>();

		var cut = ctx.Render<Microsoft.AspNetCore.Components.Authorization.CascadingAuthenticationState>(ps => ps
			.AddChildContent<Web.Components.Shared.AccessControlComponent>());

		cut.FindAll("a").Should().ContainSingle(a => a.TextContent.Contains("Log in"));
		cut.Markup.Should().NotContain("Log out");
	}

	private sealed class TestAuthStateProvider(bool isAuthenticated) : AuthenticationStateProvider
	{
		private readonly bool _isAuthenticated = isAuthenticated;

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var identity = _isAuthenticated
				? new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim("name", "TestUser") }, "TestAuth")
				: new System.Security.Claims.ClaimsIdentity();
			var user = new System.Security.Claims.ClaimsPrincipal(identity);
			return Task.FromResult(new AuthenticationState(user));
		}
	}

	private sealed class FakeAuthorizationService : IAuthorizationService
	{
		public Task<AuthorizationResult> AuthorizeAsync(System.Security.Claims.ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
		{
			var isAuthed = user.Identity?.IsAuthenticated == true;
			return Task.FromResult(isAuthed ? AuthorizationResult.Success() : AuthorizationResult.Failed());
		}

		public Task<AuthorizationResult> AuthorizeAsync(System.Security.Claims.ClaimsPrincipal user, object? resource, string policyName)
		{
			var isAuthed = user.Identity?.IsAuthenticated == true;
			return Task.FromResult(isAuthed ? AuthorizationResult.Success() : AuthorizationResult.Failed());
		}
	}
}
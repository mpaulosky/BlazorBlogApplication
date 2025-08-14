// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     RedirectToLoginTests.cs
// Company :       mpaulosky
// Author :        Junie (JetBrains AI)
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Shared;

public class RedirectToLoginTests
{
	[Fact]
	public void Navigates_To_Login_On_Initialized()
	{
		using var ctx = new BunitContext();
		var fakeNav = new FakeNavigationManager();
		ctx.Services.AddScoped<NavigationManager>(_ => fakeNav);

		ctx.Render<Web.Components.Shared.RedirectToLogin>();

		fakeNav.Uri.Should().EndWith("Account/LoginComponent");
		fakeNav.ForceLoadCalled.Should().BeTrue();
	}

	private sealed class FakeNavigationManager : NavigationManager
	{
		public bool ForceLoadCalled { get; private set; }

		public FakeNavigationManager()
		{
			Initialize("http://localhost/", "http://localhost/");
		}

		protected override void NavigateToCore(string uri, bool forceLoad)
		{
			ForceLoadCalled = forceLoad;
			Uri = ToAbsoluteUri(uri).ToString();
		}
	}
}
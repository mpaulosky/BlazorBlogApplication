// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     PlaywrightManager.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================
using System.Diagnostics;

using Microsoft.Playwright;

namespace Web.Infrastructure;

/// <summary>
///   Configure Playwright for interacting with the browser in tests.
/// </summary>
public class PlaywrightManager : IAsyncLifetime
{

	private static bool IsDebugging => Debugger.IsAttached;

	private static bool IsHeadless => IsDebugging is false;

	private IPlaywright? _playwright;

	internal IBrowser Browser { get; set; } = null!;

	public async ValueTask InitializeAsync()
	{
		_playwright = await Microsoft.Playwright.Playwright.CreateAsync();

		BrowserTypeLaunchOptions options = new() { Headless = IsHeadless };

		Browser = await _playwright.Chromium.LaunchAsync(options).ConfigureAwait(false);
	}

	public async ValueTask DisposeAsync()
	{
		await Browser.CloseAsync();

		_playwright?.Dispose();
	}

}

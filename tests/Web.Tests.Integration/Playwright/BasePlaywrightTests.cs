// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     BasePlaywrightTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     BasePlaywrightTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================

using Aspire.Hosting;

using Microsoft.Playwright;

using Web.Infrastructure;
using Web.Tests.Infrastructure;

namespace Web.Playwright;

/// <summary>
///   Base class for Playwright tests, providing common functionality and setup for Playwright testing with ASP.NET Core.
/// </summary>
public abstract class BasePlaywrightTests : IClassFixture<AspireManager>, IAsyncDisposable
{

	protected BasePlaywrightTests(AspireManager aspireManager)
	{
		AspireManager = aspireManager ?? throw new ArgumentNullException(nameof(aspireManager));
	}

	private AspireManager AspireManager { get; }

	private PlaywrightManager PlaywrightManager => AspireManager.PlaywrightManager;

	private string? DashboardUrl { get;  set; }

	public string DashboardLoginToken { get; private set; } = "";

	private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

	private IBrowserContext? _context;

	public Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
			string[]? args = null,
			Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class
	{
		return AspireManager.ConfigureAsync<TEntryPoint>(args, builder =>
		{
			string? aspNetCoreUrls = builder.Configuration["ASPNETCORE_URLS"];
			string[] urls = aspNetCoreUrls is not null ? aspNetCoreUrls.Split(";") : [];

			DashboardUrl = urls.FirstOrDefault();
			DashboardLoginToken = builder.Configuration["AppHost:BrowserToken"] ?? "";

			configureBuilder?.Invoke(builder);
		});
	}

	public async Task InteractWithPageAsync(
			string serviceName,
			Func<IPage, Task> test,
			ViewportSize? size = null)
	{

		Uri urlSought;
		CancellationToken cancellationToken = new CancellationTokenSource(DefaultTimeout).Token;

		// Empty string means the dashboard URL
		if (!string.IsNullOrEmpty(serviceName))
		{
			if (AspireManager.App!.GetEndpoint(serviceName) is null)
			{
				throw new InvalidOperationException($"Service '{serviceName}' not found in the application endpoints");
			}

			//			urlSought = new Uri (AppHostTestFixture.App.GetEndpoint(serviceName), relativeUrl);
			urlSought = AspireManager.App!.GetEndpoint(serviceName);
		}
		else
		{
			urlSought = new Uri(DashboardUrl!);
		}

		await AspireManager.App!.ResourceNotifications.WaitForResourceHealthyAsync(serviceName, cancellationToken)
				.WaitAsync(DefaultTimeout, cancellationToken);

		IPage page = await CreateNewPageAsync(urlSought, size);

		try
		{
			await test(page);
		}
		finally
		{
			await page.CloseAsync();
		}
	}

	private async Task<IPage> CreateNewPageAsync(Uri uri, ViewportSize? size = null)
	{
		_context = await PlaywrightManager.Browser.NewContextAsync(new BrowserNewContextOptions
		{
				IgnoreHTTPSErrors = true, ColorScheme = ColorScheme.Dark, ViewportSize = size, BaseURL = uri.ToString()
		});

		return await _context.NewPageAsync();

	}


	public async ValueTask DisposeAsync()
	{
		GC.SuppressFinalize(this);

		if (_context is not null)
		{
			await _context.DisposeAsync();
		}
	}

}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AspireManager.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AspireManager.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================

using Aspire.Hosting;

using Web.Infrastructure;

namespace Web.Tests.Infrastructure;

/// <summary>
///   Start up and configure the Aspire application for testing.
/// </summary>
public class AspireManager : IAsyncLifetime
{

	internal PlaywrightManager PlaywrightManager { get; } = new();

	internal DistributedApplication? App { get; private set; }

	public async Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
			string[]? args = null,
			Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class
	{

		if (App is not null)
		{
			return App;
		}

		IDistributedApplicationTestingBuilder builder = await DistributedApplicationTestingBuilder.CreateAsync<TEntryPoint>(
				args ?? [],
				static (options, _) =>
				{
					options.DisableDashboard = false;
				});

		builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

		configureBuilder?.Invoke(builder);

		App = await builder.BuildAsync();

		await App.StartAsync();

		return App;
	}


	public async ValueTask InitializeAsync()
	{
		// Initialization logic here
		await PlaywrightManager.InitializeAsync();
	}

	public async ValueTask DisposeAsync()
	{
		await PlaywrightManager.DisposeAsync();

		await (App?.DisposeAsync() ?? ValueTask.CompletedTask);
	}

}

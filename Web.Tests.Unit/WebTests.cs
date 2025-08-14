// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     WebTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication.sln
// Project Name :  Tests
// =======================================================

using System.Net;

using Aspire.Hosting.Testing;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using static Shared.Services;

namespace Web;

/// <summary>
/// Contains integration tests for the web frontend.
/// </summary>
public class WebTests
{
	/// <summary>
	/// The default timeout for test operations.
	/// </summary>
	private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);

	/// <summary>
	/// Verifies that the root web resource returns an OK status code.
	/// </summary>
	[Fact]
	public async Task GetWebResourceRootReturnsOkStatusCodeAsync()
	{
		// Arrange
		var cancellationToken = TestContext.Current.CancellationToken;
		var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(cancellationToken);
		appHost.Services.AddLogging(logging =>
		{
			logging.SetMinimumLevel(LogLevel.Debug);
			// Override the logging filters from the app's configuration
			logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
			logging.AddFilter("Aspire.", LogLevel.Debug);
			// To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
		});
		appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
		{
			clientBuilder.AddStandardResilienceHandler();
		});
		await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);
		await app.StartAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

		// Act
		var httpClient = app.CreateHttpClient(WEBSITE);
		await app.ResourceNotifications.WaitForResourceHealthyAsync(WEBSITE, cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);
		var response = await httpClient.GetAsync("/", cancellationToken);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}
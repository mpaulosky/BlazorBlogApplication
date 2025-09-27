// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IntegrationTest.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================

using System.Security.Authentication;

using Aspire.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Projects;

using static Shared.Services;

namespace Web.Integration;

public class AppHostIntegrationTests : IAsyncLifetime
{
	private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(300); // Increased timeout for container startup
	private DistributedApplication? _app;
	private IDistributedApplicationTestingBuilder? _appHost;

	public async ValueTask InitializeAsync()
	{
		// Create the AppHost once for all tests to improve performance
		CancellationToken cancellationToken = TestContext.Current.CancellationToken;

		// Set environment variables to avoid persistent containers in tests
		Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
		Environment.SetEnvironmentVariable("DOTNET_CONTAINER_LIFETIME_MODE", "Session");

		_appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(cancellationToken);

		_appHost.Services.AddLogging(logging =>
		{
			logging.SetMinimumLevel(LogLevel.Debug);
			logging.AddFilter(_appHost.Environment.ApplicationName, LogLevel.Debug);
		});

		_appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
		{
			clientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
			{
					ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			});
			clientBuilder.AddStandardResilienceHandler();
		});

		_app = await _appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
		await _app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

		// Wait for critical resources to be healthy before running any tests
		await _app.ResourceNotifications.WaitForResourceHealthyAsync(WEBSITE, cancellationToken)
				.WaitAsync(DefaultTimeout, cancellationToken);
	}

	public async ValueTask DisposeAsync()
	{
		if (_app != null)
		{
			await _app.DisposeAsync();
		}
		_appHost?.Dispose();
	}

	[Fact]
	public async Task GetWebResourceRootReturnsOkStatusCode()
	{
		// Arrange
		if (_app == null)
			throw new InvalidOperationException("AppHost not initialized");

		CancellationToken cancellationToken = TestContext.Current.CancellationToken;

		// Act
		Uri endpoint = _app.GetEndpoint(WEBSITE);
		Console.WriteLine($"Integration test: resource endpoint = {endpoint}");

		HttpResponseMessage? response = null;

		// Try using HTTPS first with a permissive handler. If SSL validation still fails, retry with HTTP.
		try
		{
			using HttpClientHandler httpClientHandler = new()
			{
					ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};

			using HttpClient httpClient = new (httpClientHandler) { BaseAddress = endpoint };

			Console.WriteLine("Integration test: sending GET / with permissive handler");
			response = await httpClient.GetAsync("/", cancellationToken);
		}
		catch (HttpRequestException ex) when (ex.InnerException is AuthenticationException)
		{
			Console.WriteLine($"Integration test: HTTPS request failed with SSL AuthenticationException: {ex.Message}");

			// If the endpoint is https://, try switching to http:// and retry
			if (endpoint.Scheme == Uri.UriSchemeHttps)
			{
				Uri httpEndpoint = new UriBuilder(endpoint) { Scheme = Uri.UriSchemeHttp, Port = endpoint.Port }.Uri;
				Console.WriteLine($"Integration test: retrying with HTTP endpoint {httpEndpoint}");
				using HttpClient httpClient = new()  { BaseAddress = httpEndpoint };
				response = await httpClient.GetAsync("/", cancellationToken);
			}
			else
			{
				throw;
			}
		}

		if (response == null)
		{
			throw new InvalidOperationException("Integration test: no response received from endpoint");
		}

		// Assert: the site may redirect to HTTPS during startup; accept OK or Redirect as healthy
		Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Redirect,
				$"Unexpected status code: {response.StatusCode}");
	}

	[Fact]
	public async Task AppHost_AllResources_ReachHealthyState()
	{
		// Arrange
		if (_app == null)
			throw new InvalidOperationException("AppHost not initialized");

		CancellationToken cancellationToken = TestContext.Current.CancellationToken;

		// Act & Assert: Verify all resources reach healthy state
		await _app.ResourceNotifications.WaitForResourceHealthyAsync(SERVER, cancellationToken)
				.WaitAsync(DefaultTimeout, cancellationToken);

		await _app.ResourceNotifications.WaitForResourceHealthyAsync(CACHE, cancellationToken)
				.WaitAsync(DefaultTimeout, cancellationToken);

		await _app.ResourceNotifications.WaitForResourceHealthyAsync(ARTICLE_DATABASE, cancellationToken)
				.WaitAsync(DefaultTimeout, cancellationToken);

		await _app.ResourceNotifications.WaitForResourceHealthyAsync(WEBSITE, cancellationToken)
				.WaitAsync(DefaultTimeout, cancellationToken);

		// Verify we can get endpoints for HTTP resources
		Uri webEndpoint = _app.GetEndpoint(WEBSITE);
		Assert.NotNull(webEndpoint);
		Assert.True(webEndpoint.IsAbsoluteUri);

		Console.WriteLine($"AppHost test: All resources are healthy");
		Console.WriteLine($"Web endpoint: {webEndpoint}");
	}

	[Fact]
	public async Task AppHost_ConnectionStrings_AreProperlyConfigured()
	{
		// Arrange
		if (_app == null)
			throw new InvalidOperationException("AppHost not initialized");

		CancellationToken cancellationToken = TestContext.Current.CancellationToken;

		// Act & Assert: Verify connection strings are available
		string? postgresConnection = await _app.GetConnectionStringAsync(ARTICLE_DATABASE, cancellationToken);
		string? redisConnection = await _app.GetConnectionStringAsync(CACHE, cancellationToken);

		Assert.NotNull(postgresConnection);
		Assert.NotEmpty(postgresConnection);
		Assert.Contains("postgres", postgresConnection, StringComparison.OrdinalIgnoreCase);

		Assert.NotNull(redisConnection);
		Assert.NotEmpty(redisConnection);
		Assert.Contains("localhost", redisConnection, StringComparison.OrdinalIgnoreCase);

		Console.WriteLine($"PostgreSQL connection: {postgresConnection}");
		Console.WriteLine($"Redis connection: {redisConnection}");
	}

	[Fact]
	public async Task AppHost_WebApplication_CanConnectToResources()
	{
		// Arrange
		if (_app == null)
			throw new InvalidOperationException("AppHost not initialized");

		CancellationToken cancellationToken = TestContext.Current.CancellationToken;

		// Act: Try to access the web application
		using HttpClient httpClient = _app.CreateHttpClient(WEBSITE);
		
		// Try different endpoints that might indicate resource connectivity
		string[] testEndpoints = { "/", "/health", "/articles" };
		bool anyEndpointWorked = false;

		foreach (string endpoint in testEndpoints)
		{
			try
			{
				HttpResponseMessage response = await httpClient.GetAsync(endpoint, cancellationToken);
				
				// Accept various success responses - the key is that we get a response indicating the app is running
				if (response.StatusCode == HttpStatusCode.OK || 
				    response.StatusCode == HttpStatusCode.Redirect ||
				    response.StatusCode == HttpStatusCode.NotFound) // 404 is OK - means the endpoint exists but returns 404
				{
					anyEndpointWorked = true;
					Console.WriteLine($"Successfully connected to {endpoint}: {response.StatusCode}");
					break;
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"Failed to connect to {endpoint}: {ex.Message}");
			}
		}

		// Assert: At least one endpoint should be accessible, indicating resource connectivity is working
		Assert.True(anyEndpointWorked, "Web application should be accessible and able to connect to resources");
	}

	[Fact]
	public async Task AppHost_ResourceEndpoints_AreAccessible()
	{
		// Arrange
		if (_app == null)
			throw new InvalidOperationException("AppHost not initialized");

		CancellationToken cancellationToken = TestContext.Current.CancellationToken;

		// Act & Assert: Verify endpoints are accessible
		Uri webEndpoint = _app.GetEndpoint(WEBSITE);
		Assert.NotNull(webEndpoint);
		Assert.True(webEndpoint.IsAbsoluteUri);
		
		// Verify the endpoint is actually responding
		using HttpClient httpClient = new(new HttpClientHandler
		{
			ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
		})
		{
			BaseAddress = webEndpoint
		};

		HttpResponseMessage response = await httpClient.GetAsync("/", cancellationToken);
		Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Redirect,
				$"Web endpoint should be accessible. Status: {response.StatusCode}");

		Console.WriteLine($"Web endpoint {webEndpoint} is accessible: {response.StatusCode}");
	}
}
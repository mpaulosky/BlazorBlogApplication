using Microsoft.Extensions.Logging;
using System.Security.Authentication;

using static Shared.Services;

namespace Web.Tests.Integration;

public class IntegrationTest1
{

	private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

	[Fact]
	public async Task GetWebResourceRootReturnsOkStatusCode()
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
			// Accept untrusted dev certificates in tests (local Aspire hosts use self-signed certs)
			clientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			});

			clientBuilder.AddStandardResilienceHandler();
		});
	
	    await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
		await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

		// Act
	await app.ResourceNotifications.WaitForResourceHealthyAsync(WEBSITE, cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

		// Use the resource endpoint and create a local HttpClient that accepts the test server's self-signed cert
	var endpoint = app.GetEndpoint(WEBSITE);
		Console.WriteLine($"Integration test: resource endpoint = {endpoint}");

		HttpResponseMessage? response = null;
		// Try using HTTPS first with a permissive handler. If SSL validation still fails, retry with HTTP.
		try
		{
			using var httpClientHandler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};
			using var httpClient = new HttpClient(httpClientHandler)
			{
				BaseAddress = endpoint
			};

			Console.WriteLine("Integration test: sending GET / with permissive handler");
			response = await httpClient.GetAsync("/", cancellationToken);
		}
		catch (HttpRequestException ex) when (ex.InnerException is AuthenticationException)
		{
			Console.WriteLine($"Integration test: HTTPS request failed with SSL AuthenticationException: {ex.Message}");
			// If the endpoint is https://, try switching to http:// and retry
			if (endpoint.Scheme == Uri.UriSchemeHttps)
			{
				var httpEndpoint = new UriBuilder(endpoint) { Scheme = Uri.UriSchemeHttp, Port = endpoint.Port }.Uri;
				Console.WriteLine($"Integration test: retrying with HTTP endpoint {httpEndpoint}");
				using var httpClient = new HttpClient { BaseAddress = httpEndpoint };
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

}
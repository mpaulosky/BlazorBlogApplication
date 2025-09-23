// using System.Net.Http.Json;

// using Microsoft.VisualBasic;

// namespace MyWeatherHub.Tests;

// public class IntegrationTests
// {
// 	private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
// 	[Fact]
// 	public async Task TestApiGetZones()
// 	{
// 		// Arrange
// 		var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(_cancellationToken);

// 		appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
// 		{
// 			clientBuilder.AddStandardResilienceHandler();
// 		});

// 		await using var app = await appHost.BuildAsync(_cancellationToken);

// 		var resourceNotificationService = app.Services
// 				.GetRequiredService<ResourceNotificationService>();

// 		await app.StartAsync();

// 		// Act
// 		var httpClient = app.CreateHttpClient("api");

// 		await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running, _cancellationToken).WaitAsync(TimeSpan.FromSeconds(30), _cancellationToken);

// 		var response = await httpClient.GetAsync("/zones", _cancellationToken);

// 		// Assert
// 		response.EnsureSuccessStatusCode();
// 		var zones = await response.Content.ReadFromJsonAsync<Zone[]>(_cancellationToken);
// 		Assert.NotNull(zones);
// 		Assert.True(zones.Length > 0);
// 	}

// 	[Fact]
// 	public async Task TestWebAppHomePage()
// 	{
// 		// Arrange
// 		var appHost = await DistributedApplicationTestingBuilder
// 				.CreateAsync<Projects.AppHost>(TestContext.Current.CancellationToken);

// 		appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
// 		{
// 			clientBuilder.AddStandardResilienceHandler();
// 		});

// 		await using var app = await appHost.BuildAsync(_cancellationToken);

// 		var resourceNotificationService = app.Services
// 				.GetRequiredService<ResourceNotificationService>();

// 		await app.StartAsync(_cancellationToken);

// 		// Act
// 		var httpClient = app.CreateHttpClient("myweatherhub");

// 		await resourceNotificationService.WaitForResourceAsync("myweatherhub", KnownResourceStates.Running, _cancellationToken).WaitAsync(TimeSpan.FromSeconds(30), _cancellationToken);

// 		var response = await httpClient.GetAsync("/", _cancellationToken);

// 		// Assert
// 		response.EnsureSuccessStatusCode();
// 		var content = await response.Content.ReadAsStringAsync(_cancellationToken);
// 		Assert.Contains("MyWeatherHub", content);
// 	}
// }

// public record Zone(string Key, string Name, string State);

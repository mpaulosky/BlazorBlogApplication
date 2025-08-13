// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     WeatherApiClient.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication.sln
// Project Name :  Web
// =======================================================

namespace Web;

/// <summary>
/// Provides methods to interact with the weather API.
/// </summary>
public class WeatherApiClient
{
	private readonly HttpClient _httpClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="WeatherApiClient"/> class.
	/// </summary>
	/// <param name="httpClient">The HTTP client used for API requests.</param>
	public WeatherApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	/// <summary>
	/// Retrieves weather forecasts asynchronously.
	/// </summary>
	/// <param name="maxItems">Maximum number of items to return.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>An array of <see cref="WeatherForecast"/>.</returns>
	public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
	{
		List<WeatherForecast>? forecasts = null;

		await foreach (var forecast in _httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast", cancellationToken))
		{
			if (forecasts?.Count >= maxItems)
			{
				break;
			}
			if (forecast is not null)
			{
				forecasts ??= [];
				forecasts.Add(forecast);
			}
		}

		return forecasts?.ToArray() ?? [];
	}
}

/// <summary>
/// Represents a weather forecast.
/// </summary>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	/// <summary>
	/// Gets the temperature in Fahrenheit.
	/// </summary>
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
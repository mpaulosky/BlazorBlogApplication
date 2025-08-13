// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Program.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication.sln
// Project Name :  ApiService
// =======================================================

namespace ApiService;

/// <summary>
/// Entry point for the API service. Configures services and endpoints for weather forecasting.
/// </summary>
public static class ApiServiceEntryPoint
{
	/// <summary>
	/// Main method to configure and run the API service.
	/// </summary>
	/// <param name="args">Command-line arguments.</param>
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add service defaults & Aspire client integrations.
		builder.AddServiceDefaults();

		// Add services to the container.
		builder.Services.AddProblemDetails();
		builder.Services.AddOpenApi();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		app.UseExceptionHandler();

		if (app.Environment.IsDevelopment())
		{
			app.MapOpenApi();
		}

		string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

		app.MapGet("/weatherforecast", () =>
				{
					var forecast = Enumerable.Range(1, 5).Select(index =>
									new WeatherForecast
									(
											DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
											Random.Shared.Next(-20, 55),
											summaries[Random.Shared.Next(summaries.Length)]
									))
							.ToArray();
					return forecast;
				})
				.WithName("GetWeatherForecast");

		app.MapDefaultEndpoints();

		app.Run();
	}

	/// <summary>
	/// Represents a weather forecast.
	/// </summary>
	private record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
	{
		/// <summary>
		/// Gets the temperature in Fahrenheit.
		/// </summary>
		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
	}
}
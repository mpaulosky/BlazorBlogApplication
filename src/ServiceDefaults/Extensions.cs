// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Extensions.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  ServiceDefaults
// =======================================================
using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ServiceDefaults;

/// <summary>
///   Provides extension methods for adding common .NET Aspire services, health checks, and OpenTelemetry.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Extensions
{

	/// <summary>
	///   The health endpoint path.
	/// </summary>
	private const string HealthEndpointPath = "/health";

	/// <summary>
	///   The aliveness endpoint path.
	/// </summary>
	private const string AlivenessEndpointPath = "/alive";

	/// <summary>
	///   Adds service defaults including OpenTelemetry, health checks, and service discovery.
	/// </summary>
	/// <typeparam name="TBuilder">The application builder type.</typeparam>
	/// <param name="builder">The application builder.</param>
	/// <returns>The application builder.</returns>
	public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	{
		builder.ConfigureOpenTelemetry();
		builder.AddDefaultHealthChecks();
		builder.Services.AddServiceDiscovery();

		builder.Services.ConfigureHttpClientDefaults(http =>
		{
			// Turn on resilience by default
			http.AddStandardResilienceHandler();

			// Turn on service discovery by default
			http.AddServiceDiscovery();
		});

		return builder;
	}

	/// <summary>
	///   Configures OpenTelemetry logging, metrics, and tracing.
	/// </summary>
	/// <typeparam name="TBuilder">The application builder type.</typeparam>
	/// <param name="builder">The application builder.</param>
	/// <returns>The application builder.</returns>
	public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
			where TBuilder : IHostApplicationBuilder
	{
		builder.Logging.AddOpenTelemetry(logging =>
		{
			logging.IncludeFormattedMessage = true;
			logging.IncludeScopes = true;
		});

		builder.Services.AddOpenTelemetry()
				.WithMetrics(metrics =>
				{
					metrics.AddAspNetCoreInstrumentation()
							.AddHttpClientInstrumentation()
							.AddRuntimeInstrumentation();
				})
				.WithTracing(tracing =>
				{
					tracing.AddSource(builder.Environment.ApplicationName)
							.AddAspNetCoreInstrumentation(tracing =>

									// Exclude health check requests from tracing
									tracing.Filter = context =>
											!context.Request.Path.StartsWithSegments(HealthEndpointPath)
											&& !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
							)

							// Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
							//.AddGrpcClientInstrumentation()
							.AddHttpClientInstrumentation();
				});

		builder.AddOpenTelemetryExporters();

		return builder;
	}

	/// <summary>
	///   Adds OpenTelemetry exporters based on configuration.
	/// </summary>
	/// <typeparam name="TBuilder">The application builder type.</typeparam>
	/// <param name="builder">The application builder.</param>
	/// <returns>The application builder.</returns>
	private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder)
			where TBuilder : IHostApplicationBuilder
	{
		bool useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

		if (useOtlpExporter)
		{
			builder.Services.AddOpenTelemetry().UseOtlpExporter();
		}

		// Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
		//if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
		//{
		//    builder.Services.AddOpenTelemetry()
		//       .UseAzureMonitor();
		//}
		return builder;
	}

	/// <summary>
	///   Adds default health checks to the application.
	/// </summary>
	/// <typeparam name="TBuilder">The application builder type.</typeparam>
	/// <param name="builder">The application builder.</param>
	/// <returns>The application builder.</returns>
	public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
			where TBuilder : IHostApplicationBuilder
	{
		builder.Services.AddHealthChecks()

				// Add a default liveness check to ensure the app is responsive
				.AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

		return builder;
	}

	/// <summary>
	///   Maps default health check endpoints for the application.
	/// </summary>
	/// <param name="app">The web application.</param>
	/// <returns>The web application.</returns>
	public static WebApplication MapDefaultEndpoints(this WebApplication app)
	{
		// Adding health checks endpoints to applications in non-development environments has security implications.
		// See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
		if (app.Environment.IsDevelopment())
		{
			// All health checks must pass for the app to be considered ready to accept traffic after starting
			app.MapHealthChecks(HealthEndpointPath);

			// Only health checks tagged with the "live" tag must pass for the app to be considered alive
			app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
		}

		return app;
	}

}

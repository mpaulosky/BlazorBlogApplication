// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ConfigurationValidationHostedService.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web;

[ExcludeFromCodeCoverage]
public sealed class ConfigurationValidationHostedService : IHostedService
{

	private readonly IConfiguration _config;

	private readonly IHostApplicationLifetime _lifetime;

	public ConfigurationValidationHostedService(IConfiguration config, IHostApplicationLifetime lifetime)
	{
		_config = config;
		_lifetime = lifetime;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		// Defer validation until the application has fully started. This allows
		// test host builders (TestWebApplicationFactory) to apply their
		// ConfigureAppConfiguration changes before we validate required keys.
		_lifetime.ApplicationStarted.Register(() =>
		{
			// Prefer IConfiguration but fall back to environment variables. In
			// some test-host scenarios the IConfiguration providers configured
			// by the test factory may not be visible at the exact moment this
			// callback runs, so checking env vars ensures values exported by
			// TestWebApplicationFactory are honored.
			string? mongoConn = _config["mongoDb-connection"];
			string? mongoEnv = Environment.GetEnvironmentVariable("mongoDb-connection");

			// Diagnostic output to help narrow down why test-provided
			// configuration isn't visible to the hosted service during tests.
			Console.WriteLine(
					$"[ConfigValidation] IConfiguration['mongoDb-connection']='{mongoConn ?? "<null>"}', ENV['mongoDb-connection']='{mongoEnv ?? "<null>"}'");

			if (string.IsNullOrWhiteSpace(mongoConn))
			{
				mongoConn = mongoEnv;
			}

			if (string.IsNullOrWhiteSpace(mongoConn))
			{
				throw new Exception("Required configuration 'mongoDb-connection' is missing");
			}

			// Auth0 removed: do not validate Auth0 configuration here. If authentication
			// settings are required by a deployment, validate them in deployment/CI.
		});

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

}
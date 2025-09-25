// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     TestWebApplicationFactory.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Infrastructure;

[ExcludeFromCodeCoverage]
public class TestWebApplicationFactory : WebApplicationFactory<IAppMarker>
{

	private readonly Dictionary<string, string?> _config;

	private readonly string _environment;

	private readonly Dictionary<string, string?> _previousEnv = new();

	public TestWebApplicationFactory()
			: this("Development") { }

	internal TestWebApplicationFactory(
			string environment = "Development",
			Dictionary<string, string?>? config = null)
	{
		_environment = environment;

		_config = config ?? new Dictionary<string, string?>
		{
				// Provide a placeholder DefaultConnection so production startup code that
				// expects a connection string can initialize during unit tests without
				// requiring external configuration.
				["DefaultConnection"] = "Host=localhost;Database=Test;Username=test;Password=test"
		};

		// Export the test configuration to environment variables so that
		// host-level configuration (which may be evaluated before the
		// web-host's in-memory collection is applied) can see these values.
		foreach (KeyValuePair<string, string?> kvp in _config)
		{
			if (kvp.Value is null)
			{
				continue;
			}

			// Preserve previous value so we can restore on disposing
			_previousEnv[kvp.Key] = Environment.GetEnvironmentVariable(kvp.Key);
			Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
		}

		// Ensure missing known keys are cleared so edge-case tests behave deterministically
		string[] knownKeys = new[] { "mongoDb-connection", "DefaultConnection" };

		foreach (string key in knownKeys)
		{
			if (!_config.ContainsKey(key) || string.IsNullOrWhiteSpace(_config[key]))
			{
				if (!_previousEnv.ContainsKey(key))
				{
					_previousEnv[key] = Environment.GetEnvironmentVariable(key);
				}

				Environment.SetEnvironmentVariable(key, null);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		// Restore environment variables to previous values
		foreach (KeyValuePair<string, string?> kvp in _previousEnv)
		{
			Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
		}

		base.Dispose(disposing);
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment(_environment);

		builder.ConfigureAppConfiguration((_, cfg) =>
		{
			cfg.AddInMemoryCollection(_config);
		});

		builder.ConfigureTestServices(services =>
		{

			// MongoDB driver has been removed; tests should use EF ApplicationDbContext.
			// Do not substitute or register any IMongoClient test double here.

			// No Auth0 test double registration - authentication behavior is covered by the test host and AddAuthorization
		});
	}

}
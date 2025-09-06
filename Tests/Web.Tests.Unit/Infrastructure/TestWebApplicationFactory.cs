// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     TestWebApplicationFactory.cs
// Company :       mpaulosky
// Author :        Matthew
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
				["auth0-domain"] = "test.example.com",
				["auth0-client-id"] = "client-id",
				["mongoDb-connection"] = "mongodb://localhost:27017"
		};

		// Export the test configuration to environment variables so that
		// host-level configuration (which may be evaluated before the
		// web-host's in-memory collection is applied) can see these values.
		foreach (var kvp in _config)
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
		var knownKeys = new[] { "auth0-domain", "auth0-client-id", "mongoDb-connection" };

		foreach (var key in knownKeys)
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
		foreach (var kvp in _previousEnv)
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

			// Only replace IMongoClient with a test double when a connection string is present.
			// If the test intends to exercise missing configuration edge-cases, it will
			// provide a config without the key, and we should not hide that failure by
			// substituting the client.
			if (!string.IsNullOrWhiteSpace(_config.GetValueOrDefault("mongoDb-connection")))
			{
				var existingMongo = services.FirstOrDefault(d => d.ServiceType == typeof(IMongoClient));

				if (existingMongo is not null)
				{
					services.Remove(existingMongo);
				}

				var mongo = Substitute.For<IMongoClient>();
				services.AddSingleton(mongo);
			}

			// Only replace IAuthenticationService when Auth0 config is present. This
			// ensures tests that purposely omit Auth0 settings will trigger startup
			// validation errors instead of being masked by a test double.
			if (!string.IsNullOrWhiteSpace(_config.GetValueOrDefault("auth0-domain"))
					&& !string.IsNullOrWhiteSpace(_config.GetValueOrDefault("auth0-client-id")))
			{
				var existingAuth = services.FirstOrDefault(d => d.ServiceType == typeof(IAuthenticationService));

				if (existingAuth is not null)
				{
					services.Remove(existingAuth);
				}

				var auth = Substitute.For<IAuthenticationService>();

				// When the app issues a ChallengeAsync, we want the test double
				// to behave like a real authentication service and set a
				// redirect-like status code so endpoint tests can assert on it.
				auth.When(x => x.ChallengeAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<AuthenticationProperties>()))
						.Do(ci =>
						{
							var ctx = ci.ArgAt<HttpContext>(0);
							var props = ci.ArgAt<AuthenticationProperties>(2);

							// Use 302 Found as a canonical redirect response and
							// set a Location header so HttpClient's redirect/cookie
							// handlers have a valid URI to work with.
							ctx.Response.StatusCode = (int)HttpStatusCode.Found;
							var location = props?.RedirectUri ?? "/";
							ctx.Response.Headers["Location"] = location;
						});

				// When signing out, set an OK status so logout endpoint tests
				// observe a successful sign-out response.
				auth.When(x => x.SignOutAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<AuthenticationProperties>()))
						.Do(ci =>
						{
							var ctx = ci.ArgAt<HttpContext>(0);
							ctx.Response.StatusCode = (int)HttpStatusCode.OK;
						});

				services.AddSingleton(auth);
			}
		});
	}

}

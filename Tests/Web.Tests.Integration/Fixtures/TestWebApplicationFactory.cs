using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Web.Data;

namespace Web.Tests.Integration.Fixtures;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
	private readonly string _mongoConnectionString;

	public TestWebApplicationFactory(string mongoConnectionString)
	{
		_mongoConnectionString = mongoConnectionString;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration((context, conf) =>
		{
			// Inject the mongo connection string into configuration so the app uses the test container
			var dict = new Dictionary<string, string?>
			{
				["mongoDb-connection"] = _mongoConnectionString,
				// Provide safe Auth0 values for tests; some tests may still rely on validation so provide placeholders
				["Auth0-Domain"] = "test.local",
				["Auth0-Client-Id"] = "test-client"
			};

			// Allow tests to disable antiforgery middleware
			dict["Disable-AntiForgery"] = "true";

			conf.AddInMemoryCollection(dict);
		});

		// Optionally override DI - leave MongoClient registration to normal DI since Program.cs reads configuration when creating it.
		builder.ConfigureServices(services =>
		{
			// No-op for now; most tests will use the configured IMongoClient from Program which will pick up the in-memory config.
		});
	}
}

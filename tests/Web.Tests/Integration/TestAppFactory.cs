using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Web.Tests.Integration
{
    // Minimal factory - real tests can extend to replace services / configure DB
    public class TestAppFactory : WebApplicationFactory<IAppMarker>
    {
        private readonly string? _connectionString;

        public TestAppFactory(string? connectionString = null)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Integration");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.Integration.json", optional: true, reloadOnChange: false);

                if (!string.IsNullOrEmpty(_connectionString))
                {
                    var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["ConnectionStrings:DefaultConnection"] = _connectionString
                    };

                    config.AddInMemoryCollection(dict);
                }
            });
        }
    }
}
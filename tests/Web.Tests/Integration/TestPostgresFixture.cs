namespace Web.Tests.Integration
{
    // Minimal stub fixture so the test project compiles while Testcontainers issues are investigated.
    // This can be replaced with a Testcontainers-based fixture once types resolve correctly.
    public class TestPostgresFixture : IAsyncLifetime
    {
        // Allow overriding via environment variable for local runs where Docker is managed externally.
        public string ConnectionString { get; private set; } = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION") ?? string.Empty;

        public async ValueTask InitializeAsync()
        {
            // No-op; a real container startup will be implemented in a follow-up.
            await ValueTask.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await ValueTask.CompletedTask;
        }
    }
}
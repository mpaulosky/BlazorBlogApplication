using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

namespace Web.Fixtures;

[Collection("Test Collection")]
[ExcludeFromCodeCoverage]
[UsedImplicitly]
public class WebTestFactory : WebApplicationFactory<IAppMarker>, IAsyncLifetime
{
	private readonly ILogger<WebTestFactory> _logger;
	private readonly string _databaseName;
	private readonly CancellationTokenSource _cts;
	private static PostgreSqlContainer? s_sharedContainer;
	private static readonly Lock Lock = new();
	private static int s_port;
	private static readonly SemaphoreSlim DbLock = new(1, 1);

	public WebTestFactory()
	{
		_databaseName = $"test_db_{Guid.NewGuid():N}";
		_cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

		var loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.AddConsole();
			builder.SetMinimumLevel(LogLevel.Debug);
		});

		_logger = loggerFactory.CreateLogger<WebTestFactory>();

		// Initialize a shared container if not already done
		if (s_sharedContainer == null)
		{
			lock (Lock)
			{
				if (s_sharedContainer == null)
				{
					s_port = new Random().Next(5237, 6000);
					s_sharedContainer = new PostgreSqlBuilder()
						.WithImage("postgres:16-alpine")
						.WithDatabase(_databaseName)
						.WithUsername("postgres")
						.WithPassword("postgrespw")
						.WithPortBinding(s_port, 5432)
						.Build();
				}
			}
		}

		// Prefer unsecured transport for Aspire/test hosts and export the DefaultConnection
		// environment variable early so it is available during host startup when
		// Program.ConfigureServices runs. Some test hosts construct the application
		// before ConfigureWebHost runs, so ConfigureAppConfiguration isn't enough.
		// Allow Aspire to expose unsecured HTTP endpoints for integration tests.
		Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
		
		// Ensure ASP.NET binds to an HTTP URL (use dynamic port 0 letting the host choose)
		Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://localhost:0");

		// Export the DefaultConnection environment variable early so it is available
		// during host startup when Program.ConfigureServices runs. Some test hosts
		// construct the application before ConfigureWebHost runs, so ConfigureAppConfiguration
		// isn't enough.
		var earlyConnection = $"Host=localhost;Port={s_port};Database={_databaseName};Username=postgres;Password=postgrespw;";
		
		Environment.SetEnvironmentVariable("DefaultConnection", earlyConnection);
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration((context, config) =>
		{
			var npgsqlConnection = $"Host=localhost;Port={s_port};Database={_databaseName};Username=postgres;Password=postgrespw;";
			// Add to the in-memory configuration so the app picks it up.
			config.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["ConnectionStrings:DefaultConnection"] = npgsqlConnection,
				["DefaultConnection"] = npgsqlConnection,
				// Prefer unsecured transport in the app's configuration for tests
				["Aspire:AllowUnsecuredTransport"] = "true",
				["ASPNETCORE_URLS"] = "http://localhost:0"
			});

			// Also, export to environment variables so code paths that read
			// Environment.GetEnvironmentVariable("DefaultConnection") can find it
			// during host startup in the test environment.
			Environment.SetEnvironmentVariable("DefaultConnection", npgsqlConnection);
		});

		// No extra service registrations required here; the app registers DbContext via configuration
	}

 public async ValueTask InitializeAsync()
	{
		try
		{
			_logger.LogInformation("Starting PostgresSQL container...");
			await s_sharedContainer!.StartAsync(_cts.Token);
			_logger.LogInformation("PostgresSQL container started successfully on localhost:{Port}", s_port);

			// Apply EF Core migrations to ensure the schema is ready
			using var scope = Services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			await db.Database.MigrateAsync(_cts.Token);
			_logger.LogInformation("Database migrated successfully");
		}
		catch (OperationCanceledException)
		{
			_logger.LogError("PostgresSQL container startup timed out after 5 minutes");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to start PostgresSQL container or migrate database");
			throw;
		}
	}

	public override async ValueTask DisposeAsync()
	{
		try
		{
			// Only dispose of the container when the last test factory is disposed
			if (s_sharedContainer != null)
			{
				_logger.LogInformation("Disposing PostgresSQL container...");
				await s_sharedContainer.DisposeAsync();
				s_sharedContainer = null;
				_logger.LogInformation("PostgresSQL container disposed successfully");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error disposing PostgresSQL container");
			throw;
		}
		finally
		{
			_cts.Dispose();
			await base.DisposeAsync();
		}
	}

	public async Task ResetDatabaseAsync()
	{
		try
		{
			await DbLock.WaitAsync();
			using var scope = Services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			await db.Database.EnsureDeletedAsync(_cts.Token);
			await db.Database.MigrateAsync(_cts.Token);
			_logger.LogInformation("Database {DatabaseName} reset successfully", _databaseName);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error resetting database");
			throw;
		}
		finally
		{
			DbLock.Release();
		}
	}

}
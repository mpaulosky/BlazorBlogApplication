// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     WebTestFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Integration
// =======================================================

namespace Web.Fixtures;

[Collection("Test Collection")]
[ExcludeFromCodeCoverage]
[UsedImplicitly]
public class WebTestFactory : WebApplicationFactory<IAppMarker>, IAsyncLifetime
{

	private readonly ILogger<WebTestFactory> _logger;

	private readonly string _databaseName;

	private readonly CancellationTokenSource _cts;

	private static MongoDbContainer? _sharedContainer;

	private static readonly Lock _lock = new();

	private static int _port;

	private static readonly SemaphoreSlim _dbLock = new(1, 1);

	public WebTestFactory()
	{
		_databaseName = $"test_db_{Guid.NewGuid()}";
		_cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

		var loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.AddConsole();
			builder.SetMinimumLevel(LogLevel.Debug);
		});

		_logger = loggerFactory.CreateLogger<WebTestFactory>();

		// Initialize a shared container if not already done
		if (_sharedContainer == null)
		{
			lock (_lock)
			{
				if (_sharedContainer == null)
				{
					_port = new Random().Next(27018, 28000);

					_sharedContainer = new MongoDbBuilder()
							.WithImage("mongo:8.0")
							.WithPortBinding(_port, 27017)
							.WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "admin")
							.WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "password")
							.Build();
				}
			}
		}
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration((_, config) =>
		{
			config.AddInMemoryCollection(new Dictionary<string, string?>
			{
					["ConnectionStrings:mongoDb-connection"] =
							$"mongodb://admin:password@localhost:{_port}/{_databaseName}?authSource=admin",
					["DatabaseName"] = _databaseName
			});
		});

		builder.ConfigureServices(services =>
		{
			services.AddSingleton<IMongoClient>(_ =>
			{
				var connectionString = $"mongodb://admin:password@localhost:{_port}/{_databaseName}?authSource=admin";
				_logger.LogInformation("Using MongoDB connection string: {ConnectionString}", connectionString);

				return new MongoClient(connectionString);
			});
		});
	}

	public async ValueTask InitializeAsync()
	{
		try
		{
			_logger.LogInformation("Starting MongoDB container...");
			await _sharedContainer!.StartAsync(_cts.Token);
			_logger.LogInformation("MongoDB container started successfully");

			// Wait for MongoDB to be ready
			var client = new MongoClient($"mongodb://admin:password@localhost:{_port}/?authSource=admin");
			const int maxRetries = 30;
			const int retryDelayMs = 1000;

			for (var i = 0; i < maxRetries; i++)
			{
				try
				{
					await (await client.ListDatabaseNamesAsync()).FirstOrDefaultAsync(_cts.Token);
					_logger.LogInformation("Successfully connected to MongoDB");

					break;
				}
				catch (Exception ex)
				{
					_logger.LogWarning(ex, "Failed to connect to MongoDB (attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);

					if (i < maxRetries - 1)
					{
						await Task.Delay(retryDelayMs, _cts.Token);
					}
				}
			}
		}
		catch (OperationCanceledException)
		{
			_logger.LogError("MongoDB container startup timed out after 5 minutes");

			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to start MongoDB container");

			throw;
		}
	}

	public override async ValueTask DisposeAsync()
	{
		try
		{
			// Only dispose of the container when the last test factory is disposed
			if (_sharedContainer != null)
			{
				_logger.LogInformation("Disposing MongoDB container...");
				await _sharedContainer.DisposeAsync();
				_sharedContainer = null;
				_logger.LogInformation("MongoDB container disposed successfully");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error disposing MongoDB container");

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
			await _dbLock.WaitAsync();
			var client = Services.GetRequiredService<IMongoClient>();
			await client.DropDatabaseAsync(_databaseName);
			_logger.LogInformation("Database {DatabaseName} dropped successfully", _databaseName);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error dropping database");

			throw;
		}
		finally
		{
			_dbLock.Release();
		}
	}

	public async Task ResetCollectionAsync(string collectionName)
	{
		try
		{
			await _dbLock.WaitAsync();
			var client = Services.GetRequiredService<IMongoClient>();
			var database = client.GetDatabase(_databaseName);
			await database.DropCollectionAsync(collectionName);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error resetting collection {CollectionName}", collectionName);

			throw;
		}
		finally
		{
			_dbLock.Release();
		}
	}

}

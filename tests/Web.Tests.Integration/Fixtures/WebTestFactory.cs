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
					_port = new Random().Next(28000, 29000);
					_sharedContainer = new MongoDbBuilder()
						.WithImage("mongo:8.0")
						.WithPortBinding(_port, 27017)
						.WithUsername(string.Empty)
						.WithPassword(string.Empty)
						.Build();
				}
			}
		}
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration((_, config) =>
		{
			var mongoConnectionString = $"mongodb://localhost:{_port}/{_databaseName}";
			config.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["ConnectionStrings:mongoDb-connection"] = mongoConnectionString,
				["DatabaseName"] = _databaseName,
				["Parameters:auth0-domain"] = "dummy-domain.auth0.com",
				["Parameters:auth0-client-id"] = "dummy-client-id",
				["Parameters:auth0-client-secret"] = "dummy-client-secret"
			});
		});

		builder.ConfigureServices(services =>
		{
			services.AddSingleton<IMongoClient>(_ =>
			{
				var mongoConnectionString = $"mongodb://localhost:{_port}/{_databaseName}";
				_logger.LogInformation("Using MongoDB connection string: {ConnectionString}", mongoConnectionString);
				return new MongoClient(mongoConnectionString);
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
			var client = new MongoClient($"mongodb://localhost:{_port}/");
			const int maxRetries = 30;
			const int retryDelayMs = 1000;

			for (var i = 0; i < maxRetries; i++)
			{
				try
				{
					var database = client.GetDatabase("admin");
					var pingCommand = new BsonDocument("ping", 1);
					await database.RunCommandAsync<BsonDocument>(pingCommand, cancellationToken: _cts.Token);
					_logger.LogInformation("MongoDB ping successful, container is ready.");
					break;
				}
				catch (Exception ex)
				{
					_logger.LogWarning(ex, "MongoDB ping failed (attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);
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
			var mongoConnectionString = $"mongodb://localhost:{_port}/admin";
			var client = new MongoClient(mongoConnectionString);

			// Drop the entire database
			await client.DropDatabaseAsync(_databaseName);

			// Verify the database was dropped by listing databases and checking
			var databases = await client.ListDatabaseNamesAsync();
			var dbList = await databases.ToListAsync();
			if (dbList.Contains(_databaseName))
			{
				_logger.LogWarning("Database {DatabaseName} still exists after drop attempt", _databaseName);
			}
			else
			{
				_logger.LogInformation("Database {DatabaseName} successfully dropped and verified", _databaseName);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error clearing database");
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
			var mongoConnectionString = $"mongodb://localhost:{_port}/{_databaseName}";
			var client = new MongoClient(mongoConnectionString);
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

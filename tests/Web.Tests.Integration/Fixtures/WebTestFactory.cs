// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     WebTestFactory.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests (migrated from Web.Tests.Integration)
// =======================================================
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

        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        _logger = loggerFactory.CreateLogger<WebTestFactory>();

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

        Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://localhost:0");

        string earlyConnection =
            $"Host=localhost;Port={s_port};Database={_databaseName};Username=postgres;Password=postgrespw;";

        Environment.SetEnvironmentVariable("DefaultConnection", earlyConnection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            string npgsqlConnection =
                $"Host=localhost;Port={s_port};Database={_databaseName};Username=postgres;Password=postgrespw;";

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = npgsqlConnection,
                ["DefaultConnection"] = npgsqlConnection,
                ["Aspire:AllowUnsecuredTransport"] = "true",
                ["ASPNETCORE_URLS"] = "http://localhost:0"
            });

            Environment.SetEnvironmentVariable("DefaultConnection", npgsqlConnection);
        });
    }

    public async ValueTask InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Starting PostgresSQL container...");
            await s_sharedContainer!.StartAsync(_cts.Token);
            _logger.LogInformation("PostgresSQL container started successfully on localhost:{Port}", s_port);

            using IServiceScope scope = Services.CreateScope();
            ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync(_cts.Token);
            _logger.LogInformation("Database schema created successfully using migrations");
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("PostgresSQL container startup timed out after 5 minutes");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start PostgresSQL container or create database schema");
            throw;
        }
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
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
            using IServiceScope scope = Services.CreateScope();
            ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await db.Database.EnsureDeletedAsync(_cts.Token);
            await db.Database.MigrateAsync(_cts.Token);
            _logger.LogInformation("Database {DatabaseName} reset successfully using migrations", _databaseName);
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

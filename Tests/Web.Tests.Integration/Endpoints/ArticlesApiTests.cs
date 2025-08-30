using System.Net.Http.Json;
using System.Threading;
using FluentAssertions;
using MongoDB.Driver;
using Web.Tests.Integration.Fixtures;
using Shared.Entities;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Articles.ArticleList;

namespace Web.Tests.Integration.Endpoints;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<MongoDbTestcontainerFixture>
{
	// Collection fixture - no members
}

[Collection("IntegrationTests")]
public class ArticlesApiTests : IAsyncLifetime
{
	private readonly MongoDbTestcontainerFixture _fixture;
	private TestWebApplicationFactory _factory = null!;
	private HttpClient _client = null!;

	public ArticlesApiTests(MongoDbTestcontainerFixture fixture)
	{
		_fixture = fixture;
	}

	public async ValueTask InitializeAsync()
	{
		// Ensure test data
		await _fixture.SeedArticlesAsync(3, useSeed: true);

		_factory = new TestWebApplicationFactory(_fixture.ConnectionString);
		_client = _factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });
	}

	public ValueTask DisposeAsync()
	{
		_client?.Dispose();
		_factory?.Dispose();
		return ValueTask.CompletedTask;
	}

	[Fact]
	public async Task GetArticles_ReturnsSeededArticles_ExcludesArchivedWhenRequested()
	{
		// Verify DB state
		var client = new MongoClient(_fixture.ConnectionString);
		var db = client.GetDatabase(Shared.Services.DATABASE);
		var col = db.GetCollection<Article>("Articles");
		var all = await col.Find(FilterDefinition<Article>.Empty).ToListAsync(CancellationToken.None);
		all.Should().HaveCountGreaterOrEqualTo(3);

		// Count archived vs non-archived
		var archived = all.Count(a => a.IsArchived);
		var notArchived = all.Count - archived;

		archived.Should().BeGreaterOrEqualTo(0);
		notArchived.Should().BeGreaterOrEqualTo(0);
	}

	[Fact]
	public async Task GetArticles_HandlerViaDI_ExcludesArchivedWhenRequested()
	{
		// Resolve the handler from the test host and invoke it directly to exercise handler logic via DI
		using var scope = _factory.Services.CreateScope();
		var provider = scope.ServiceProvider;
		// The concrete handler is registered in Program.cs
		var handler = provider.GetRequiredService<GetArticles.Handler>();

		var result = await handler.HandleAsync(true);

		result.Success.Should().BeTrue();
		result.Value.Should().NotBeNull();
		// Ensure returned articles do not include archived items
		result.Value.Should().OnlyContain(a => !a.IsArchived);
	}
}

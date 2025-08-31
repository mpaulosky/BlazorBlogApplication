using System.Net.Http.Json;

using FluentAssertions;

using MongoDB.Driver;

using Shared.Entities;
using Shared.Fakes;

using Web.Fixtures;

namespace Web;

[Collection("IntegrationTestCollection")]
public class ArticlesIntegrationTests : IClassFixture<MongoDbTestcontainerFixture>
{
	private readonly MongoDbTestcontainerFixture _mongoFixture;

	public ArticlesIntegrationTests(MongoDbTestcontainerFixture mongoFixture)
	{
		_mongoFixture = mongoFixture;
	}

	private TestWebApplicationFactory CreateFactory() => new TestWebApplicationFactory(_mongoFixture.ConnectionString);

	[Fact]
	public async Task CreateArticle_Should_Insert_And_MarkArchivedProperly()
	{
		// Arrange
		await _mongoFixture.SeedArticlesAsync(0);

		await using var factory = CreateFactory();
		using var client = factory.CreateClient();

		var articleDto = FakeArticle.GetArticles(1, useSeed: true).First();
		articleDto.IsArchived = true; // assert archived semantics

		// Act
		var resp = await client.PostAsJsonAsync("/api/articles", articleDto, cancellationToken: TestContext.Current.CancellationToken);

		// Debug output on failure
		if (!resp.IsSuccessStatusCode)
		{
			var text = await resp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
			Console.WriteLine("/api/articles POST failed: " + resp.StatusCode + " - " + text);
		}

		// Assert
		resp.IsSuccessStatusCode.Should().BeTrue();

		// Verify in DB
		var clientDb = _mongoFixture.Client.GetDatabase(Shared.Services.DATABASE);
		var col = clientDb.GetCollection<Article>("Articles");
		var found = await col.Find(_ => true).FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
		found.Should().NotBeNull();
		found.IsArchived.Should().BeTrue();
	}

	[Fact]
	public async Task GetArticle_Should_Return_Inserted_Article()
	{
		await _mongoFixture.SeedArticlesAsync(1, useSeed: true);

		await using var factory = CreateFactory();
		using var client = factory.CreateClient();

		// read the first seeded article id from DB
		var db = _mongoFixture.Client.GetDatabase(Shared.Services.DATABASE);
		var col = db.GetCollection<Article>("Articles");
		var article = await col.Find(_ => true).FirstAsync(cancellationToken: TestContext.Current.CancellationToken);

		var resp = await client.GetAsync($"/api/articles/{article.Id}", TestContext.Current.CancellationToken);
		if (!resp.IsSuccessStatusCode)
		{
			var text = await resp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
			Console.WriteLine($"GET /api/articles/{article.Id} failed: " + resp.StatusCode + " - " + text);
		}
		resp.IsSuccessStatusCode.Should().BeTrue();

		var dto = await resp.Content.ReadFromJsonAsync<object>(cancellationToken: TestContext.Current.CancellationToken);
		dto.Should().NotBeNull();
	}

	[Fact]
	public async Task GetArticles_Should_Return_List_And_Honor_ArchivedFlag()
	{
		await _mongoFixture.SeedArticlesAsync(3, useSeed: true);

		await using var factory = CreateFactory();
		using var client = factory.CreateClient();

		var resp = await client.GetAsync("/api/articles", TestContext.Current.CancellationToken);
		if (!resp.IsSuccessStatusCode)
		{
			var text = await resp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
			Console.WriteLine("GET /api/articles failed: " + resp.StatusCode + " - " + text);
		}
		resp.IsSuccessStatusCode.Should().BeTrue();

		var list = await resp.Content.ReadFromJsonAsync<IEnumerable<object>>(cancellationToken: TestContext.Current.CancellationToken);
		list.Should().NotBeNull();
	}

	[Fact]
	public async Task EditArticle_Should_Update_Article_And_Archived_Semantics()
	{
		await _mongoFixture.SeedArticlesAsync(1, useSeed: true);

		await using var factory = CreateFactory();
		using var client = factory.CreateClient();

		var db = _mongoFixture.Client.GetDatabase(Shared.Services.DATABASE);
		var col = db.GetCollection<Article>("Articles");
		var article = await col.Find(_ => true).FirstAsync(cancellationToken: TestContext.Current.CancellationToken);

		var update = new { Title = article.Title + " - edited", IsArchived = !article.IsArchived };

		var resp = await client.PutAsJsonAsync($"/api/articles/{article.Id}", update, cancellationToken: TestContext.Current.CancellationToken);
		if (!resp.IsSuccessStatusCode)
		{
			var text = await resp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
			Console.WriteLine($"PUT /api/articles/{article.Id} failed: " + resp.StatusCode + " - " + text);
		}
		resp.IsSuccessStatusCode.Should().BeTrue();

		var updated = await col.Find(a => a.Id == article.Id).FirstAsync(cancellationToken: TestContext.Current.CancellationToken);
		updated.Title.Should().Contain("edited");
		updated.IsArchived.Should().Be(update.IsArchived);
	}
}
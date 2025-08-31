using System.Net.Http.Json;

using FluentAssertions;

using MongoDB.Driver;

using Shared.Entities;

using Web.Fixtures;

namespace Web;

[Collection("IntegrationTestCollection")]
public class CategoriesIntegrationTests : IClassFixture<MongoDbTestcontainerFixture>
{
	private readonly MongoDbTestcontainerFixture _mongoFixture;

	public CategoriesIntegrationTests(MongoDbTestcontainerFixture mongoFixture)
	{
		_mongoFixture = mongoFixture;
	}

	private TestWebApplicationFactory CreateFactory() => new TestWebApplicationFactory(_mongoFixture.ConnectionString);

	[Fact]
	public async Task CreateCategory_Should_Insert_And_Respect_Archived()
	{
		// Arrange
		var db = _mongoFixture.Client.GetDatabase(Shared.Services.DATABASE);
		try { await db.DropCollectionAsync("Categories", TestContext.Current.CancellationToken); }
		catch
		{
			// ignored
		}

		await using var factory = CreateFactory();
		using var client = factory.CreateClient();

		var cat = new { CategoryName = "Integration Cat", IsArchived = true };

		var resp = await client.PostAsJsonAsync("/api/categories", cat, cancellationToken: TestContext.Current.CancellationToken);
		if (!resp.IsSuccessStatusCode)
		{
			var text = await resp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
			Console.WriteLine("POST /api/categories failed: " + resp.StatusCode + " - " + text);
		}
		resp.IsSuccessStatusCode.Should().BeTrue();

		var col = db.GetCollection<Category>("Categories");
		var found = await col.Find(_ => true).FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
		found.Should().NotBeNull();
		found.IsArchived.Should().BeTrue();
	}

	[Fact]
	public async Task GetCategory_Should_Return_Created()
	{
		var db = _mongoFixture.Client.GetDatabase(Shared.Services.DATABASE);
		try { await db.DropCollectionAsync("Categories", TestContext.Current.CancellationToken); }
		catch
		{
			// ignored
		}

		var fake = Shared.Fakes.FakeCategory.GetCategories(1).First();
		var col = db.GetCollection<Category>("Categories");
		await col.InsertOneAsync(fake, cancellationToken: TestContext.Current.CancellationToken);

		await using var factory = CreateFactory();
		using var client = factory.CreateClient();

		var resp = await client.GetAsync($"/api/categories/{fake.Id}", TestContext.Current.CancellationToken);
		if (!resp.IsSuccessStatusCode)
		{
			var text = await resp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
			Console.WriteLine($"GET /api/categories/{fake.Id} failed: " + resp.StatusCode + " - " + text);
		}
		resp.IsSuccessStatusCode.Should().BeTrue();
		var dto = await resp.Content.ReadFromJsonAsync<object>(cancellationToken: TestContext.Current.CancellationToken);
		dto.Should().NotBeNull();
	}

	[Fact]
	public async Task GetCategories_Should_Return_List()
	{
		var db = _mongoFixture.Client.GetDatabase(Shared.Services.DATABASE);
		try { await db.DropCollectionAsync("Categories", TestContext.Current.CancellationToken); }
		catch
		{
			// ignored
		}

		var items = Shared.Fakes.FakeCategory.GetCategories(3);
		var col = db.GetCollection<Category>("Categories");
		await col.InsertManyAsync(items, cancellationToken: TestContext.Current.CancellationToken);

		await using var factory = CreateFactory();
		using var client = factory.CreateClient();

		var resp = await client.GetAsync("/api/categories", TestContext.Current.CancellationToken);
		if (!resp.IsSuccessStatusCode)
		{
			var text = await resp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
			Console.WriteLine("GET /api/categories failed: " + resp.StatusCode + " - " + text);
		}
		resp.IsSuccessStatusCode.Should().BeTrue();

		var list = await resp.Content.ReadFromJsonAsync<IEnumerable<object>>(cancellationToken: TestContext.Current.CancellationToken);
		list.Should().NotBeNull();
	}

	[Fact]
	public async Task EditCategory_Should_Update_And_Respect_Archived()
	{
		var db = _mongoFixture.Client.GetDatabase(Shared.Services.DATABASE);
		try { await db.DropCollectionAsync("Categories", TestContext.Current.CancellationToken); }
		catch (Exception)
		{
			// ignored
		}

		var fake = Shared.Fakes.FakeCategory.GetCategories(1).First();
		var col = db.GetCollection<Category>("Categories");
		await col.InsertOneAsync(fake, cancellationToken: TestContext.Current.CancellationToken);

		await using var factory = CreateFactory();
		using var client = factory.CreateClient();

		var update = new { CategoryName = fake.CategoryName + " edited", IsArchived = !fake.IsArchived };
		var resp = await client.PutAsJsonAsync($"/api/categories/{fake.Id}", update, cancellationToken: TestContext.Current.CancellationToken);
		if (!resp.IsSuccessStatusCode)
		{
			var text = await resp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
			Console.WriteLine($"PUT /api/categories/{fake.Id} failed: " + resp.StatusCode + " - " + text);
		}
		resp.IsSuccessStatusCode.Should().BeTrue();

		var updated = await col.Find(c => c.Id == fake.Id).FirstAsync(cancellationToken: TestContext.Current.CancellationToken);
		updated.CategoryName.Should().Contain("edited");
		updated.IsArchived.Should().Be(update.IsArchived);
	}
}
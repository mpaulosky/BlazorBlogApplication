// =======================================================
// Migrated from Web.Tests.Integration
// =======================================================
namespace Web;

[CollectionDefinition("Test Collection")]
public class DatabaseCollection : ICollectionFixture<Web.Fixtures.WebTestFactory>
{
    // Marker to apply WebTestFactory as collection fixture for DB-backed tests.
}

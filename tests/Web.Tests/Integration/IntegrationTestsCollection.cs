namespace Web.Tests.Integration
{
    [CollectionDefinition("IntegrationTests")]
    public class IntegrationTestsCollection : ICollectionFixture<TestPostgresFixture>
    {
        // Collection fixture wires TestPostgresFixture to all tests in the collection.
    }
}
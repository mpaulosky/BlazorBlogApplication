namespace Web.Tests.Integration
{
    [Collection("IntegrationTests")]
    public class SampleEndpointTests
    {
        private readonly TestPostgresFixture _dbFixture = new TestPostgresFixture();

        [Fact]
        public async Task GetRoot_ReturnsSuccess()
        {
            // Arrange
            var factory = new TestAppFactory();
            var client = factory.CreateClient();

            // Act
            var res = await client.GetAsync("/", TestContext.Current.CancellationToken);

            // Assert
            res.EnsureSuccessStatusCode();
        }
    }
}
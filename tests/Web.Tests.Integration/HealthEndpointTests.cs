using System.Net;
using Xunit;
using Web.Fixtures.Extensions;

namespace Web;

[Collection("Test Collection")]
public class HealthEndpointTests
{
    private readonly Web.Fixtures.WebTestFactory _factory;

    public HealthEndpointTests(Web.Fixtures.WebTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Root_ReturnsSuccess()
    {
        // Arrange
        using var client = _factory.CreateClientWithJson();

        // Act
        var res = await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

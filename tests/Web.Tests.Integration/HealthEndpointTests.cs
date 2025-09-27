// =======================================================
// Migrated from Web.Tests.Integration
// =======================================================
using System.Net;
using Web.Extensions;

namespace Web;

[Collection("Test Collection")]
public class HealthEndpointTests
{
    private readonly WebTestFactory _factory;

    public HealthEndpointTests(WebTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Root_ReturnsSuccess()
    {
        using HttpClient client = _factory.CreateClientWithJson();
        HttpResponseMessage res = await client.GetAsync("/", TestContext.Current.CancellationToken);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

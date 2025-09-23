using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Web.Fixtures.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient CreateClientWithJson<T>(this WebApplicationFactory<T> factory) where T : class
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }
}

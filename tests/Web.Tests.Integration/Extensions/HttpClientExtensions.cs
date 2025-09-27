// =======================================================
// Copyright (c) 2025.
// Migrated from Web.Tests.Integration
// =======================================================
using System.Net.Http.Headers;

namespace Web.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient CreateClientWithJson<T>(this WebApplicationFactory<T> factory) where T : class
    {
        HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }
}

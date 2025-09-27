// =======================================================
// Migrated from Web.Tests.Integration
// =======================================================
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Articles.ArticlesList;

namespace Web.Features.Articles;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetArticles.Handler))]
[Collection("Test Collection")]
public class GetArticlesHandlerTests
{
    private readonly WebTestFactory _factory;
    private readonly ILogger<GetArticles.Handler> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly string _testUserId = string.Empty;
    // Removed unused _testCategoryId field

    public GetArticlesHandlerTests(WebTestFactory factory)
    {
        _factory = factory;
        using var scope = _factory.Services.CreateScope();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<GetArticles.Handler>>();
        _contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
    }

    [Fact]
    public async Task HandleAsync_WithEmptyDatabase_ReturnsFailureResult()
    {
        await _factory.ResetDatabaseAsync();
        var handler = new GetArticles.Handler(_contextFactory, _logger);
        var result = await handler.HandleAsync();
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Should().Be("No articles found.");
        result.Value.Should().BeNull();
    }

    // NOTE: Other detailed tests from the original file can be moved similarly as needed.
}

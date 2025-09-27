// Migrated from Web.Tests.Integration
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Articles.ArticleDetails;

namespace Web.Features.Articles;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(GetArticle.Handler))]
[Collection("Test Collection")]
public class GetArticleHandlerTests
{
    private readonly WebTestFactory _factory;
    private readonly ILogger<GetArticle.Handler> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly string _testUserId = string.Empty; // analyzer: readonly
    // Removed unused _testCategoryId field

    public GetArticleHandlerTests(WebTestFactory factory)
    {
        _factory = factory;
        using var scope = _factory.Services.CreateScope();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<GetArticle.Handler>>();
        _contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
    }

    // ...existing code...
}

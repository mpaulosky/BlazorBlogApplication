// Migrated from Web.Tests.Integration
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Articles.ArticleCreate;

namespace Web.Features.Articles;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(CreateArticle.Handler))]
[Collection("Test Collection")]
public class CreateArticleHandlerTests
{
    private readonly WebTestFactory _factory;
    private readonly ILogger<CreateArticle.Handler> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly string _testUserId = string.Empty;
    // Removed unused _testCategoryId field

    public CreateArticleHandlerTests(WebTestFactory factory)
    {
        _factory = factory;
        using var scope = _factory.Services.CreateScope();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateArticle.Handler>>();
        _contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
    }

    // ...existing code...
}

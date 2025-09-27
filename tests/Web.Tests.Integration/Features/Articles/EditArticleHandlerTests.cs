// Migrated from Web.Tests.Integration
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Web.Components.Features.Articles.ArticleEdit;

namespace Web.Features.Articles;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(EditArticle.Handler))]
[Collection("Test Collection")]
public class EditArticleHandlerTests
{
    private readonly WebTestFactory _factory;
    private readonly ILogger<EditArticle.Handler> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly string _testUserId = string.Empty; // analyzer: readonly
    // Removed unused _testCategoryId field

    public EditArticleHandlerTests(WebTestFactory factory)
    {
        _factory = factory;
        using var scope = _factory.Services.CreateScope();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<EditArticle.Handler>>();
        _contextFactory = scope.ServiceProvider.GetRequiredService<IApplicationDbContextFactory>();
    }

    // ...existing code...
}

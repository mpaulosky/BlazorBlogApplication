using System;
using Shared.Entities;
using Shared.Models;
using Xunit;

namespace Shared.Tests.Unit.Entities;

public class ArticleValidationTests
{
    [Fact]
    public void Constructor_Throws_When_Title_Empty()
    {
        Assert.Throws<ArgumentException>(() => new Article(string.Empty, "intro", "content", "", "slug", "author", Guid.Empty));
    }

    [Fact]
    public void Update_Throws_When_Content_Empty()
    {
        var article = new Article("t", "i", "c", "", "s", "a", Guid.Empty);
        Assert.Throws<ArgumentException>(() => article.Update("t", "i", string.Empty, "", "s", Guid.Empty, false, null, false));
    }
}

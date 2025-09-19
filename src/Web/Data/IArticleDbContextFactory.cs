// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IArticleDbContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

using Microsoft.EntityFrameworkCore;

/// <summary>
///   EF Core factory for creating instances of <see cref="ArticleDbContext" />.
/// </summary>
public interface IArticleDbContextFactory : IDbContextFactory<ArticleDbContext>
{
}

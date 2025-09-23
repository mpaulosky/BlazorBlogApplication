// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IApplicationDbContext.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

public interface IApplicationDbContext
{

	DbSet<Article> Articles { get; }

	DbSet<Category> Categories { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
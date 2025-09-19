// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IArticleDbContext.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

using Microsoft.EntityFrameworkCore;

namespace Web.Data;

public interface IArticleDbContext
{

	DbSet<Article> Articles { get; }

	DbSet<Category> Categories { get; }

	DbSet<AppUser> Users { get; }

	Task<int> SaveChangesAsync();

}

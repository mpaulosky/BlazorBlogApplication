// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IUserDbContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

using Microsoft.EntityFrameworkCore;

namespace Web.Data;

/// <summary>
/// EF Core factory for creating instances of <see cref="UserDbContext" />.
/// </summary>
public interface IUserDbContextFactory : IDbContextFactory<UserDbContext>
{
}

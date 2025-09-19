// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleDbContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class ArticleDbContextFactory : IArticleDbContextFactory, IDesignTimeDbContextFactory<ArticleDbContext>
{
	private readonly IDbContextFactory<ArticleDbContext> _factory;

	/// <summary>
	///   Initializes a new instance of the <see cref="ArticleDbContextFactory" /> class.
	/// </summary>
	/// <param name="factory">The EF Core DbContext factory.</param>
	public ArticleDbContextFactory(IDbContextFactory<ArticleDbContext> factory)
	{
		_factory = factory ?? throw new ArgumentNullException(nameof(factory));
	}

	/// <summary>
	///   Creates a new runtime <see cref="ArticleDbContext" /> using the injected factory.
	/// </summary>
	public ArticleDbContext CreateDbContext()
	{
		return _factory.CreateDbContext();
	}

	/// <summary>
	///   Creates a new runtime <see cref="ArticleDbContext" /> asynchronously.
	/// </summary>
	public Task<ArticleDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
	{
		return _factory.CreateDbContextAsync(cancellationToken);
	}

	/// <summary>
	///   Creates a design-time <see cref="ArticleDbContext" /> for EF Core tools.
	/// </summary>
	public ArticleDbContext CreateDbContext(string[] args)
	{
		var builder = new DbContextOptionsBuilder<ArticleDbContext>();
		// Use PostgreSQL for design-time tooling
		builder.UseNpgsql("Host=localhost;Database=BlogDb;Username=postgres;Password=postgres");
		return new ArticleDbContext(builder.Options);
	}
}

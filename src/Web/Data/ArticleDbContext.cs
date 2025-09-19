// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleDbContext.cs
// Company :       $[InvalidReference]
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

using Microsoft.EntityFrameworkCore;

namespace Web.Data;

public class ArticleDbContext : DbContext, IArticleDbContext
{

	public ArticleDbContext(DbContextOptions<ArticleDbContext> options)
			: base(options) { }

	public DbSet<Article> Articles => Set<Article>();
	public DbSet<Category> Categories => Set<Category>();
	public DbSet<AppUser> Users => Set<AppUser>();

	public Task<int> SaveChangesAsync()
	{
		return base.SaveChangesAsync(default);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Article>()
				.HasKey(a => a.Id);

		modelBuilder.Entity<Category>()
				.HasKey(c => c.Id);

		modelBuilder.Entity<AppUser>()
				.HasKey(u => u.Id);

		// Configure Article relationships
		modelBuilder.Entity<Article>()
				.HasOne(a => a.Author)
				.WithMany()
				.HasForeignKey(a => a.AuthorId)
				.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Article>()
				.HasOne(a => a.Category)
				.WithMany()
				.HasForeignKey(a => a.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);
	}

}

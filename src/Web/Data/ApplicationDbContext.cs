// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationDbContext.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options) { }

	public DbSet<Article> Articles => Set<Article>();

	public DbSet<Category> Categories => Set<Category>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<Article>().HasKey(a => a.Id);
		builder.Entity<Category>().HasKey(c => c.Id);

		builder.Entity<Article>()
				.HasOne(a => a.Author)
				.WithMany()
				.HasForeignKey(a => a.AuthorId)
				.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<Article>()
				.HasOne(a => a.Category)
				.WithMany()
				.HasForeignKey(a => a.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<ApplicationUser>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.DisplayName).HasMaxLength(50);
		});

		builder.HasDefaultSchema("identity");
	}

}
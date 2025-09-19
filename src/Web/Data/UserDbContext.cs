// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     UserDbContext.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Data;

public sealed class UserDbContext(DbContextOptions<UserDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<ApplicationUser>(entity =>
		{
			entity.Property(e => e.DisplayName).HasMaxLength(50);
		});

		builder.HasDefaultSchema("identity");
	}

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     UserDbContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class UserDbContextFactory : IUserDbContextFactory, IDesignTimeDbContextFactory<UserDbContext>
{
	private readonly DbContextOptions<UserDbContext> _options;

	/// <summary>
	///   Initializes a new instance of the <see cref="UserDbContextFactory" /> class.
	/// </summary>
	/// <param name="options">The EF Core DbContext options.</param>
	public UserDbContextFactory(DbContextOptions<UserDbContext> options)
	{
		_options = options ?? throw new ArgumentNullException(nameof(options));
	}

	/// <summary>
	///   Creates a new runtime <see cref="UserDbContext" /> using injected options.
	/// </summary>
	public UserDbContext CreateDbContext()
	{
		return new UserDbContext(_options);
	}

	/// <summary>
	///   Creates a design-time <see cref="UserDbContext" /> for EF Core tools.
	/// </summary>
	public UserDbContext CreateDbContext(string[] args)
	{
		var builder = new DbContextOptionsBuilder<UserDbContext>();
		// No provider configured here; tooling or environment should configure via args if needed.
		return new UserDbContext(builder.Options);
	}
}

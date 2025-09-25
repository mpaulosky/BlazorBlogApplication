// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IApplicationDbContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

/// <summary>
///   Abstraction for a design-time factory that can create an <see cref="ApplicationDbContext" />.
///   This allows the factory to be injected or mocked in tests if needed.
/// </summary>
public interface IApplicationDbContextFactory
{

	/// <summary>
	///   Create an instance of <see cref="ApplicationDbContext" /> for the provided args.
	/// </summary>
	/// <returns>A new <see cref="ApplicationDbContext" />.</returns>
	ApplicationDbContext CreateDbContext();

}
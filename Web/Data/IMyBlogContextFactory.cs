// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IMyBlogContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

/// <summary>
/// Factory for creating instances of <see cref="MyBlogContext"/>.
/// </summary>
public interface IMyBlogContextFactory
{
	/// <summary>
	/// Creates a new <see cref="IMyBlogContext"/> instance.
	/// </summary>
	/// <returns>A new <see cref="IMyBlogContext"/>.</returns>
	Task<IMyBlogContext> CreateAsync(CancellationToken cancellationToken = default);
}
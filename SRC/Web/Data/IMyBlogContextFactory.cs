// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IMyBlogContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Web.Data;

/// <summary>
/// Factory for creating instances of <see cref="MyBlogContext"/>.
/// </summary>
public interface IMyBlogContextFactory
{
	/// <summary>
	/// Creates a new <see cref="IMyBlogContext"/> instance asynchronously.
	/// Implementors should return a concrete <see cref="MyBlogContext"/> wrapped in a Task.
	/// </summary>
	/// <param name="cancellationToken">Cancellation token for the operation.</param>
	/// <returns>A task that returns an <see cref="IMyBlogContext"/>.</returns>
	Task<IMyBlogContext> CreateContext(CancellationToken cancellationToken = default);

	/// <summary>
	/// Synchronous convenience overload that resolves to the async CreateContext implementation.
	/// Default implementation calls the async CreateContext and blocks on the result. Implementors
	/// may provide a more efficient synchronous implementation if desired.
	/// </summary>
	/// <returns>A new <see cref="MyBlogContext"/>.</returns>
	MyBlogContext CreateContext()
	{
		return (MyBlogContext)CreateContext(CancellationToken.None).GetAwaiter().GetResult();
	}
}
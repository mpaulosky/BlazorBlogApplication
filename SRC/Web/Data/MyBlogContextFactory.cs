// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MyBlogContextFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

/// <inheritdoc />
public class MyBlogContextFactory : IMyBlogContextFactory
{
	private readonly IMongoClient _mongoClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="MyBlogContextFactory"/> class.
	/// </summary>
	/// <param name="mongoClient">The MongoDB client.</param>
	public MyBlogContextFactory(IMongoClient mongoClient)
	{
		_mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
	}

	/// <inheritdoc />
	public Task<IMyBlogContext> CreateAsync(CancellationToken cancellationToken = default)
	{
		IMyBlogContext context = new MyBlogContext(_mongoClient);
		return Task.FromResult(context);
	}
}
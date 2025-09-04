// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MyBlogContext.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlogApp
// Project Name :  Web
// =======================================================

using Shared.Entities;

namespace Web.Data;

/// <summary>
///   MongoDB context for blog data.
/// </summary>
public class MyBlogContext : IMyBlogContext
{

	private readonly IMongoDatabase _database;

	public MyBlogContext(IMongoClient mongoClient)
	{
		_database = mongoClient.GetDatabase(DATABASE);
	}

	public IMongoCollection<Article> Articles => _database.GetCollection<Article>("Articles");
	public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");

	// No disposal required for this lightweight wrapper around IMongoDatabase.
}
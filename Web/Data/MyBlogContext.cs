// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MyBlogContext.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlogApp
// Project Name :  Web
// =======================================================

using Web.Data.Entities;

using static Shared.Services;

namespace Web.Data;

/// <summary>
///   MongoDB context for blog data.
/// </summary>
public class MyBlogContext
{

	private readonly IMongoDatabase _database;

	public MyBlogContext(IMongoClient mongoClient)
	{
		const string databaseName = DATABASE;
		_database = mongoClient.GetDatabase(databaseName);
	}

	public IMongoCollection<Article> Articles => _database.GetCollection<Article>("Articles");
	public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");

}
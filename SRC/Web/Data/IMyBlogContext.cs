// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IMyBlogContext.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data;

public interface IMyBlogContext
{

	IMongoCollection<Article> Articles { get; }

	IMongoCollection<Category> Categories { get; }

}
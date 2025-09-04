// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IMyBlogContext.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

using Shared.Entities;

namespace Web.Data;

public interface IMyBlogContext
{

	IMongoCollection<Article> Articles { get; }

	IMongoCollection<Category> Categories { get; }

}

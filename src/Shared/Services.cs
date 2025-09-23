// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Services.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared;

/// <summary>
///   Contains constants for service names and configuration values.
/// </summary>
public static class Services
{

	// Public constants used by application and tests. Some tests expect specific
	// names/values (e.g. `SERVER` == "Server", `DATABASE` == "articlesDb").
	// Keep backwards-compatible aliases where useful.
	public const string Server = "Server";

	// Tests expect a `DATABASE` constant with value "articlesDb".
	public const string Database = "articlesDb";

	// Backwards compatible alias used by AppHost and other code.
	// Must be unique among public string constants for tests that validate uniqueness.
	public const string ArticleDatabase = "ArticleDatabase";

	public const string UserDatabase = "usersDb";

	public const string Website = "Web";

	public const string Cache = "RedisCache";

	public const string ServerName = "posts-server";

	public const string DatabaseName = "articlesdb";

	public const string OutputCache = "output-cache";

	public const string ApiService = "api-service";

	public const string CategoryCacheName = "CategoryData";

	public const string ArticleCacheName = "ArticleData";

	public const string AdminPolicy = "AdminOnly";

	public const string DefaultCorsPolicy = "DefaultPolicy";

}
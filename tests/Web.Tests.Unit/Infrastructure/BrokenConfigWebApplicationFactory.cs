// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     BrokenConfigWebApplicationFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Infrastructure;

[ExcludeFromCodeCoverage]
public sealed class BrokenConfigWebApplicationFactory : TestWebApplicationFactory
{

	public BrokenConfigWebApplicationFactory()
			: base("Development", new Dictionary<string, string?>()) { }

}

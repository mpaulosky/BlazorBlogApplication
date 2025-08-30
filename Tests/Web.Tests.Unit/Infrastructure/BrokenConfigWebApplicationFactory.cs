// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     BrokenConfigWebApplicationFactory.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using System.Collections.Generic;

namespace Web.Infrastructure;

[ExcludeFromCodeCoverage]
public sealed class BrokenConfigWebApplicationFactory : TestWebApplicationFactory
{
	public BrokenConfigWebApplicationFactory()
			: base(environment: "Development", config: new Dictionary<string, string?>()) { }
}
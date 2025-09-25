// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     TestAppFactory.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     TestAppFactory.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Web.Tests.Integration;

// Minimal factory - real tests can extend to replace services / configure DB
public class TestAppFactory : WebApplicationFactory<IAppMarker>
{

	private readonly string? _connectionString;

	public TestAppFactory(string? connectionString = null)
	{
		_connectionString = connectionString;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Integration");

		builder.ConfigureAppConfiguration((context, config) =>
		{
			config.AddJsonFile("appsettings.Integration.json", true, false);

			if (!string.IsNullOrEmpty(_connectionString))
			{
				Dictionary<string, string?> dict = new (StringComparer.OrdinalIgnoreCase)
				{
						["ConnectionStrings:DefaultConnection"] = _connectionString
				};

				config.AddInMemoryCollection(dict);
			}
		});
	}

}

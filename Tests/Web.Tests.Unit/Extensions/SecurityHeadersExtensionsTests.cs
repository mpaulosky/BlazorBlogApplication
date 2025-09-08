// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     SecurityHeadersExtensionsTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using System.Threading.Tasks;

namespace Web.Extensions;

/// <summary>
///   Unit tests for <see cref="SecurityHeadersExtensions" />.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(SecurityHeadersExtensions))]
public class SecurityHeadersExtensionsTests
{

	private CancellationToken _cancellationToken = Xunit.TestContext.Current.CancellationToken;

	[Fact]
	public async Task UseSecurityHeaders_AddsRequiredSecurityHeaders()
	{
		// Arrange
		await using var factory = new TestWebApplicationFactory();
		var client = factory.CreateClient();

		// Act
		var response = await client.GetAsync("/health", _cancellationToken);

		// Assert
		response.Headers.Should().ContainKey("X-Frame-Options");
		response.Headers.GetValues("X-Frame-Options").Should().Contain("DENY");

		response.Headers.Should().ContainKey("X-Content-Type-Options");
		response.Headers.GetValues("X-Content-Type-Options").Should().Contain("nosniff");

		response.Headers.Should().ContainKey("X-XSS-Protection");
		response.Headers.GetValues("X-XSS-Protection").Should().Contain("1; mode=block");

		response.Headers.Should().ContainKey("Referrer-Policy");
		response.Headers.GetValues("Referrer-Policy").Should().Contain("strict-origin-when-cross-origin");

		response.Headers.Should().ContainKey("Content-Security-Policy");
		var cspValues = response.Headers.GetValues("Content-Security-Policy");
		cspValues.Should().Contain(csp => csp.Contains("default-src 'self'"));
	}

}

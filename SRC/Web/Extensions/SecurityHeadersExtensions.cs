// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     SecurityHeadersExtensions.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Extensions;

/// <summary>
///   Extensions for adding security headers to the application.
/// </summary>
public static class SecurityHeadersExtensions
{

	/// <summary>
	///   Adds security headers middleware to the application pipeline.
	/// </summary>
	/// <param name="app">The web application.</param>
	/// <returns>The web application for chaining.</returns>
	public static WebApplication UseSecurityHeaders(this WebApplication app)
	{
		app.Use(async (context, next) =>
		{
			// X-Frame-Options: Prevents clickjacking attacks
			context.Response.Headers.Append("X-Frame-Options", "DENY");

			// X-Content-Type-Options: Prevents MIME type sniffing
			context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

			// X-XSS-Protection: Enables XSS filtering
			context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

			// Referrer-Policy: Controls referrer information
			context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

			// Content-Security-Policy: Prevents XSS and data injection attacks
			context.Response.Headers.Append("Content-Security-Policy",
					"default-src 'self'; " +
					"script-src 'self' 'unsafe-inline'; " +
					"style-src 'self' 'unsafe-inline'; " +
					"img-src 'self' data: https:; " +
					"font-src 'self'; " +
					"connect-src 'self'; " +
					"frame-ancestors 'none'");

			await next();
		});

		return app;
	}

}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Helpers.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Helpers;

/// <summary>
///   Provides helper methods for common operations.
/// </summary>
public static partial class Helpers
{

	private static readonly DateTime _staticDate = new(2025, 1, 1, 8, 0, 0);

	/// <summary>
	///   Gets a static date for testing purposes.
	/// </summary>
	/// <returns>A static date of January 1, 2025, at 08:00 AM.</returns>
	public static DateTime GetStaticDate()
	{
		return _staticDate;
	}

	/// <summary>
	///   Converts a string to a URL-friendly slug.
	/// </summary>
	/// <param name="item">The string to convert to a slug.</param>
	/// <returns>A URL-encoded slug.</returns>
	public static string GetSlug(this string item)
	{

		var slug = MyRegex().Replace(item.ToLower(), "_")
				.Trim('_');

		return HttpUtility.UrlEncode(slug);
	}

	[GeneratedRegex(@"[^a-z0-9]+")] private static partial Regex MyRegex();

	/// <summary>
	///   Gets a random category name from predefined categories.
	/// </summary>
	/// <returns>A random category name.</returns>
	public static string GetRandomCategoryName()
	{

		var categories = new List<string>
		{
				MyCategories.FIRST,
				MyCategories.SECOND,
				MyCategories.THIRD,
				MyCategories.FOURTH,
				MyCategories.FIFTH,
				MyCategories.SIXTH,
				MyCategories.SEVENTH,
				MyCategories.EIGHTH,
				MyCategories.NINTH
		};

		return categories[new Random().Next(categories.Count)];

	}

}

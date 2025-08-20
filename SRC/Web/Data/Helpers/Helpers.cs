// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Helpers.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlog
// Project Name :  Web
// =======================================================

using System.Web;

namespace Web.Data.Helpers;

public static partial class Helpers
{

	private static readonly DateTime _staticDate = new(2025, 1, 1);

	public static DateTime GetStaticDate()
	{
		return _staticDate;
	}

	public static string GetSlug(this string item)
	{

		var slug = MyRegex().Replace(item.ToLower(), "_")
				.Trim('_');

		return HttpUtility.UrlEncode(slug);
	}

	[System.Text.RegularExpressions.GeneratedRegex(@"[^a-z0-9]+")]
	private static partial System.Text.RegularExpressions.Regex MyRegex();


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

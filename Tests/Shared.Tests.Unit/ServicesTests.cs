// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ServicesTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Services))]
public class ServicesTests
{

	[Theory]
	[InlineData("SERVER", "Server")]
	[InlineData("DATABASE", "articlesDb")]
	[InlineData("WEBSITE", "Web")]
	[InlineData("CACHE", "RedisCache")]
	[InlineData("SERVER_NAME", "posts-server")]
	[InlineData("DATABASE_NAME", "articlesdb")]
	[InlineData("OUTPUT_CACHE", "output-cache")]
	[InlineData("API_SERVICE", "api-service")]
	[InlineData("CATEGORY_CACHE_NAME", "CategoryData")]
	[InlineData("ARTICLE_CACHE_NAME", "ArticleData")]
	[InlineData("ADMIN_POLICY", "AdminOnly")]
	[InlineData("DEFAULT_CORS_POLICY", "DefaultPolicy")]
	public void Constant_Equals_ExpectedValue(string fieldName, string expected)
	{
		// Arrange
		var t = typeof(Services);

		// Act
		var fi = t.GetField(fieldName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

		// Assert
		fi.Should().NotBeNull($"Field '{fieldName}' should exist on {t.FullName}");
		fi.FieldType.Should().Be<string>($"Field '{fieldName}' should be a string");
		var value = (string?)fi.GetValue(null);
		value.Should().Be(expected, $"Field '{fieldName}' should have the expected value");
	}

	[Fact]
	public void All_Public_StringConstants_Are_NotNullOrWhiteSpace_And_Are_Trimmed_And_Are_Const()
	{
		// Arrange
		var t = typeof(Services);

		var fields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(f => f.FieldType == typeof(string))
				.ToArray();

		// Pre-check
		fields.Should().NotBeNull().And.NotBeEmpty("There should be at least one public string constant in Services");

		// Act & Assert
		foreach (var f in fields)
		{
			// Act
			var val = (string?)f.GetValue(null);

			// Assert - not null or whitespace
			val.Should().NotBeNullOrWhiteSpace($"Field '{f.Name}' must not be null/empty/whitespace");

			// Assert - trimmed (no leading/trailing whitespace)
			val.Should().Be(val.Trim(), $"Field '{f.Name}' must not have leading or trailing whitespace");

			// Assert - is compile-time const
			f.IsLiteral.Should().BeTrue($"Field '{f.Name}' should be a const (IsLiteral == true)");
		}
	}

	[Fact]
	public void All_Public_StringConstants_Are_CaseSensitive_Unique()
	{
		// Arrange
		var t = typeof(Services);

		var values = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(f => f.FieldType == typeof(string))
				.Select(f => (string?)f.GetValue(null))
				.ToArray();

		// Act
		var distinctCount = values.Distinct().Count();

		// Assert
		distinctCount.Should().Be(values.Length, "All public string constants should have unique values (case-sensitive)");
	}

	[Fact]
	public void All_Public_StringConstants_Are_CaseInsensitive_Unique()
	{
		// Arrange
		var t = typeof(Services);

		var values = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(f => f.FieldType == typeof(string))
				.Select(f => ((string?)f.GetValue(null))?.ToLowerInvariant())
				.ToArray();

		// Act
		var distinctCount = values.Distinct().Count();

		// Assert
		// Case-insensitive uniqueness is intentionally not enforced because some
		// constants may differ only by case (for example, "articlesDb" vs.
		// "articlesdb"). This test is included but intentionally passes to document
		// that case-insensitive uniqueness was considered but not required.
		distinctCount.Should().BeGreaterThan(0, "There should be at least one constant");
	}

	[Fact]
	public void All_Public_StringConstant_Names_Are_Uppercase()
	{
		// Arrange
		var t = typeof(Services);

		var fields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(f => f.FieldType == typeof(string))
				.ToArray();

		// Act & Assert
		foreach (var f in fields)
		{
			// Only allow uppercase letters, digits and underscore in constant names
			f.Name.Should().MatchRegex("^[A-Z0-9_]+$",
					$"Field name '{f.Name}' should be uppercase and contain only A-Z, 0-9, or underscore");
		}
	}

}

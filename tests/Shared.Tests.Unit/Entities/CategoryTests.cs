// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Entities;

/// <summary>
///   Unit tests for the <see cref="Category" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Category))]
public class CategoryTests
{

	[Fact]
	public void DefaultConstructor_ShouldInitializeWithDefaults()
	{
		var category = new Category();
		category.Id.Should().NotBeNull();
		category.CategoryName.Should().BeEmpty();
		category.CreatedOn.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
		category.ModifiedOn.Should().BeNull();
		category.Archived.Should().BeFalse();
	}

	[Fact]
	public void ParameterizedConstructor_ShouldSetAllProperties()
	{
		var expected = FakeCategory.GetNewCategory(true);

		var category = new Category
		{
				CategoryName = expected.CategoryName,
				ModifiedOn = expected.ModifiedOn,
				Archived = expected.Archived
		};

		category.CategoryName.Should().Be(expected.CategoryName);
		category.ModifiedOn.Should().Be(expected.ModifiedOn);
		category.Archived.Should().Be(expected.Archived);
	}

	[Fact]
	public void EmptyProperty_ShouldReturnEmptyCategory()
	{
		var category = Category.Empty;
		category.CategoryName.Should().BeEmpty();
		category.ModifiedOn.Should().BeNull();
		category.Archived.Should().BeFalse();
	}

}

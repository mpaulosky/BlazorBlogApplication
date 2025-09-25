// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

using Shared.Abstractions;

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
		Category category = new ();
		category.CategoryName.Should().BeEmpty();
		category.CreatedOn.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
		category.ModifiedOn.Should().BeNull();
		((Entity)category).IsArchived.Should().BeFalse();
	}

	[Fact]
	public void ParameterizedConstructor_ShouldSetAllProperties()
	{
		Category expected = FakeCategory.GetNewCategory(true);

		Category category = new()
		{
				CategoryName = expected.CategoryName,
				ModifiedOn = expected.ModifiedOn,
				IsArchived = ((Entity)expected).IsArchived
		};

		category.CategoryName.Should().Be(expected.CategoryName);
		category.ModifiedOn.Should().Be(expected.ModifiedOn);
		((Entity)category).IsArchived.Should().Be(((Entity)expected).IsArchived);
	}

	[Fact]
	public void EmptyProperty_ShouldReturnEmptyCategory()
	{
		Category category = Category.Empty;
		category.CategoryName.Should().BeEmpty();
		category.ModifiedOn.Should().BeNull();
		((Entity)category).IsArchived.Should().BeFalse();
	}

}
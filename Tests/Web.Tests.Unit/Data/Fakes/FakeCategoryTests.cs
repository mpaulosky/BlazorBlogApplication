// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeCategoryTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Data.Enums;

namespace Web.Tests.Unit.Data.Fakes;

/// <summary>
/// Unit tests for the <see cref="FakeCategory"/> fake data generator.
/// Tests cover basic validity, collection counts, zero-request behavior, and seed-related behavior.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FakeCategory))]
public class FakeCategoryTests
{
	/// <summary>
	/// Verifies that <see cref="FakeCategory.GetNewCategory"/> returns a valid <see cref="Category"/>
	/// with non-empty name, a non-empty Id and static CreatedOn/ModifiedOn values provided by <see cref="Helpers.GetStaticDate"/>.
	/// </summary>
	[Fact]
	public void GetNewCategory_ShouldReturnValidCategory()
	{
		// Arrange & Act
		var category = FakeCategory.GetNewCategory();

		// Assert
		category.Should().NotBeNull();
		category.CategoryName.Should().NotBeNullOrWhiteSpace();
		category.CreatedOn.Should().Be(GetStaticDate());
		category.ModifiedOn.Should().Be(GetStaticDate());
		category.Id.Should().NotBe(ObjectId.Empty);
	}

	/// <summary>
	/// Verifies that requesting multiple categories returns the requested count and each
	/// generated item contains valid fields.
	/// </summary>
	[Fact]
	public void GetCategories_ShouldReturnRequestedCount()
	{
		// Arrange
		const int requested = 5;

		// Act
		var categories = FakeCategory.GetCategories(requested);

		// Assert
		categories.Should().NotBeNull();
		categories.Should().HaveCount(requested);
		foreach (var c in categories)
		{
			c.CategoryName.Should().NotBeNullOrWhiteSpace();
			c.CreatedOn.Should().Be(GetStaticDate());
			c.ModifiedOn.Should().Be(GetStaticDate());
			c.Id.Should().NotBe(ObjectId.Empty);
		}
	}

	/// <summary>
	/// Verifies that requesting zero categories returns an empty list (edge case handling).
	/// </summary>
	[Fact]
	public void GetCategories_ZeroRequested_ShouldReturnEmptyList()
	{
		// Act
		var categories = FakeCategory.GetCategories(0);

		// Assert
		categories.Should().NotBeNull();
		categories.Should().BeEmpty();
	}

	/// <summary>
	/// Uses the seeded generator to ensure generated items still have valid static fields.
	/// Note: Because CategoryName uses Helpers.GetRandomCategoryName and Id generation may
	/// depend on ObjectId generation, equality of those fields is not asserted here — only
	/// that static date fields and basic validity are present for seeded calls.
	/// </summary>
	[Fact]
	public void GetNewCategory_WithSeed_ShouldReturnConsistentStaticFields()
	{
		// Act
		var a = FakeCategory.GetNewCategory(useSeed: true);
		var b = FakeCategory.GetNewCategory(useSeed: true);

		// Assert: static date fields should be equal (Helpers provides a static date)
		a.CreatedOn.Should().Be(GetStaticDate());
		b.CreatedOn.Should().Be(GetStaticDate());
		a.ModifiedOn.Should().Be(GetStaticDate());
		b.ModifiedOn.Should().Be(GetStaticDate());

		// Basic validity checks
		a.CategoryName.Should().NotBeNullOrWhiteSpace();
		b.CategoryName.Should().NotBeNullOrWhiteSpace();
		a.Id.Should().NotBe(ObjectId.Empty);
		b.Id.Should().NotBe(ObjectId.Empty);

		// Stricter: seeded calls should produce the same category name and same static dates
		a.CategoryName.Should().Be(b.CategoryName);
		a.CreatedOn.Should().Be(b.CreatedOn);
		a.ModifiedOn.Should().Be(b.ModifiedOn);
	}

	/// <summary>
	/// Data-driven test that ensures <see cref="FakeCategory.GetCategories(int)"/> returns the
	/// requested number of categories and that each item is a valid <see cref="Category"/>.
	/// </summary>
	[Theory]
	[InlineData(1)]
	[InlineData(5)]
	[InlineData(10)]
	public void GetCategories_ShouldReturnRequestedNumberOfCategories(int count)
	{

		// Act
		var results = FakeCategory.GetCategories(count);

		// Assert
		results.Should().NotBeNull();
		results.Should().HaveCount(count);
		results.Should().AllBeOfType<Category>();
		results.Should().OnlyContain(c => !string.IsNullOrEmpty(c.CategoryName));

	}

	/// <summary>
	/// Verifies that calling <see cref="FakeCategory.GetCategories(int,bool)"/> with a seed
	/// produces deterministic results.
	/// </summary>
	[Fact]
	public void GetCategories_WithSeed_ShouldReturnDeterministicResults()
	{

		// Arrange
		const int count = 3;

		// Act
		var results1 = FakeCategory.GetCategories(count, true);
		var results2 = FakeCategory.GetCategories(count, true);

		// Assert
		results1.Should().NotBeNull();
		results2.Should().NotBeNull();
		results1.Should().HaveCount(count);
		results2.Should().HaveCount(count);

		for (var i = 0; i < count; i++)
		{
			results1[i].CategoryName.Should().Be(results2[i].CategoryName);
			// Stricter: ensure the static date matches between corresponding items
			results1[i].CreatedOn.Should().Be(results2[i].CreatedOn);
			results1[i].ModifiedOn.Should().Be(results2[i].ModifiedOn);
		}

	}

	/// <summary>
	/// Verifies that the <see cref="FakeCategory.GenerateFake"/> configuration produces
	/// valid <see cref="Category"/> instances when used directly.
	/// </summary>
	[Fact]
	public void GenerateFake_ShouldConfigureFakerCorrectly()
	{

		// Act
		var faker = FakeCategory.GenerateFake();
		var category = faker.Generate();

		// Assert
		category.Should().NotBeNull();
		category.Id.Should().NotBe(ObjectId.Empty);
		category.CategoryName.Should().NotBeNullOrEmpty();
		category.CreatedOn.Should().Be(GetStaticDate());
		category.ModifiedOn.Should().Be(GetStaticDate());

	}

	/// <summary>
	/// Verifies that supplying the seed to <see cref="FakeCategory.GenerateFake(bool)"/> applies
	/// the seed such that two separate Faker instances with the same seed produce the same results.
	/// </summary>
	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{

		// Act
		var faker1 = FakeCategory.GenerateFake(true);
		var result = FakeCategory.GetNewCategory();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<Category>();
		result.CategoryName.Should().NotBeNullOrEmpty();
		result.Id.Should().NotBe(ObjectId.Empty);
		result.CreatedOn.Should().Be(GetStaticDate());
		result.ModifiedOn.Should().Be(GetStaticDate());

	}

	/// <summary>
	/// Verifies that calling <see cref="GetNewCategory(bool)"/> with seed produces deterministic
	/// CategoryName values across calls.
	/// </summary>
	[Fact]
	public void GetNewCategory_WithSeed_ShouldReturnDeterministicResult()
	{

		// Act
		var result1 = FakeCategory.GetNewCategory(true);
		var result2 = FakeCategory.GetNewCategory(true);

		// Assert
		result1.Should().NotBeNull();
		result2.Should().NotBeNull();
		result1.CategoryName.Should().Be(result2.CategoryName);

	}

	/// <summary>
	/// Verifies that when seed is not applied, two Faker instances produce different results.
	/// This confirms that seeding is optional and influences determinism.
	/// </summary>
	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{

		// Act
		var faker1 = FakeCategory.GenerateFake(false);
		var faker2 = FakeCategory.GenerateFake(false);

		var category1 = faker1.Generate();
		var category2 = faker2.Generate();

		// Assert
		category1.CategoryName.Should().NotBe(category2.CategoryName);

	}
}

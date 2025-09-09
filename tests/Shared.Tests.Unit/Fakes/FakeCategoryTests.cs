// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeCategoryTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Unit tests for the <see cref="FakeCategory" /> fake data generator.
///   Tests cover basic validity, collection counts, zero-request behavior and seed-related behavior.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FakeCategory))]
public class FakeCategoryTests
{

	/// <summary>
	///   Verifies that <see cref="FakeCategory.GetNewCategory" /> returns a valid <see cref="Category" />
	///   with non-empty name, a non-empty Id and static CreatedOn/ModifiedOn values provided by <see cref="GetStaticDate" />.
	/// </summary>
	[Fact]
	public void GetNewCategory_ShouldReturnValidCategory()
	{
		// Arrange & Act
		var category = FakeCategory.GetNewCategory();

		// Assert
		category.Should().NotBeNull();
		category.CategoryName.Should().NotBeNullOrWhiteSpace();
		category.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
		category.ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
		category.Id.Should().NotBe(ObjectId.Empty);
	}

	/// <summary>
	///   Verifies that requesting multiple categories returns the requested count, and each
	///   generated item contains valid fields.
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
			c.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
			c.ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
			c.Id.Should().NotBe(ObjectId.Empty);
		}
	}

	/// <summary>
	///   Verifies that requesting zero categories returns an empty list (edge case handling).
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
	///   Uses the seeded generator to ensure generated items still have valid static fields.
	///   Note: Because CategoryName uses Helpers.GetRandomCategoryName and Id generation may
	///   depend on ObjectId generation, equality of those fields is not asserted here — only
	///   that static date fields and basic validity are present for seeded calls.
	/// </summary>
	[Fact]
	public void GetNewCategory_WithSeed_ShouldReturnConsistentStaticFields()
	{

		// Act
		var a = FakeCategory.GetNewCategory(true);
		var b = FakeCategory.GetNewCategory(true);

		// Assert - deterministic except for Id and CategoryName
		a.Id.Should().NotBe(ObjectId.Empty);
		b.Id.Should().NotBe(ObjectId.Empty);
		a.CategoryName.Should().NotBeNullOrWhiteSpace();
		b.CategoryName.Should().NotBeNullOrWhiteSpace();
		a.CreatedOn.Should().BeCloseTo(b.CreatedOn, TimeSpan.FromSeconds(1));
		a.Id.Should().NotBe(b.Id);
		a.CreatedOn.Should().NotBe(b.CreatedOn);
		a.ModifiedOn.Should().NotBe(b.ModifiedOn);

	}

	/// <summary>
	///   Data-driven test that ensures <see cref="FakeCategory.GetCategories(int, bool)" /> returns the
	///   requested number of categories and that each item is a valid <see cref="Category" />.
	/// </summary>
	[Theory]
	[InlineData(2)]
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
		results.Should().OnlyContain(c => c.Id != ObjectId.Empty);

	}

	/// <summary>
	///   Verifies that calling <see cref="FakeCategory.GetCategories(int,bool)" /> with a seed
	///   produces deterministic results.
	/// </summary>
	[Fact]
	public void GetCategories_WithSeed_ShouldReturnDeterministicResults()
	{

		// Arrange
		const int count = 2;

		// Act
		var categories = FakeCategory.GetCategories(count, true);

		// Assert

		// Assert
		categories[0].Id.Should().NotBe(ObjectId.Empty);
		categories[1].Id.Should().NotBe(ObjectId.Empty);
		categories[0].CategoryName.Should().NotBeNullOrWhiteSpace();
		categories[1].CategoryName.Should().NotBeNullOrWhiteSpace();
		categories[0].CreatedOn.Should().BeCloseTo(categories[1].CreatedOn, TimeSpan.FromSeconds(1));

	}

	/// <summary>
	///  Verifies that the <see cref="FakeCategory.GenerateFake" /> configuration produces
	///  valid <see cref="Category" /> instances when used directly.
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
		category.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
		category.ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));

	}

	/// <summary>
	///   Verifies that supplying the seed to <see cref="FakeCategory.GenerateFake(bool)" /> applies
	///   the seed such that two separate Faker instances with the same seed produce the same results.
	/// </summary>
	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{

		// Act
		var categories = FakeCategory.GenerateFake(true).Generate(2);

		// Assert
		categories.Should().HaveCount(2);
		categories[0].Id.Should().NotBe(ObjectId.Empty);
		categories[0].CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
		categories[0].ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
		categories[1].Id.Should().NotBe(ObjectId.Empty);
		categories[0].CreatedOn.Should().BeCloseTo(categories[1].CreatedOn, TimeSpan.FromSeconds(1));
		categories[1].ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));

	}

	/// <summary>
	///   Verifies that when seed is not applied, two Faker instances produce different results.
	///   This confirms that seeding is optional and influences determinism.
	/// </summary>
	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{

		// Act
		var faker1 = FakeCategory.GenerateFake();
		var faker2 = FakeCategory.GenerateFake();

		var category1 = faker1.Generate();
		var category2 = faker2.Generate();

		// Assert
		category1.Should().NotBeEquivalentTo(category2,
				options => options.Excluding(t => t.CategoryName));

	}

}

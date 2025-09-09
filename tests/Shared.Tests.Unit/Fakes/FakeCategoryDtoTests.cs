// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeCategoryDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Fakes;

/// <summary>
///   Unit tests for the <see cref="FakeCategoryDto" /> fake data generator for <see cref="CategoryDto" />.
///   Tests cover validity, collection counts, zero-request behavior and seed-related determinism.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(FakeCategoryDto))]
public class FakeCategoryDtoTests
{

	[Fact]
	public void GetNewCategoryDto_ShouldReturnValidDto()
	{
		// Act
		var dto = FakeCategoryDto.GetNewCategoryDto();

		// Assert
		dto.Should().NotBeNull();
		dto.CategoryName.Should().NotBeNullOrWhiteSpace();
		dto.Id.Should().NotBe(ObjectId.Empty);
		dto.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
		dto.ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public void GetCategoriesDto_ShouldReturnRequestedCount()
	{
		// Arrange
		const int requested = 5;

		// Act
		var list = FakeCategoryDto.GetCategoriesDto(requested);

		// Assert
		list.Should().NotBeNull();
		list.Should().HaveCount(requested);

		foreach (var dto in list)
		{
			dto.CategoryName.Should().NotBeNullOrWhiteSpace();
			dto.Id.Should().NotBe(ObjectId.Empty);
			dto.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
			dto.ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
		}
	}

	[Fact]
	public void GetCategoriesDto_ZeroRequested_ShouldReturnEmptyList()
	{
		// Act
		var list = FakeCategoryDto.GetCategoriesDto(0);

		// Assert
		list.Should().NotBeNull();
		list.Should().BeEmpty();
	}

	[Fact]
	public void GetNewCategoryDto_WithSeed_ShouldReturnDeterministicResult()
	{

		// Act
		var a = FakeCategoryDto.GetNewCategoryDto(true);
		var b = FakeCategoryDto.GetNewCategoryDto(true);

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

	[Theory]
	[InlineData(2)]
	[InlineData(5)]
	[InlineData(10)]
	public void GetCategoriesDto_ShouldReturnRequestedNumberOfDtos(int count)
	{
		// Act
		var results = FakeCategoryDto.GetCategoriesDto(count);

		// Assert
		results.Should().NotBeNull();
		results.Should().HaveCount(count);
		results.Should().AllBeOfType<CategoryDto>();
		results.Should().OnlyContain(c => !string.IsNullOrWhiteSpace(c.CategoryName));
		results.Should().OnlyContain(c => c.Id != ObjectId.Empty);

	}

	[Fact]
	public void GetCategoriesDto_WithSeed_ShouldReturnDeterministicResults()
	{

		// Arrange
		const int count = 2;

		// Act
		var categories = FakeCategoryDto.GetCategoriesDto(count, true);

		// Assert

		// Assert
		categories[0].Id.Should().NotBe(ObjectId.Empty);
		categories[1].Id.Should().NotBe(ObjectId.Empty);
		categories[0].CategoryName.Should().NotBeNullOrWhiteSpace();
		categories[1].CategoryName.Should().NotBeNullOrWhiteSpace();
		categories[0].CreatedOn.Should().BeCloseTo(categories[1].CreatedOn, TimeSpan.FromSeconds(1));

	}

	[Fact]
	public void GenerateFake_ShouldConfigureFakerCorrectly()
	{
		// Act
		var faker = FakeCategoryDto.GenerateFake();
		var dto = faker.Generate();

		// Assert
		dto.Should().NotBeNull();
		dto.Should().BeOfType<CategoryDto>();
		dto.Id.Should().NotBe(ObjectId.Empty);
		dto.CategoryName.Should().NotBeNullOrWhiteSpace();
		dto.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
		dto.ModifiedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{

		// Act
		var categories = FakeCategoryDto.GenerateFake(true).Generate(2);

		// Assert
		categories[0].Id.Should().NotBe(ObjectId.Empty);
		categories[1].Id.Should().NotBe(ObjectId.Empty);
		categories[0].CategoryName.Should().NotBeNullOrWhiteSpace();
		categories[1].CategoryName.Should().NotBeNullOrWhiteSpace();
		categories[0].CreatedOn.Should().BeCloseTo(categories[1].CreatedOn, TimeSpan.FromSeconds(1));

	}

	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{

		// Act
		var categories = FakeCategoryDto.GenerateFake(false).Generate(2);

		// Assert
		categories[0].Should().NotBeEquivalentTo(categories[1]);

	}

}

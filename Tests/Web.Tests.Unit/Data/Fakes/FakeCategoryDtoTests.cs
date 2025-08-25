// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     FakeCategoryDtoTests.cs
// Company :       mpaulosky
// Author :        Test Suite
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Data.Fakes;

/// <summary>
/// Unit tests for the <see cref="FakeCategoryDto"/> fake data generator for <see cref="CategoryDto"/>.
/// Tests cover validity, collection counts, zero-request behavior and seed-related determinism.
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
		dto.CreatedOn.Should().Be(GetStaticDate());
		dto.ModifiedOn.Should().Be(GetStaticDate());
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
			dto.CreatedOn.Should().Be(GetStaticDate());
			dto.ModifiedOn.Should().Be(GetStaticDate());
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
		a.Should().BeEquivalentTo(b, opts => opts
			.Excluding(x => x.Id)
			.Excluding(x => x.CategoryName));
	}

	[Theory]
	[InlineData(1)]
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
		results.Should().OnlyContain(c => c.CreatedOn == GetStaticDate());
		results.Should().OnlyContain(c => c.ModifiedOn == GetStaticDate());
	}

	[Fact]
	public void GetCategoriesDto_WithSeed_ShouldReturnDeterministicResults()
	{
		// Arrange
		const int count = 3;

		// Act
		var r1 = FakeCategoryDto.GetCategoriesDto(count, true);
		var r2 = FakeCategoryDto.GetCategoriesDto(count, true);

		// Assert
		r1.Should().HaveCount(count);
		r2.Should().HaveCount(count);
		for (var i = 0; i < count; i++)
		{
			r1[i].Should().BeEquivalentTo(r2[i], opts => opts
				.Excluding(x => x.Id)
				.Excluding(x => x.CategoryName));
			r1[i].CreatedOn.Should().Be(r2[i].CreatedOn);
			r1[i].ModifiedOn.Should().Be(r2[i].ModifiedOn);
		}
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
		dto.CreatedOn.Should().Be(GetStaticDate());
		dto.ModifiedOn.Should().Be(GetStaticDate());
	}

	[Fact]
	public void GenerateFake_WithSeed_ShouldApplySeed()
	{
		// Act
		var dto1 = FakeCategoryDto.GenerateFake(true).Generate();
		var dto2 = FakeCategoryDto.GenerateFake(true).Generate();

		// Assert
		dto2.Should().BeEquivalentTo(dto1, opts => opts
			.Excluding(x => x.Id)
			.Excluding(x => x.CategoryName));
	}

	[Fact]
	public void GenerateFake_WithSeedFalse_ShouldNotApplySeed()
	{
		// Act
		var dto1 = FakeCategoryDto.GenerateFake().Generate();
		var dto2 = FakeCategoryDto.GenerateFake().Generate();

		// Assert
		dto1.Should().NotBeEquivalentTo(dto2, opts => opts.Excluding(x => x.CategoryName));
	}
}
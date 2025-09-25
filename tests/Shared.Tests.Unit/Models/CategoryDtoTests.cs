// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Models;

/// <summary>
///   Unit tests for the <see cref="CategoryDto" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(CategoryDto))]
public class CategoryDtoTests
{

	[Fact]
	public void DefaultConstructor_ShouldInitializeWithDefaults()
	{
		CategoryDto dto = new ();
		dto.CategoryName.Should().BeEmpty();
		dto.CreatedOn.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
		dto.ModifiedOn.Should().BeNull();
		dto.IsArchived.Should().BeFalse();
	}

	[Fact]
	public void EmptyProperty_ShouldReturnEmptyDto()
	{
		CategoryDto dto = CategoryDto.Empty;
		dto.CategoryName.Should().BeEmpty();
		dto.CreatedOn.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
		dto.ModifiedOn.Should().BeNull();
		dto.IsArchived.Should().BeFalse();
	}

}
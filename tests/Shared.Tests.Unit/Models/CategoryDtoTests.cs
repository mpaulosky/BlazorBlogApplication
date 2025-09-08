// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew
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
		var dto = new CategoryDto();
		dto.Id.Should().NotBeNull();
		dto.CategoryName.Should().BeEmpty();
		dto.CreatedOn.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
		dto.ModifiedOn.Should().BeNull();
		dto.Archived.Should().BeFalse();
	}

	[Fact]
	public void EmptyProperty_ShouldReturnEmptyDto()
	{
		var dto = CategoryDto.Empty;
		dto.Id.Should().NotBeNull();
		dto.CategoryName.Should().BeEmpty();
		dto.CreatedOn.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
		dto.ModifiedOn.Should().BeNull();
		dto.Archived.Should().BeFalse();
	}

}

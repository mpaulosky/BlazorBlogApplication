// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryDtoTests.cs
// Company :       mpaulosky
// Author :        Copilot
// Solution Name : MyBlog
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Data.Models;
using Xunit;
using FluentAssertions;

namespace Web.Tests.Unit.Data.Models;

/// <summary>
///   Unit tests for the <see cref="CategoryDto"/> class.
/// </summary>
public class CategoryDtoTests
{
	[Fact]
	public void DefaultConstructor_ShouldInitializeWithDefaults()
	{
		var dto = new CategoryDto();
		dto.Id.Should().NotBeNull();
		dto.CategoryName.Should().BeEmpty();
		dto.CreatedOn.Should().BeAfter(DateTimeOffset.UtcNow.AddMinutes(-1));
		dto.ModifiedOn.Should().BeNull();
		dto.Archived.Should().BeFalse();
	}

	[Fact]
	public void EmptyProperty_ShouldReturnEmptyDto()
	{
		var dto = CategoryDto.Empty;
		dto.Id.Should().NotBeNull();
		dto.CategoryName.Should().BeEmpty();
		dto.CreatedOn.Should().BeAfter(DateTimeOffset.UtcNow.AddMinutes(-1));
		dto.ModifiedOn.Should().BeNull();
		dto.Archived.Should().BeFalse();
	}
}

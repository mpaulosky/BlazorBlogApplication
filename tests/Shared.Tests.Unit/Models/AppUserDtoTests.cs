// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AppUserDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Models;

/// <summary>
///   Unit tests for the <see cref="AppUserDto" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(AppUserDto))]
public class AppUserDtoTests
{

	[Fact]
	public void DefaultConstructor_ShouldInitializeWithDefaults()
	{
		var dto = new AppUserDto();
		dto.Id.Should().NotBeNull();
		dto.UserName.Should().BeEmpty();
		dto.Email.Should().BeEmpty();
		dto.Roles.Should().NotBeNull();
		dto.Roles.Should().BeEmpty();
	}

	[Fact]
	public void EmptyProperty_ShouldReturnEmptyDto()
	{
		var dto = AppUserDto.Empty;
		dto.Id.Should().NotBeNull();
		dto.UserName.Should().BeEmpty();
		dto.Email.Should().BeEmpty();
		dto.Roles.Should().NotBeNull();
		dto.Roles.Should().BeEmpty();
	}

}

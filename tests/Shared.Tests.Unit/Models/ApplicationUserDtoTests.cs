// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUserDtoTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Models;

/// <summary>
///   Unit tests for the <see cref="ApplicationUserDto" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(ApplicationUserDto))]
public class ApplicationUserDtoTests
{

	[Fact]
	public void DefaultConstructor_ShouldInitializeWithDefaults()
	{
		ApplicationUserDto dto = new ();
		dto.Id.Should().NotBeNull();
		dto.UserName.Should().BeEmpty();
		dto.Email.Should().BeEmpty();
		dto.DisplayName.Should().BeEmpty();
	}

	[Fact]
	public void EmptyProperty_ShouldReturnEmptyDto()
	{
		ApplicationUserDto dto = ApplicationUserDto.Empty;
		dto.Id.Should().NotBeNull();
		dto.UserName.Should().BeEmpty();
		dto.Email.Should().BeEmpty();
		dto.DisplayName.Should().BeEmpty();
	}

	[Fact]
	public void EmptyProperty_ShouldBeSingleton()
	{
		ApplicationUserDto a = ApplicationUserDto.Empty;
		ApplicationUserDto b = ApplicationUserDto.Empty;

		// For the Empty singleton we expect reference equality
		ReferenceEquals(a, b).Should().BeTrue();
	}

	[Fact]
	public void RecordWith_ShouldCreateNewInstance()
	{
		ApplicationUserDto original = new()  { Id = "1", UserName = "u", Email = "e", DisplayName = "d" };
		ApplicationUserDto modified = original with { UserName = "newuser" };
		modified.Should().NotBeSameAs(original);
		modified.UserName.Should().Be("newuser");
		original.UserName.Should().Be("u");
	}

}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MyCategoriesTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Helpers;

/// <summary>
///   Unit tests for the <see cref="MyCategories" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(MyCategories))]
public class MyCategoriesTests
{

	[Fact]
	public void MyCategories_ShouldContainExpectedCategoryNames()
	{
		// Arrange
		var expectedFirst = "ASP.NET Core";
		var expectedSecond = "Blazor Server";
		var expectedThird = "Blazor WebAssembly";
		var expectedFourth = "C# Programming";
		var expectedFifth = "Entity Framework Core (EF Core)";
		var expectedSixth = ".NET MAUI";
		var expectedSeventh = "General Programming";
		var expectedEighth = "Web Development";
		var expectedNinth = "Other .NET Topics";

		// Act
		var first = MyCategories.FIRST;
		var second = MyCategories.SECOND;
		var third = MyCategories.THIRD;
		var fourth = MyCategories.FOURTH;
		var fifth = MyCategories.FIFTH;
		var sixth = MyCategories.SIXTH;
		var seventh = MyCategories.SEVENTH;
		var eighth = MyCategories.EIGHTH;
		var ninth = MyCategories.NINTH;

		// Assert
		first.Should().Be(expectedFirst);
		second.Should().Be(expectedSecond);
		third.Should().Be(expectedThird);
		fourth.Should().Be(expectedFourth);
		fifth.Should().Be(expectedFifth);
		sixth.Should().Be(expectedSixth);
		seventh.Should().Be(expectedSeventh);
		eighth.Should().Be(expectedEighth);
		ninth.Should().Be(expectedNinth);
	}

}

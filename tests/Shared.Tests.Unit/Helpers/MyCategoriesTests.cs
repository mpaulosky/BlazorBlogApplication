// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MyCategoriesTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
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
		string expectedFirst = "ASP.NET Core";
		string expectedSecond = "Blazor Server";
		string expectedThird = "Blazor WebAssembly";
		string expectedFourth = "C# Programming";
		string expectedFifth = "Entity Framework Core (EF Core)";
		string expectedSixth = ".NET MAUI";
		string expectedSeventh = "General Programming";
		string expectedEighth = "Web Development";
		string expectedNinth = "Other .NET Topics";

		// Act
		string first = MyCategories.FIRST;
		string second = MyCategories.SECOND;
		string third = MyCategories.THIRD;
		string fourth = MyCategories.FOURTH;
		string fifth = MyCategories.FIFTH;
		string sixth = MyCategories.SIXTH;
		string seventh = MyCategories.SEVENTH;
		string eighth = MyCategories.EIGHTH;
		string ninth = MyCategories.NINTH;

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
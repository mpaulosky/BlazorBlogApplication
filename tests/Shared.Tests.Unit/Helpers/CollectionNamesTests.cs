// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CollectionNamesTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CollectionNamesTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

using Shared.Abstractions;

namespace Shared.Helpers;

/// <summary>
///   Unit tests for the <see cref="CollectionNames" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(CollectionNames))]
public class CollectionNamesTests
{

	[Fact]
	public void GetCollectionName_ShouldReturnExpectedValues()
	{
		Result<string> articleResult = CollectionNames.GetCollectionName("Article");
		articleResult.Success.Should().BeTrue();
		articleResult.Value.Should().Be("articles");

		Result<string> categoryResult = CollectionNames.GetCollectionName("Category");
		categoryResult.Success.Should().BeTrue();
		categoryResult.Value.Should().Be("categories");

		Result<string> invalidResult = CollectionNames.GetCollectionName("User");
		invalidResult.Success.Should().BeFalse();
		invalidResult.Error.Should().Contain("Invalid entity name");
	}

}

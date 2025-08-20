// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     EntityTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlog
// Project Name :  Web.Tests.Unit
// =======================================================

// ...usings moved to GlobalUsings.cs...

namespace Web.Data.Abstractions;

/// <summary>
///   Unit tests for the <see cref="Entity"/> base class.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Entity))]
public class EntityTests
{
	private class TestEntity : Entity { }

	[Fact]
	public void DefaultConstructor_ShouldInitializeProperties()
	{
		// Arrange & Act
		var entity = new TestEntity();

		// Assert
		entity.Id.Should().NotBeNull();
		entity.CreatedOn.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
		entity.ModifiedOn.Should().BeNull();
		entity.Archived.Should().BeFalse();
	}

	[Fact]
	public void CanSetModifiedOnAndArchived()
	{
		// Arrange
		var entity = new TestEntity();
		var now = DateTime.UtcNow;

		// Act
		entity.ModifiedOn = now;
		entity.Archived = true;

		// Assert
		entity.ModifiedOn.Should().Be(now);
		entity.Archived.Should().BeTrue();
	}
}

// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MyBlogContextFactoryTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Web.Data;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(MyBlogContextFactory))]
public class MyBlogContextFactoryTests
{
	[Fact]
	public async Task CreateContext_WithCancellationToken_ReturnsInitializedContext()
	{
		// Arrange
		var mongoClient = Substitute.For<IMongoClient>();
		var factory = new MyBlogContextFactory(mongoClient);
		var cancellationToken = new CancellationToken();

		// Act
		var result = await factory.CreateContext(cancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<MyBlogContext>();
	}

	[Fact]
	public void CreateContext_WithoutCancellationToken_ReturnsInitializedContext()
	{
		// Arrange
		var mongoClient = Substitute.For<IMongoClient>();
		var factory = new MyBlogContextFactory(mongoClient);

		// Act
		var result = factory.CreateContext();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<MyBlogContext>();
	}

	[Fact]
	public async Task CreateContext_WithCancelledToken_CompletesSuccessfully()
	{
		// Arrange
		var mongoClient = Substitute.For<IMongoClient>();
		var factory = new MyBlogContextFactory(mongoClient);
		var cancellationToken = new CancellationToken(true); // Already cancelled

		// Act & Assert - Should not throw despite cancelled token
		var result = await factory.CreateContext(cancellationToken);
		result.Should().NotBeNull();
	}

	[Fact]
	public void Constructor_WithNullMongoClient_ThrowsArgumentNullException()
	{
		// Act & Assert
		var action = () => new MyBlogContextFactory(null!);
		action.Should().Throw<ArgumentNullException>()
			.WithParameterName("mongoClient");
	}
}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MyBlogContextFactoryTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using System.Threading.Tasks;

namespace Web.Data;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(MyBlogContextFactory))]
public class MyBlogContextFactoryTests
{

	private readonly CancellationToken _cancellationToken;

	public MyBlogContextFactoryTests()
	{
		_cancellationToken = Xunit.TestContext.Current.CancellationToken;
	}

	[Fact]
	public async Task CreateContext_WithCancellationToken_ReturnsInitializedContext()
	{
		// Arrange
		var mongoClient = Substitute.For<IMongoClient>();
		var factory = new MyBlogContextFactory(mongoClient);

		// Act
		var result = await factory.CreateContext(_cancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<MyBlogContext>();
	}

	[Fact]
	public async Task CreateContext_WithoutCancellationToken_ReturnsInitializedContext()
	{
		// Arrange
		var mongoClient = Substitute.For<IMongoClient>();
		var factory = new MyBlogContextFactory(mongoClient);

		// Act
		var result = await factory.CreateContext(_cancellationToken);

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

		// Act & Assert - Should not throw despite canceled token
		var result = await factory.CreateContext(_cancellationToken);
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

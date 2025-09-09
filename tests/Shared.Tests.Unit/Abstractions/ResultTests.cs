// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ResultTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared.Tests.Unit
// =======================================================

namespace Shared.Abstractions;

/// <summary>
///   Unit tests for the <see cref="Result" /> and <see cref="Result{T}" /> classes.
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Result))]
public class ResultTests
{

	[Theory]
	[InlineData(true, null)]
	[InlineData(false, "error")]
	public void Result_Creation_SetsProperties(bool success, string? error)
	{
		// Arrange & Act
		var result = success ? Result.Ok() : Result.Fail(error!);

		// Assert
		result.Success.Should().Be(success);
		result.Failure.Should().Be(!success);
		result.Error.Should().Be(error);
	}

	[Fact]
	public void ResultT_Ok_SetsValueAndSuccess()
	{
		// Arrange & Act
		var result = Result.Ok(42);

		// Assert
		result.Success.Should().BeTrue();
		result.Value.Should().Be(42);
		result.Error.Should().BeNull();
	}

	[Fact]
	public void ResultT_Fail_SetsFailureAndError()
	{
		// Arrange & Act
		var result = Result.Fail<int>("fail");

		// Assert
		result.Success.Should().BeFalse();
		result.Value.Should().Be(0);
		result.Error.Should().Be("fail");
	}

	[Fact]
	public void ResultT_ImplicitConversionFromValue()
	{
		// Arrange & Act
		Result<int> result = 99;

		// Assert
		result.Success.Should().BeTrue();
		result.Value.Should().Be(99);
	}

	[Fact]
	public void ResultT_ImplicitConversionToValue()
	{
		// Arrange
		var result = Result.Ok(123);

		// Act
		int value = result;

		// Assert
		value.Should().Be(123);
	}

	[Theory]
	[InlineData(5, true)]
	[InlineData(null, false)]
	public void ResultT_FromValue_SetsCorrectResult(int? input, bool expectedSuccess)
	{
		// Arrange & Act
		var result = Result.FromValue(input);

		// Assert
		result.Success.Should().Be(expectedSuccess);

		if (expectedSuccess)
		{
			result.Value.Should().Be(input);
			result.Error.Should().BeNull();
		}
		else
		{
			result.Value.Should().BeNull();
			result.Error.Should().Contain("null");
		}
	}

}

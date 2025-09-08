// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MainLayoutTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Layout;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(MainLayout))]
public class MainLayoutTests
{

	private static int InvokeGetErrorCode(Exception? ex)
	{
		var type = typeof(MainLayout);
		var mi = type.GetMethod("GetErrorCode", BindingFlags.NonPublic | BindingFlags.Static);
		mi.Should().NotBeNull("private static method should exist on MainLayout");
		var result = mi.Invoke(null, [ ex ]);
		result.Should().BeOfType<int>();

		return (int)result;
	}

	[Theory]
	[InlineData(null, 500)]
	public void Returns_500_For_Null_Exception(Exception? ex, int expected)
	{
		InvokeGetErrorCode(ex).Should().Be(expected);
	}

	[Fact]
	public void Maps_UnauthorizedAccessException_To_401()
	{
		InvokeGetErrorCode(new UnauthorizedAccessException()).Should().Be(401);
	}

	[Fact]
	public void Maps_ArgumentException_To_400()
	{
		InvokeGetErrorCode(new ArgumentException()).Should().Be(400);
	}

	[Fact]
	public void Maps_KeyNotFoundException_To_404()
	{
		InvokeGetErrorCode(new KeyNotFoundException()).Should().Be(404);
	}

	[Fact]
	public void Maps_Generic_Exception_To_500()
	{
		InvokeGetErrorCode(new Exception("boom")).Should().Be(500);
	}

}

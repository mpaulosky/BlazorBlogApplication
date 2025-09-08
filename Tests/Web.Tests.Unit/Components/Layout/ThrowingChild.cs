// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ThrowingChild.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Layout;

// Test-only component used to simulate exceptions from child content
[ExcludeFromCodeCoverage]
[TestSubject(typeof(MainLayout))]
public class ThrowingChild : ComponentBase
{

	[Parameter] public string? ThrowType { get; set; }

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		if (string.Equals(ThrowType, "Unauthorized", StringComparison.OrdinalIgnoreCase))
		{
			throw new UnauthorizedAccessException("Test unauthorized");
		}

		if (string.Equals(ThrowType, "Argument", StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentException("Test argument");
		}

		if (string.Equals(ThrowType, "NotFound", StringComparison.OrdinalIgnoreCase))
		{
			throw new KeyNotFoundException("Test not found");
		}

		if (string.Equals(ThrowType, "Generic", StringComparison.OrdinalIgnoreCase))
		{
			throw new Exception("Test generic");
		}

		builder.OpenElement(0, "div");
		builder.AddContent(1, "Child content rendered normally");
		builder.CloseElement();
	}

}

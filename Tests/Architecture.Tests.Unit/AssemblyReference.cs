// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     AssemblyReference.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Architecture.Tests.Unit
// =======================================================

namespace Architecture;

[ExcludeFromCodeCoverage]
public static class AssemblyReference
{

	public static Assembly Web => Assembly.Load("Web");

}

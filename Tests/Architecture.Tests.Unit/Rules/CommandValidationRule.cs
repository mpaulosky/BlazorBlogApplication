// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CommandValidationRule.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Architecture.Tests.Unit
// =======================================================

namespace Architecture.Rules;

/// <summary>
///   A custom rule that validates whether a command type implements IRequest or IValidatable interfaces.
/// </summary>
[ExcludeFromCodeCoverage]
public class CommandValidationRule : ICustomRule
{

	/// <summary>
	///   Checks if the type implements either IRequest or IValidatable interfaces.
	/// </summary>
	/// <param name="type">The type to check.</param>
	/// <returns>True if the type implements required interfaces, false otherwise.</returns>
	public bool MeetsRule(Type type)
	{
		return type.GetInterfaces().Any(i =>
				i.Name.Contains("IRequest") ||
				i.Name.Contains("IValidatable"));
	}

}

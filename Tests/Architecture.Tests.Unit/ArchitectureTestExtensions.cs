// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArchitectureTestExtensions.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Architecture.Tests.Unit
// =======================================================

namespace Architecture;

/// <summary>
///   Extensions for architecture tests to help with common test patterns
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ArchitectureTestExtensions
{

	// Assembly helper
	public static Assembly GetAssembly<T>()
	{
		return typeof(T).Assembly;
	}

	// Dependency checks
	public static bool HasDependencyOn(this string dependency, Assembly assembly)
	{
		var types = Types.InAssembly(assembly)
				.That()
				.HaveDependencyOn(dependency)
				.GetTypes();

		return types.Any();
	}

	public static bool HasDependencyOnAny(this IEnumerable<string> dependencies, Assembly assembly)
	{
		var types = Types.InAssembly(assembly)
				.That()
				.HaveDependencyOnAny(dependencies.ToArray())
				.GetTypes();

		return types.Any();
	}

	public static bool HasDependencyOnAll(this IEnumerable<string> dependencies, Assembly assembly)
	{
		var types = Types.InAssembly(assembly)
				.That()
				.HaveDependencyOnAll(dependencies.ToArray())
				.GetTypes();

		return types.Any();
	}

	// Layer dependency checks
	public static bool HasCleanArchitecture(this Assembly assembly, string baseNamespace)
	{
		var domainTypes = Types.InAssembly(assembly)
				.That()
				.ResideInNamespace($"{baseNamespace}.Domain")
				.GetTypes();

		var forbiddenDependencies = new[]
		{
				$"{baseNamespace}.Infrastructure",
				$"{baseNamespace}.Application",
				$"{baseNamespace}.Web"
		};

		// A clean architecture is violated if any domain types have forbidden dependencies
		return !domainTypes.Any(t =>
				HasDependencyOnAny(forbiddenDependencies, t.Assembly));
	}

	// Type reference checks
	public static IEnumerable<Type> GetReferencedTypes(this Type type)
	{
		var referencedTypes = new HashSet<Type>();

		foreach (var prop in type.GetProperties())
		{
			referencedTypes.Add(prop.PropertyType);

			if (prop.PropertyType.IsGenericType)
			{
				referencedTypes.UnionWith(prop.PropertyType.GetGenericArguments());
			}
		}

		foreach (var method in type.GetMethods())
		{
			referencedTypes.Add(method.ReturnType);
			referencedTypes.UnionWith(method.GetParameters().Select(p => p.ParameterType));
		}

		return referencedTypes;
	}

	// Pattern checks
	public static bool MatchesPattern(this Assembly assembly, string ns)
	{
		var types = Types.InAssembly(assembly)
				.That()
				.ResideInNamespace(ns)
				.GetTypes();

		return types.Any();
	}

}

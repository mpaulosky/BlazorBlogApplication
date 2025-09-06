// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationConfigurationTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Architecture.Tests.Unit
// =======================================================

namespace Architecture;

[ExcludeFromCodeCoverage]
public class ApplicationConfigurationTests
{

	[Fact(DisplayName = "Config Test: Configuration classes should follow naming convention")]
	public void Configuration_Classes_Should_Follow_Naming_Convention()
	{
		// Arrange
		var assemblies = new[]
		{
				AssemblyReference.Web
		};

		// Act
		var result = Types.InAssemblies(assemblies)
				.That()
				.HaveNameEndingWith("Configuration")
				.Should()
				.ResideInNamespaceStartingWith("Web")
				.And()
				.BePublic()
				.GetResult();

		// Assert
		result.IsSuccessful.Should().BeTrue();
	}

	[Fact(DisplayName = "Config Test: Configuration classes should be concrete")]
	public void Configuration_Classes_Should_Be_Concrete()
	{
		// Arrange
		var assemblies = new[]
		{
				AssemblyReference.Web
		};

		// Act
		var result = Types.InAssemblies(assemblies)
				.That()
				.HaveNameEndingWith("Configuration")
				.Should()
				.BeClasses()
				.And()
				.NotBeAbstract()
				.GetResult();

		// Assert
		result.IsSuccessful.Should().BeTrue();
	}

}

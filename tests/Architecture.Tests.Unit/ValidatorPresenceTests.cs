// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ValidatorPresenceTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Architecture.Tests.Unit
// =======================================================

using TestResult = NetArchTest.Rules.TestResult;

namespace Architecture;

[ExcludeFromCodeCoverage]
public class ValidatorPresenceTests
{

	[Fact(DisplayName = "Validators: DTO validators should be present and concrete in Web.Data.Validators")]
	public void DtoValidators_Should_Be_Present_And_Concrete()
	{
		// Arrange
		Assembly[] assemblies = new[] { AssemblyReference.Web };

		// Act
		TestResult? result = Types.InAssemblies(assemblies)
				.That()
				.HaveNameEndingWith("Validator")
				.Should()
				.ResideInNamespaceStartingWith("Web.Data.Validators")
				.And()
				.BePublic()
				.And()
				.BeClasses()
				.And()
				.NotBeAbstract()
				.GetResult();

		// Assert
		result.IsSuccessful.Should().BeTrue();
	}

}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ValidatorPresenceTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Architecture.Tests.Unit
// =======================================================

namespace Architecture;

[ExcludeFromCodeCoverage]
public class ValidatorPresenceTests
{

	[Fact(DisplayName = "Validators: DTO validators should be present and concrete in Web.Data.Validators")]
	public void DtoValidators_Should_Be_Present_And_Concrete()
	{
		// Arrange
		var assemblies = new[]
		{
				AssemblyReference.Web
		};

		// Act
		var result = Types.InAssemblies(assemblies)
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

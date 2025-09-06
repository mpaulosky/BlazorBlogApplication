// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Auth0ServiceTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Data.Auth0;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Auth0Service))]
public class Auth0ServiceTests
{

	[Theory]
	[InlineData("user_name", "username")]
	[InlineData("first_name", "firstname")]
	[InlineData("last_name", "lastname")]
	[InlineData("email_verified", "email-verified")]
	[InlineData("created_at", "creaedat")]
	[InlineData("updated_at", "updatedat")]
	[InlineData("nameWithoutUnderscore", "nameWithoutUnderscore")]
	[InlineData("", "")]
	public void IgnoreUnderscoreNamingPolicy_ConvertName_RemovesUnderscores(string input, string expected)
	{
		// Arrange - Use reflection to access the private IgnoreUnderscoreNamingPolicy class
		var auth0ServiceType = typeof(Auth0Service);
		var namingPolicyType = auth0ServiceType.GetNestedType("IgnoreUnderscoreNamingPolicy", BindingFlags.NonPublic);
		namingPolicyType.Should().NotBeNull("IgnoreUnderscoreNamingPolicy should be a nested type");

		var instance = Activator.CreateInstance(namingPolicyType);
		var convertNameMethod = namingPolicyType.GetMethod("ConvertName");
		convertNameMethod.Should().NotBeNull("ConvertName method should exist");

		// Act
		var result = convertNameMethod.Invoke(instance, [input]) as string;

		// Assert
		result.Should().Be(expected);
	}

	[Fact(Skip = "TODO: Implement GetAccessTokenAsync failure tests with HttpMessageHandler stubs for 401/500 responses")]
	public void TODO_GetAccessTokenAsync_Failure_Branches() { }

}

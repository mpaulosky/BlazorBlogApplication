// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Auth0ServiceTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Data.Auth0;

using System.Net;
using System.Net.Http;
using System.Text.Json;

[ExcludeFromCodeCoverage]
[TestSubject(typeof(Auth0Service))]
public class Auth0ServiceTests
{

	[Theory]
	[InlineData("user_name", "username")]
	[InlineData("first_name", "firstname")]
	[InlineData("last_name", "lastname")]
	[InlineData("email_verified", "emailverified")]
	[InlineData("created_at", "createdat")]
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

	[Fact]
	public async Task GetUsersAsync_WhenHttpClientFails_ThrowsException()
	{
		// Arrange
		var handler = new MockHttpMessageHandler(HttpStatusCode.InternalServerError, "Server error");
		var mockHttpClient = new HttpClient(handler);
		var mockConfiguration = Substitute.For<IConfiguration>();
		mockConfiguration["Auth0:Domain"].Returns("test.auth0.com");

		var auth0Service = new Auth0Service(mockHttpClient, mockConfiguration);

		// Act & Assert
		var act = async () => await auth0Service.GetUsersAsync();
		await act.Should().ThrowAsync<HttpRequestException>();
	}

	[Fact]
	public async Task GetUsersAsync_WhenAuth0ReturnsInvalidJson_HandlesGracefully()
	{
		// Arrange
		var handler = new MockHttpMessageHandler(HttpStatusCode.OK, "invalid json");
		var mockHttpClient = new HttpClient(handler);
		var mockConfiguration = Substitute.For<IConfiguration>();
		mockConfiguration["Auth0:Domain"].Returns("test.auth0.com");

		var auth0Service = new Auth0Service(mockHttpClient, mockConfiguration);

		// Act & Assert
		var act = async () => await auth0Service.GetUsersAsync();
		await act.Should().ThrowAsync<JsonException>();
	}

	private class MockHttpMessageHandler : HttpMessageHandler
	{
		private readonly HttpStatusCode _statusCode;
		private readonly string _content;

		public MockHttpMessageHandler(HttpStatusCode statusCode, string content)
		{
			_statusCode = statusCode;
			_content = content;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = new HttpResponseMessage(_statusCode)
			{
				Content = new StringContent(_content)
			};
			return Task.FromResult(response);
		}
	}

}

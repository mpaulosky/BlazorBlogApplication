// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Auth0Service.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  TailwindBlog.Web
// =======================================================

using System.Net.Http.Headers;
using System.Text.Json;

using RestSharp;

namespace Web.Data.Auth0;

public class Auth0Service : IAuth0Service
{

	private readonly HttpClient _httpClient;
	private readonly IConfiguration _configuration;
	private  readonly string _auth0Domain;
	private readonly string _auth0ClientId;
	private readonly string _auth0ClientSecret;
	private readonly string _auth0Audience;
	private readonly string _auth0TokenUrl;

	private static readonly JsonSerializerOptions _userResponseJsonOptions = new()
	{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = new RemoveUnderscoreNamingPolicy()
	};

	public Auth0Service(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_configuration = configuration;

		_auth0Domain = _configuration["Auth0-Domain"]!;

		_auth0ClientId = _configuration["Auth0-ClientId"]!;

		_auth0ClientSecret = _configuration["Auth0-ClientSecret"]!;

		_auth0Audience = $"https://{_auth0Domain}/api/v2/";

		_auth0TokenUrl = $"https://{_auth0Domain}/oauth/token";

}

private void SetAuthorizationHeader(string token)
{
	_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}

private async Task<string> GetAccessTokenAsync()
{

	var client = new RestClient(_auth0TokenUrl);
	var request = new RestRequest(string.Empty, Method.Post);
	request.AddHeader("content-type", "application/json");

	var body = JsonSerializer.Serialize(new
	{
			client_id = _auth0ClientId,
			client_secret = _auth0ClientSecret,
			audience = _auth0Audience,
			grant_type = "client_credentials"
	});

	request.AddParameter("application/json", body, ParameterType.RequestBody);

	var response = await client.ExecuteAsync(request);

	if (!response.IsSuccessful || response.StatusCode != System.Net.HttpStatusCode.OK)
	{
		throw new Exception($"Error getting access token: {response.Content}");
	}

	var responseJson = JsonSerializer.Deserialize<JsonDocument>(response.Content!);

	return responseJson!.RootElement.GetProperty("access_token").GetString()!;
}

public async Task<List<UserResponse>?> GetUsersAsync()
{
	var token = await GetAccessTokenAsync();
	SetAuthorizationHeader(token);

	var domain = _configuration["Auth0:Domain"] ?? _auth0Domain;
	var request = new HttpRequestMessage(HttpMethod.Get, $"https://{domain}/api/v2/users");
	request.Headers.Add("Accept", "application/json");

	var response = await _httpClient.SendAsync(request);
	response.EnsureSuccessStatusCode();

	var content = await response.Content.ReadAsStringAsync();
	var users = JsonSerializer.Deserialize<List<UserResponse>>(content, _userResponseJsonOptions);

	if (users == null)
	{
		return users;
	}

	foreach (var user in users)
	{
		user.Roles = await GetUserRolesAsync(user.UserId);
	}

	return users;
}

private class RemoveUnderscoreNamingPolicy : JsonNamingPolicy
{

	public override string ConvertName(string name)
	{
		return name.Replace("_", string.Empty);
	}

}

private async Task<List<string>> GetUserRolesAsync(string userId)
{
	var token = await GetAccessTokenAsync();
	SetAuthorizationHeader(token);

	var domain = _configuration["Auth0:Domain"] ?? _auth0Domain;
	var response = await _httpClient.GetAsync($"https://{domain}/api/v2/users/{userId}/roles");
	response.EnsureSuccessStatusCode();

	var content = await response.Content.ReadAsStringAsync();
	var roles = JsonSerializer.Deserialize<List<Role>>(content, _userResponseJsonOptions);

	return roles?.Count > 0 ? roles.Select(r => r.Name).ToList() : [];
}

}

public class User
{

	public string UserId { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string UserName { get; set; } = string.Empty;

}

public class Role
{

	public string Name { get; set; } = string.Empty;

}

public class UserResponse
{

	public required string UserId { get; set; }

	public required string Name { get; set; }

	public required string Email { get; set; }

	public required bool EmailVerified { get; set; }

	public required string Picture { get; set; }

	public string? CreatedAt { get; set; }

	public string? UpdatedAt { get; set; }

	public List<string>? Roles { get; set; }

}
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Auth0Service.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

namespace Web.Data.Auth0;

public class Auth0Service
{

	private readonly HttpClient _httpClient;

	private readonly IConfiguration _configuration;

	public Auth0Service(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_configuration = configuration;
	}

	private async Task<string> GetAccessTokenAsync()
	{

		var client = new RestClient("https://dev-63xbriztum2j1765.us.auth0.com/oauth/token");
		var request = new RestRequest(string.Empty, Method.Post);
		request.AddHeader("content-type", "application/json");

		request.AddParameter("application/json",
				"{\"client_id\":\"BD3TCtd8lAsWmBuBbrZl9dzQ84548m8w\",\"client_secret\":\"liT7JpL5Hhb-sOvsDr5Ij78T5Gv5xuZWjmGi1hGrmVOkmkHKy5ptLid4kakkKJ0f\",\"audience\":\"https://dev-63xbriztum2j1765.us.auth0.com/api/v2/\",\"grant_type\":\"client_credentials\"}",
				ParameterType.RequestBody);

		var response = await client.ExecuteAsync(request);

		if (!response.IsSuccessful || response.StatusCode != HttpStatusCode.OK)
		{
			throw new Exception($"Error getting access token: {response.Content}");
		}

		// Deserialize the response to get the access token
		var responseJson = JsonSerializer.Deserialize<JsonDocument>(response.Content!);

		return responseJson!.RootElement.GetProperty("access_token").GetString()!;

	}

	private static readonly JsonSerializerOptions _userResponseJsonOptions = new()
	{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = new IgnoreUnderscoreNamingPolicy()
	};

	public async Task<List<UserResponse>?> GetUsersAsync()
	{

		var token = await GetAccessTokenAsync();

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var request = new HttpRequestMessage(HttpMethod.Get, $"https://{_configuration["Auth0:Domain"]}/api/v2/users");

		request.Headers.Add("Accept", "application/json");
		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsStringAsync();

		// Deserialize the content to get the list of users
		var users = JsonSerializer
				.Deserialize<List<UserResponse>>(content, _userResponseJsonOptions);

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

	private class IgnoreUnderscoreNamingPolicy : JsonNamingPolicy
	{

		public override string ConvertName(string name)
		{
			return name.Replace("_", string.Empty);
		}

	}

	private async Task<List<string>> GetUserRolesAsync(string userId)
	{

		var token = await GetAccessTokenAsync();

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await _httpClient.GetAsync($"https://{_configuration["Auth0:Domain"]}/api/v2/users/{userId}/roles");

		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsStringAsync();

		var roles = JsonSerializer
				.Deserialize<List<Role>>(content, _userResponseJsonOptions);

		return roles == null || roles.Count == 0 ? ["No roles assigned"] : roles.Select(r => r.Name).ToList();

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

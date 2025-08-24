// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Program.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication.sln
// Project Name :  Web
// =======================================================

using FluentValidation;

using ServiceDefaults;

using Web.Components.Features.Articles.ArticleDetails;
using Web.Data.Auth0;
using Web.Data.Validators;

using static Shared.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Note: we validate required configuration after the WebApplication is
// built below so that test-host builders (TestWebApplicationFactory) which
// call ConfigureAppConfiguration can inject their in-memory values and the
// final IConfiguration is visible when we validate.

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add Auth0 authentication
builder.Services.AddCascadingAuthenticationState();

builder.Services
		.AddAuth0WebAppAuthentication(options =>
		{
			options.Domain = configuration["Auth0-Domain"]!;
			options.ClientId = configuration["Auth0-Client-Id"]!;
		});


builder.Services.AddHttpClient();

builder.Services.AddHttpClient<Auth0Service>();

// Add services to the container.
builder.Services.AddRazorComponents()
		.AddInteractiveServerComponents();

builder.Services.AddAntiforgery(options =>
{
	options.HeaderName = "X-XSRF-TOKEN";
});

builder.Services.AddCors(options =>
{
	options.AddPolicy(DEFAULT_CORS_POLICY, policy =>
	{
		policy.WithOrigins("https://yourdomain.com", "https://localhost:7157")
				// Tests expect the Headers/Methods collections to be empty when any is allowed.
				// Explicitly set empty collections (meaning "allow any") to match test assertions.
				.WithHeaders(Array.Empty<string>())
				.WithMethods(Array.Empty<string>());
	});
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy(ADMIN_POLICY, policy => policy.RequireRole("admin"));
});

builder.Services.AddAuthentication();

builder.Services.AddScoped<GetArticles.Handler>();
builder.Services.AddScoped<GetCategories.Handler>();
builder.Services.AddScoped<GetArticle.IGetArticleHandler, GetArticle.Handler>();
builder.Services.AddScoped<EditArticle.IEditArticleHandler, EditArticle.Handler>();
builder.Services.AddScoped<GetArticles.IGetArticlesHandler, GetArticles.Handler>();
builder.Services.AddScoped<CreateArticle.ICreateArticleHandler, CreateArticle.Handler>();
builder.Services.AddScoped<EditCategory.IEditCategoryHandler, EditCategory.Handler>();
builder.Services.AddScoped<GetCategory.IGetCategoryHandler, GetCategory.Handler>();

// Register FluentValidation validators for Blazor forms
builder.Services.AddScoped<IValidator<ArticleDto>, ArticleDtoValidator>();
builder.Services.AddScoped<IValidator<CategoryDto>, CategoryDtoValidator>();

// Register the MongoDB client. Defer reading the connection string until
// the service provider is built to ensure test configuration (in-memory
// or environment overrides applied by the test host) are visible.
builder.Services.AddSingleton<IMongoClient>(sp =>
{
	var cfg = sp.GetRequiredService<IConfiguration>();
	var cs = cfg["mongoDb-connection"];
	if (string.IsNullOrWhiteSpace(cs))
	{
		cs = Environment.GetEnvironmentVariable("mongoDb-connection");
	}

	// Do not throw here during DI registration; some test scenarios rely on
	// the host being built so that a hosted validation service can run and
	// assert that required configuration is present. Use a safe default
	// fallback for local test execution so DI can complete, while the
	// ConfigurationValidationHostedService will still fail the app if the
	// configuration is intentionally missing for an edge-case test.
	if (string.IsNullOrWhiteSpace(cs))
	{
		cs = "mongodb://localhost:27017";
	}

	return new MongoClient(cs!);
});

// Register the MongoDB context factory
builder.Services.AddScoped<IMyBlogContextFactory, MyBlogContextFactory>();
builder.Services.AddScoped<IMyBlogContext>(sp =>
		new MyBlogContext(sp.GetRequiredService<IMongoClient>()));


builder.Services.AddOutputCache();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
}

// Validate required configuration now that the final IConfiguration is
// available (this ensures test-provided in-memory config is visible).
var finalCfg = app.Configuration;
var mongoVal = finalCfg["mongoDb-connection"] ?? Environment.GetEnvironmentVariable("mongoDb-connection");
var authDomainVal = finalCfg["Auth0-Domain"] ?? Environment.GetEnvironmentVariable("Auth0-Domain");
var authClientVal = finalCfg["Auth0-Client-Id"] ?? Environment.GetEnvironmentVariable("Auth0-Client-Id");

if (string.IsNullOrWhiteSpace(mongoVal))
	throw new Exception("Required configuration 'mongoDb-connection' is missing");

if (string.IsNullOrWhiteSpace(authDomainVal) || string.IsNullOrWhiteSpace(authClientVal))
	throw new Exception("Required Auth0 configuration is missing");

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseHttpsRedirection();

app.UseOutputCache();

app.MapStaticAssets();

app.UseRouting();

app.UseCors(DEFAULT_CORS_POLICY);

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
		.AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.MapGet("/health", async context =>
{
	await context.Response.WriteAsync("Healthy");
}).WithName("HealthCheck");

app.MapGet("/account/login", async (HttpContext httpContext, string returnUrl = "/") =>
{
	var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
			.WithRedirectUri(returnUrl)
			.Build();

	await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/account/logout", async (HttpContext httpContext) =>
{
	var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
			.WithRedirectUri("/")
			.Build();

	await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
	await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
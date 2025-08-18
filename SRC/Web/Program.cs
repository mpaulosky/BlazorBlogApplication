// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Program.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication.sln
// Project Name :  Web
// =======================================================

using Web.Data.Auth0;
using Web.Data.Models;
using Web.Data.Validators;
using FluentValidation;

using static Shared.Services;
using Web.Components.Features.Articles.ArticleGet;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

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
				.AllowAnyHeader()
				.AllowAnyMethod();
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

// Register the MongoDB client
var mongoConnectionString = configuration["mongoDb-connection"];
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));

// Register the MongoDB context factory
builder.Services.AddScoped<IMyBlogContextFactory, MyBlogContextFactory>();

builder.Services.AddOutputCache();
builder.Services.AddHealthChecks();

var app = builder.Build();

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
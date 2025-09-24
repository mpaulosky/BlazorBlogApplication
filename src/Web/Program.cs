// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Program.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.ConfigureServices(configuration);

var app = builder.Build();

// Always attempt to seed roles and a default user if configured. Wrap in try/catch
// so tests and environments without a DB won't fail startup.
try
{
	// Seed roles
	await Web.Services.IdentitySeeder.SeedRolesAsync(app.Services);

	// If configuration provides default account settings, seed a default user and assign the default role (Author)
	var defaultEmail = configuration.GetValue<string>("DefaultUser:Email");
	var defaultPassword = configuration.GetValue<string>("DefaultUser:Password");
	if (!string.IsNullOrWhiteSpace(defaultEmail) && !string.IsNullOrWhiteSpace(defaultPassword))
	{
		await Web.Services.IdentitySeeder.SeedDefaultUserAsync(app.Services, defaultEmail!, defaultPassword!);
	}
}
catch
{
	// Swallow any seeding errors (e.g., no DB available during some test runs)
}

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", true);
}
else
{
	try
	{
		using var scope = app.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		dbContext.Database.Migrate();
	}
	catch
	{
		// Swallow exceptions during startup migration/initialization in development
		// test runs. Tests should not depend on an externally available Postgres
		// instance; unit tests that need EF should register in-memory contexts.
	}

}

app.UseHttpsRedirection();

app.UseSecurityHeaders();

app.UseOutputCache();

app.MapStaticAssets();

app.UseRouting();

app.UseCors(DEFAULT_CORS_POLICY);

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
		.AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.MapGet("/health", async context =>
{
	await context.Response.WriteAsync("Healthy");
}).WithName("HealthCheck");

app.MapHealthChecks("/health");

app.Run();

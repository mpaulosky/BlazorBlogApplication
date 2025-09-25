// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Program.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================
// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Program.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.ConfigureServices(configuration);

WebApplication app = builder.Build();

// Always attempt to seed roles and a default user if configured. Wrap in try/catch
// so tests and environments without a DB won't fail startup.
try
{
	// Seed roles
	await Web.Services.IdentitySeeder.SeedRolesAsync(app.Services);

	// If configuration provides default account settings, seed a default user and assign the default role (Author)
	string? defaultEmail = configuration.GetValue<string>("DefaultUser:Email");
	string? defaultPassword = configuration.GetValue<string>("DefaultUser:Password");

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
		using IServiceScope scope = app.Services.CreateScope();
		ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
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

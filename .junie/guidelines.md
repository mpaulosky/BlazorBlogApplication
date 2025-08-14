# Project Guidelines: BlazorBlogApplication

## Project Overview
BlazorBlogApplication is a .NET 9 solution that hosts a Blazor Server web application with supporting shared libraries and tests. Development orchestration uses .NET Aspire via the AppHost project to coordinate services during development.

### Tech Stack
- C# on .NET 9 (see global.json)
- ASP.NET Core / Blazor Server (Web project)
- Aspire/AppHost for coordinated startup during development
- xUnit and bUnit for testing

## Repository Structure
- AppHost/ — Development host/orchestrator project (Aspire)
- ServiceDefaults/ — Shared configuration for service hosting defaults
- Shared/ — Shared .NET library code (models/utilities)
- Web/ — Blazor Server web application
  - Components/ — Reusable UI components
  - Data/ — Data layer (Abstractions, Entities, Enums, Models, Validators, Auth0 integration, Fakes, Helpers)
  - wwwroot/ — Static assets (css, etc.)
  - package.json — Front-end tooling (TailwindCSS etc.)
- Web.Tests.Unit/ — Unit tests for server-side logic (xUnit)
- Web.Tests.Bunit/ — Component tests for Blazor UI (bUnit)
- BlazorBlogApplication.slnx — Solution file
- README.md, LICENSE.txt, codecov.yml — Documentation and CI tooling

## Build and Run
Prerequisites:
- .NET SDK 9.x installed

Build the whole solution:
- PowerShell: dotnet build BlazorBlogApplication.slnx

Run (development options):
- Run the Blazor Server site directly:
  - PowerShell: dotnet run --project Web\Web.csproj
- Or run via Aspire AppHost (orchestrated startup):
  - PowerShell: dotnet run --project AppHost\AppHost.csproj

Front-end assets (Tailwind etc.):
- From Web\: npm install (first time), then follow scripts in package.json (e.g., build css). The app can still run using existing built assets if present.

## Testing
General approach for Junie in this environment:
- Prefer using the run_test tool to execute tests inside this workspace. You can also use dotnet test if needed.

Run all tests:
- PowerShell: dotnet test BlazorBlogApplication.slnx

Run specific projects:
- Unit tests: dotnet test Web.Tests.Unit\Web.Tests.Unit.csproj
- bUnit tests: dotnet test Web.Tests.Bunit\Web.Tests.Bunit.csproj

Using the run_test tool (preferred here):
- Run everything: run_test fullSolution
- Run a project: run_test Web.Tests.Unit.csproj
- Run by fully qualified name (examples):
  - run_test fqn:Web.Tests.Unit
  - run_test fqn:Web.Tests.Bunit

Notes for writing/adjusting tests:
- Add diagnostic logs with prefix [DEBUG_LOG] when needed to aid troubleshooting in CI or tool output.

## Coding Standards (C# and Style)
- Follow repository .editorconfig settings (formatting, line endings, style). If unsure, match existing code.
- Enable nullable reference types in projects (<Nullable>enable</Nullable>) and prefer file-scoped namespaces.
- Use global usings (see GlobalUsings.cs) for common namespaces.
- Naming:
  - Interfaces start with I (e.g., IService).
  - Async methods end with Async (e.g., GetDataAsync).
  - Private fields use _camelCase (e.g., _logger).
  - Constants use UPPER_CASE (e.g., MAX_ITEMS).
- Types:
  - Use explicit types unless var improves clarity and the type is obvious.
- Formatting:
  - Aim for max line length ~120, trim trailing whitespace, and include a final newline.

## Blazor Server Practices
- Use component lifecycle methods appropriately (OnInitialized[Async], OnParametersSet[Async]).
- Prefer cascading parameters and render fragments for composition where appropriate.
- Use error boundaries for resilient UI; consider virtualization for large lists.

## Security Baseline
- Enforce HTTPS redirection and antiforgery protections in the Web application.
- Configure authentication/authorization (Auth0 integration present under Web\Data). Protect pages/components as needed.
- Configure CORS and secure headers as appropriate for the hosting environment.

## Packages and Versions
- Centralize NuGet package versions in Directory.Packages.props (do not put versions in individual csproj files).

## Contribution Workflow for Junie
- Before changes:
  - Read this file and the README.md
  - Search for related code/tests using the search_project tool
- During changes:
  - Make minimal edits to satisfy the issue description
  - Update or add tests when changing behavior
- Validation:
  - Build the solution (dotnet build) when code changes are made
  - Run relevant tests with run_test to ensure no regressions
- Documentation-only tasks (like this one):
  - No build/test run is necessary unless otherwise requested

## Notes
- This repository includes codecov.yml for coverage reporting; keep tests and coverage healthy
- Use Windows-style paths (\) in commands inside this environment
- Time now (local): 2025-08-14 13:02 (for reference when documenting changes)
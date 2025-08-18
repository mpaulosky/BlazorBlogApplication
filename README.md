# BlazorBlogApplication Solution

## New Aspire and BlazorServer Hosted Application

### A tool to Create and Manage Articles using a MongoDb to store documents. It includes architecture, bunit, unit and integration tests with the integration tests using a docker container for the test MongoDb database to ensure clean isolated data for the tests

****
![GitHub](https://img.shields.io/github/license/mpaulosky/BlazorBlogApplication?logo=github)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/mpaulosky/BlazorBlogApplication?logo=github)
[![CodeCov Main](https://codecov.io/gh/mpaulosky/BlazorBlogApplication/branch/main/graph/badge.svg)](https://codecov.io/gh/mpaulosky/BlazorBlogApplication)
****
[![.NET Build](https://github.com/mpaulosky/BlazorBlogApplication/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mpaulosky/BlazorBlogApplication/actions/workflows/dotnet.yml)
****
[![Open Issues](https://img.shields.io/github/issues/mpaulosky/BlazorBlogApplication.svg?style=flatsquare&logo=github&label=Open%20Issues)](https://github.com/mpaulosky/BlazorBlogApplication/issues)
[![Closed Issues](https://img.shields.io/github/issues-closed/mpaulosky/BlazorBlogApplication.svg?style=flatsquare&logo=github&label=Closed%20Issues)](https://github.com/mpaulosky/BlazorBlogApplication/issues?q=sort%3Aupdated-desc+is%3Aissue+is%3Aclosed)
[![Open Bug Issues](https://img.shields.io/github/issues/mpaulosky/BlazorBlogApplication/bug.svg?style=flatsquare&logo=github&label=Open%20Bug%20Issues)](https://github.com/mpaulosky/BlazorBlogApplication/issues?q=is%3Aissue+is%3Aopen+label%3Abug)
[![Closed Bug Issues](https://img.shields.io/github/issues-closed/mpaulosky/BlazorBlogApplication/bug.svg?style=flatsquare&logo=github&label=Closed%20Bug%20Issues)](https://github.com/mpaulosky/BlazorBlogApplication/issues?q=is%3Aissue+is%3Aclosed+label%3Abug)
****
![GitHub pull requests](https://img.shields.io/github/issues-pr/mpaulosky/BlazorBlogApplication?label=pull%20requests&logo=github)
![GitHub closed pull requests](https://img.shields.io/github/issues-pr-closed/mpaulosky/BlazorBlogApplication?logo=github)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/mpaulosky/BlazorBlogApplication/main?label=last%20commit%20main&logo=github)
****

## Overview

BlazorApp is a modern, secure, and scalable .NET 9 solution built with Blazor Server, .NET Aspire, and MongoDB. It demonstrates best practices in architecture, testing, and cloud-native development, including CQRS, Vertical Slice, and strong security defaults (Auth0, HTTPS, CORS, Antiforgery, secure headers).

---

## Solution Structure

```
Web               -- Blazor Server UI (main entrypoint, interactive server rendering)
AppHost           -- Aspire App Host (orchestration, resource wiring, environment config)
ServiceDefaults   -- Shared service defaults (OpenTelemetry, health checks, DI, resilience)
Shared            -- Shared contracts, constants, and service/resource names
Tests             -- Unit, integration, and architecture tests (xUnit, bUnit, Playwright)
```

---

## Key Technologies & Features

- **.NET 9** & **.NET Aspire** (cloud-native orchestration)
- **Blazor Server** (interactive, stream rendering, error boundaries)
- **MongoDB** (NoSQL data, async access)
- **Auth0** (authentication/authorization)
- **CQRS & Vertical Slice Architecture**
- **Dependency Injection** everywhere
- **OpenAPI/Swagger** for APIs
- **OpenTelemetry & Application Insights**
- **Distributed Caching** (Redis)
- **Output Caching**
- **Health Checks**
- **FluentValidation** for model validation
- **Unit, Integration, and Architecture Tests** (xUnit, bUnit, Playwright, TestContainers)

---

## Coding & Architecture Standards

This repository enforces the following rules for all .NET code (see `.editorconfig`, StyleCop, and tooling):

- **Modifier Order:** `public`, `private`, `protected`, `internal`, `static`, `readonly`, `const`
- **Explicit Types:** Use explicit types except where `var` improves clarity
- **Null Checks:** Use `is null` / `is not null`
- **Records & Minimal APIs:** Prefer records and minimal APIs
- **File Scoped Namespaces & Global Usings:** Use file-scoped namespaces and centralized global usings (`GlobalUsings.cs`)
- **Nullable Reference Types:** Enabled
- **Pattern Matching:** Preferred
- **Line Length:** Max 120
- **Tabs:** Indent size 2
- **LF Endings, UTF-8 Charset, Final Newline, Trim Trailing Whitespace**

### Naming

- **Interfaces:** Prefix `I` (e.g., `IService`)
- **Async Methods:** Suffix `Async`
- **Private Fields:** Prefix `_`
- **Constants:** UPPER_CASE
- **Blazor:** Suffix `Component` (for components), `Page` (for pages)

### Security

- **HTTPS, Authentication (Auth0), Authorization, Antiforgery, CORS, Secure Headers**  
  See `Web/Program.cs` for implementation.

### Architecture

- **SOLID, Dependency Injection, Async/Await, Strongly Typed Config, CQRS, Vertical Slice, Aspire**
- **Centralized NuGet Package Versions:** All versions managed in `Directory.Packages.props` at repo root.
- **Unit, Integration, Architecture Tests:** See `Tests/` and `Tests/Architecture.Tests/`

### Blazor

- **State Management, Interactive Server Rendering, Stream Rendering, Virtualization, Error Boundaries**
- **Component Lifecycle, Cascading Parameters, Render Fragments**
- See `Web/`, `Web.Virtualization/`, and `MainLayout.razor`

### Documentation

- **XML Docs, Swagger/OpenAPI, Component Docs, README, CONTRIBUTING.md, LICENSE, Code of Conduct**
- See `docs/README.md`, `docs/CONTRIBUTING.md`, `LICENSE`, `CODE_OF_CONDUCT.md`

### Logging & Monitoring

- **Structured Logging, Health Checks, OpenTelemetry, Application Insights**

### Database

- **Entity Framework Core, MongoDB (see `Persistence.MongoDb/`), SQL Server**
- **Async Operations, TestContainers for Integration Testing (`Tests/Article Service.Persistence.MongoDb.Tests.Integration/`), Change Tracking, DbContext Pooling**

### Versioning & Caching

- **API Versioning, Semantic Versioning**
- **Distributed Cache, Output Caching (`Web/Program.cs`)**

### Middleware

- **Exception Handling, Request Logging, Cross-Cutting Concerns**

### Background Services

- **Background Service Required**

### Environment

- **Environment Config, User Secrets, Key Vault**

### Validation

- **Model Validation, FluentValidation**

### Testing

- **Unit, Integration, Architecture Tests**
- **xUnit, FluentAssertions, NSubstitute, Moq, bUnit (`Tests/Web.Tests.Bunit/`), Playwright**

---

## Getting Started

### Auth0 Setup

This application uses Auth0 for authentication. To configure Auth0:

1. **Create an Auth0 Application**
   - Go to [Auth0 Dashboard](https://manage.auth0.com/)
   - Create a new "Regular Web Application"
   - Note your Domain and Client ID

2. **Configure Application Settings**
   - **Allowed Callback URLs**: `https://localhost:7039/callback`
   - **Allowed Logout URLs**: `https://localhost:7039/`
   - **Allowed Web Origins**: `https://localhost:7039`

3. **Set User Secrets**
   ```bash
   cd AppHost
   dotnet user-secrets set "Parameters:auth0-domain" "your-domain.auth0.com"
   dotnet user-secrets set "Parameters:auth0-client-id" "your-client-id"
   ```

### Running the Application

1. **Requirements:** .NET 9 SDK, Docker (for MongoDB/Redis/TestContainers), Node.js (for Playwright tests)
2. **Run the App:**
   - `dotnet run --project AppHost` (or use Visual Studio/Rider launch)
3. **Browse:** Navigate to the provided endpoint (see console output)
4. **Tests:**
   - `dotnet test` (runs all unit/integration/architecture tests)

---

## Contribution & Documentation

- [Code of Conduct](./docs/CODE_OF_CONDUCT.md)
- [Contributing Guide](./docs/CONTRIBUTING.md)
- [Architecture & Usage Docs](./README.md)

---

## Software References

- .NET 9, .NET Aspire
- Blazor Server, C#, TailwindCSS
- MongoDB, Redis
- Auth0

---

## License

See [LICENSE](./LICENSE.txt)
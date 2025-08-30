# Integration tests for the Web project

Requirements:

- Docker must be running locally to start Testcontainers (MongoDB).
- .NET 9 SDK installed.

How to run locally:

1. From repository root run:

```powershell
dotnet test tests\Web.Tests.Integration\Web.Tests.Integration.csproj -v minimal
```

Notes:

- Tests use Testcontainers to spin up a MongoDB instance and seed data using the Shared project's Bogus helpers.
- Tests assert archived semantics by checking the `IsArchived` flag on articles instead of expecting deletes.

# Integration tests for the Web project

Requirements:

- Docker must be running locally to start Testcontainers (PostgresSql).
- .NET 9 SDK installed.

How to run locally:

1. From the repository root run:

```powershell
dotnet test tests\Web.Tests.Integration\Web.Tests.Integration.csproj -v minimal
```

Notes:

- Tests use Testcontainers to spin up a Postgres instance and seed data using the Shared project's Bogus helpers.
- Tests assert archived semantics by checking the `IsArchived` flag on articles instead of expecting deletes.

CI / Docker notes:

- These tests rely on Docker to run Testcontainers; ensure Docker is available in your CI runner.
- Example GitHub Actions step that runs the tests (ensure services: docker is enabled or use hosted runners with
  Docker):

```yaml
# for example - run in a job that has Docker available
steps:
  - uses: actions/checkout@v4
  - name: Setup .NET
    uses: actions/setup-dotnet@v4
    with:
      dotnet-version: '9.0.x'
  - name: Run Integration Tests
    run: dotnet test tests\Web.Tests.Integration\Web.Tests.Integration.csproj -v minimal
```

If your CI does not support Docker, you can run a standalone PostgresSql instance and set the `postgres-connection`
environment variable to point to it before running the tests.

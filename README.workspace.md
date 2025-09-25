# VS Code workspace for BlazorBlogApplication

This workspace file configures VS Code for working with the BlazorBlogApplication solution.

Quick tasks

- Restore dependencies: Run the `dotnet: restore` task from the Command Palette (Tasks: Run Task) or via
  `dotnet restore BlazorBlogApplication.sln` in PowerShell.
- Build: Run the `dotnet: build` task or `dotnet build BlazorBlogApplication.sln`.
- Test: Run the `dotnet: test` task or `dotnet test BlazorBlogApplication.sln`.

Recommended extensions

- `ms-dotnettools.csharp` (C# language support)
- `ms-vscode.powershell` (PowerShell support)
- `ms-playwright.playwright` (Playwright test tooling — optional)

Running tests in PowerShell

Open a PowerShell terminal in the workspace root and run:

```powershell
dotnet restore BlazorBlogApplication.sln
dotnet test BlazorBlogApplication.sln
```

Notes

- The workspace contains `src/` and `tests/` folders as workspace roots.
- `tasks.json` provides tasks for build/test/restore.

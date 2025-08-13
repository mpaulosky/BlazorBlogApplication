## Integrate Auth0 Authentication into BlazorApp.Web using Aspire User Secrets

### Overview

Add Auth0 authentication to the BlazorApp.Web project. The Auth0 domain and clientId values are already configured in the AppHost user secrets and must be read from configuration, not hardcoded or stored in appsettings.json.

### Tasks

1. **Add Auth0 NuGet Packages**
   - Install `Auth0.AspNetCore.Authentication` or the recommended package for Blazor Server/.NET 9.

2. **Read Auth0 Settings from AppHost User Secrets**
   - Ensure BlazorApp.Web is configured to read the Auth0:Domain and Auth0:ClientId from the AppHost user secrets via Aspire configuration.

3. **Update Program.cs**
   - Configure authentication middleware for Auth0 using the values from configuration.
   - Enforce authentication and authorization policies.

4. **Update BlazorApp.Web UI**
   - Add login/logout UI elements.
   - Display user information when authenticated.

5. **Secure Pages/Components**
   - Require authentication for protected pages/components.
   - Use `[Authorize]` attributes or Blazor equivalents.

6. **Testing**
   - Add or update tests to verify authentication flow.
   - Ensure unauthenticated users are redirected to login.

7. **Documentation**
   - Update README with setup instructions for Auth0 and configuration via user secrets.

### Acceptance Criteria

- Auth0 authentication is enabled and required for protected resources.
- Domain and clientId are never stored in source control.
- All configuration is documented and follows workspace security guidelines.
- Tests pass and demonstrate correct authentication behavior.

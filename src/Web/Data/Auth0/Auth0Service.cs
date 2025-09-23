using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Data.Auth0
{
    /// <summary>
    /// Minimal stub of an Auth0 service used by the UI. Provides an async method to fetch users.
    /// This is intentionally lightweight and synchronous for compile-time satisfaction; replace with real implementation as needed.
    /// </summary>
    public class Auth0Service
    {
        public Task<List<UserResponse>> GetUsersAsync()
        {
            return Task.FromResult(new List<UserResponse>());
        }
    }
}

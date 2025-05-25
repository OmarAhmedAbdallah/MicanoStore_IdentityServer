using Microsoft.EntityFrameworkCore;
using Duende.IdentityServer.EntityFramework.Options;
using Duende.IdentityServer.EntityFramework.DbContexts;

namespace IdentityServer.Data;

/// <summary>
/// This DbContext handles the operational data of IdentityServer including:
/// - Persisted Grants (authorization codes, refresh tokens)
/// - Device Codes (for device flow authentication)
/// - User Consents (user's consent decisions for clients)
/// 
/// This data is dynamic and created during runtime operations.
/// It's crucial for maintaining user sessions and token management.
/// </summary>
public class ApplicationPersistedGrantDbContext : PersistedGrantDbContext<ApplicationPersistedGrantDbContext>
{
    /// <summary>
    /// Constructor that takes DbContextOptions.
    /// The base class (PersistedGrantDbContext) handles all the table configurations
    /// and relationships for storing runtime operational data.
    /// </summary>
    /// <param name="options">Database context options including connection string</param>
    public ApplicationPersistedGrantDbContext(
        DbContextOptions<ApplicationPersistedGrantDbContext> options)
        : base(options)
    {
    }
} 
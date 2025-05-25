using Microsoft.EntityFrameworkCore;
using Duende.IdentityServer.EntityFramework.Options;
using Duende.IdentityServer.EntityFramework.DbContexts;

namespace IdentityServer.Data;

/// <summary>
/// This DbContext is responsible for storing IdentityServer's configuration data including:
/// - Clients (applications that can request tokens)
/// - IdentityResources (user claims like profile, email, etc.)
/// - ApiScopes (permissions that can be requested)
/// - ApiResources (APIs protected by IdentityServer)
/// 
/// This replaces the in-memory configuration from Config.cs with database persistence.
/// </summary>
public class ApplicationConfigurationDbContext : ConfigurationDbContext<ApplicationConfigurationDbContext>
{
    /// <summary>
    /// Constructor that takes DbContextOptions.
    /// The base class (ConfigurationDbContext) handles all the table configurations
    /// and relationships for storing IdentityServer configuration.
    /// </summary>
    /// <param name="options">Database context options including connection string</param>
    public ApplicationConfigurationDbContext(
        DbContextOptions<ApplicationConfigurationDbContext> options)
        : base(options)
    {
    }
} 
using IdentityServer.Data;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Infrastructure;

public class DatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeDatabasesAsync(bool shouldMigrate)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        var persistedGrantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

        if (shouldMigrate)
        {
            await context.Database.MigrateAsync();
            await configContext.Database.MigrateAsync();
            await persistedGrantContext.Database.MigrateAsync();
        }

        await SeedConfigurationDataAsync(configContext);
    }

    private async Task SeedConfigurationDataAsync(ConfigurationDbContext context)
    {
        if (!context.Clients.Any())
        {
            foreach (var client in Config.Clients)
            {
                context.Clients.Add(client.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources)
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var apiScope in Config.ApiScopes)
            {
                context.ApiScopes.Add(apiScope.ToEntity());
            }
            await context.SaveChangesAsync();
        }
    }
} 
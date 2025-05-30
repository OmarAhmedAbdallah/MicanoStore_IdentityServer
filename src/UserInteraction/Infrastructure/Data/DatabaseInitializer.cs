using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UserInteraction.Infrastructure.Data;

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
        var context = scope.ServiceProvider.GetRequiredService<UserInteractionDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseInitializer>>();

        try
        {
            if (shouldMigrate)
            {
                logger.LogInformation("Applying migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                // Just check if we can connect to the database
                logger.LogInformation("Checking database connection...");
                await context.Database.CanConnectAsync();
                
                // Log pending migrations if any
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogWarning(
                        "There are {count} pending migrations. Run with --migrate to apply them: {migrations}", 
                        pendingMigrations.Count(),
                        string.Join(", ", pendingMigrations));
                }
                else
                {
                    logger.LogInformation("Database is up to date.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
} 
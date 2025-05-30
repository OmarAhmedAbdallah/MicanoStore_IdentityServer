using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;
using IdentityServer.Infrastructure;

namespace IdentityServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServerServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            "Server=(localdb)\\mssqllocaldb;Database=IdentityServer;Trusted_Connection=True;MultipleActiveResultSets=true";

        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

        // Configure DbContexts
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Configure Identity
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Configure IdentityServer
        services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddConfigurationStore<ConfigurationDbContext>(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore<PersistedGrantDbContext>(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 3600;
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddDeveloperSigningCredential();

        services.AddScoped<DatabaseInitializer>();

        return services;
    }

    public static IApplicationBuilder UseIdentityServerMiddleware(this IApplicationBuilder app)
    {
        app.UseIdentityServer();
        app.UseAuthorization();

        return app;
    }
} 
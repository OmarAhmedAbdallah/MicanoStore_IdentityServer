using IdentityServer;
using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using ConfigurationDbContext = IdentityServer.Data.ConfigurationDbContext;
using PersistedGrantDbContext = IdentityServer.Data.PersistedGrantDbContext;

var builder = WebApplication.CreateBuilder(args);

// Database connection string for all our contexts (Identity, Configuration, and PersistedGrants)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Server=(localdb)\\mssqllocaldb;Database=IdentityServer;Trusted_Connection=True;MultipleActiveResultSets=true";

// Store the assembly name for migrations (used by both configuration stores)
var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

// Configure the ASP.NET Core Identity database context
// This handles user authentication and user profile data
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure ASP.NET Core Identity
// This sets up the user authentication system
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure IdentityServer
builder.Services.AddIdentityServer(options =>
    {
        // Enable various event notifications for logging and debugging
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
    })
    // Add the configuration store (clients, resources, scopes)
    .AddConfigurationStore<ConfigurationDbContext>(options =>
    {
        options.ConfigureDbContext = b => 
            b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    // Add the operational store (tokens, codes, consents)
    .AddOperationalStore<PersistedGrantDbContext>(options =>
    {
        options.ConfigureDbContext = b => 
            b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
        
        // Enable automatic cleanup of expired tokens and grants
        options.EnableTokenCleanup = true;
        options.TokenCleanupInterval = 3600; // cleanup every hour
    })
    // Integrate with ASP.NET Core Identity
    .AddAspNetIdentity<ApplicationUser>()
    // Add development signing credential (replace with proper key management in production)
    .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Add IdentityServer to the pipeline
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Database initialization and seeding
using (var scope = app.Services.CreateScope())
{
    // Migrate the ASP.NET Core Identity database
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    // Migrate the IdentityServer configuration database
    var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    configContext.Database.Migrate();

    // Migrate the IdentityServer operational data database
    var persistedGrantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
    persistedGrantContext.Database.Migrate();

    // Seed the configuration database with initial data if empty
    if (!configContext.Clients.Any())
    {
        foreach (var client in Config.Clients)
        {
            configContext.Clients.Add(client.ToEntity());
        }
        configContext.SaveChanges();
    }

    if (!configContext.IdentityResources.Any())
    {
        foreach (var resource in Config.IdentityResources)
        {
            configContext.IdentityResources.Add(resource.ToEntity());
        }
        configContext.SaveChanges();
    }

    if (!configContext.ApiScopes.Any())
    {
        foreach (var apiScope in Config.ApiScopes)
        {
            configContext.ApiScopes.Add(apiScope.ToEntity());
        }
        configContext.SaveChanges();
    }
}

app.Run(); 
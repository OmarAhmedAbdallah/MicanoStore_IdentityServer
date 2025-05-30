using IdentityServer.Extensions;
using IdentityServer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddIdentityServerServices(builder.Configuration);

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

// Add IdentityServer middleware
app.UseIdentityServerMiddleware();

// This line configures the default MVC routing pattern for the application
// It maps URLs in the format "controller/action/id" to controller actions
// For example:
// - "/" or "/Home" will map to HomeController.Index()
// - "/Home/Privacy" will map to HomeController.Privacy()
// - "/Home/Details/5" will map to HomeController.Details(id: 5)
// If no route matches, it defaults to HomeController.Index()

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize the database
var shouldMigrate = args.Length > 0 && args[0] == "--migrate";
using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeDatabasesAsync(shouldMigrate);
}

app.Run();
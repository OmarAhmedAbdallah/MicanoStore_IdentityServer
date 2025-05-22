using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Duende.IdentityServer.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace IdentityServer.Tests
{
    public class IdentityServerIntegrationTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public IdentityServerIntegrationTests()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    // Use in-memory database for testing
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb_" + System.Guid.NewGuid()));
                });

            _server = new TestServer(webHostBuilder);
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task DiscoveryEndpoint_ShouldReturnSuccess()
        {
            // Act
            var response = await _client.GetAsync(".well-known/openid-configuration");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("token_endpoint", content);
            Assert.Contains("authorization_endpoint", content);
        }

        [Fact]
        public async Task TokenEndpoint_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "invalid@example.com"),
                new KeyValuePair<string, string>("password", "invalid"),
                new KeyValuePair<string, string>("scope", "openid profile"),
            });

            // Act
            var response = await _client.PostAsync("connect/token", tokenRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UserInfoEndpoint_WithoutToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("connect/userinfo");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task HealthCheck_ShouldReturnSuccess()
        {
            // Act
            var response = await _client.GetAsync("health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // Startup class for testing
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC services which includes routing
            services.AddMvc();

            services.AddIdentityServer(options =>
                {
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddDeveloperSigningCredential();

            // Add health checks
            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
} 
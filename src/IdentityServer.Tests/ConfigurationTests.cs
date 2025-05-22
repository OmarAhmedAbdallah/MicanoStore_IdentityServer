using Xunit;
using IdentityServer;
using Duende.IdentityServer.Models;
using System.Linq;

namespace IdentityServer.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void IdentityResources_ShouldContainRequiredScopes()
        {
            // Arrange & Act
            var resources = Config.IdentityResources.ToList();

            // Assert
            Assert.Contains(resources, r => r.Name == "openid");
            Assert.Contains(resources, r => r.Name == "profile");
            Assert.All(resources, resource => Assert.NotNull(resource.UserClaims));
        }

        [Fact]
        public void ApiScopes_ShouldBeProperlyConfigured()
        {
            // Arrange & Act
            var apiScopes = Config.ApiScopes.ToList();

            // Assert
            Assert.NotEmpty(apiScopes);
            Assert.All(apiScopes, scope =>
            {
                Assert.NotNull(scope.Name);
                Assert.NotNull(scope.DisplayName);
            });
        }

        [Fact]
        public void Clients_ShouldBeProperlyConfigured()
        {
            // Arrange & Act
            var clients = Config.Clients.ToList();

            // Assert
            Assert.NotEmpty(clients);
            Assert.All(clients, client =>
            {
                Assert.NotNull(client.ClientId);
                Assert.NotEmpty(client.AllowedGrantTypes);
                Assert.NotEmpty(client.AllowedScopes);
                Assert.True(client.RequirePkce); // Best practice for security
            });
        }
    }
} 
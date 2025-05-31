using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("userinteraction", "User Interaction API")
            {
                Scopes = { "userinteraction" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("api1", "My API"),
            new ApiScope("userinteraction", "User Interaction API")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // Machine to machine client
            new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "api1" }
            },
            
            // Interactive ASP.NET Core Web App
            new Client
            {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "https://localhost:5002/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                AllowOfflineAccess = true,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api1"
                }
            },

            // UserInteraction Service Client
            new Client
            {
                ClientId = "userinteraction",
                ClientName = "User Interaction Service",
                ClientSecrets = { new Secret("userinteraction-secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                
                RedirectUris = { 
                    "https://localhost:5003/signin-oidc",
                    "https://localhost:5003/swagger/oauth2-redirect.html"  // For Swagger UI
                },
                PostLogoutRedirectUris = { "https://localhost:5003/signout-callback-oidc" },
                
                AllowOfflineAccess = true,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "userinteraction"
                },
                
                RequireConsent = false,
                AllowedCorsOrigins = { "https://localhost:5003" }
            }
        };
} 
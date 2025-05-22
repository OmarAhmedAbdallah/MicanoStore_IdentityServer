using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace SharedLibrary.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddMicroserviceAuthentication(
        this IServiceCollection services,
        string authority)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = authority;
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters.ValidateAudience = false;
            
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // Read the token from cookie
                    context.Token = context.Request.Cookies["access_token"];
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
} 
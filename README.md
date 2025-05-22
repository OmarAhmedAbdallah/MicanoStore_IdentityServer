# Identity Server for Microservices

This solution provides a centralized Identity Server for microservices authentication using cookie-based authentication. It consists of two projects:

1. **IdentityServer**: The main identity server application that handles authentication and authorization
2. **SharedLibrary**: A shared library containing common code and authentication helpers

## Setup

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full installation)

### Getting Started

1. Clone the repository
2. Navigate to the solution directory
3. Run the following commands:

```bash
dotnet restore
dotnet build
cd src/IdentityServer
dotnet run
```

The Identity Server will be available at `https://localhost:5001`.

## Using in Microservices

To use the Identity Server in your microservices, follow these steps:

1. Add a reference to the SharedLibrary project in your microservice
2. In your microservice's `Program.cs`, add the following code:

```csharp
builder.Services.AddMicroserviceAuthentication("https://localhost:5001");

// ... other service configurations ...

app.UseAuthentication();
app.UseAuthorization();
```

3. Protect your endpoints using the `[Authorize]` attribute:

```csharp
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        // Your code here
    }
}
```

## Configuration

The Identity Server is configured with the following defaults:

- Uses SQL Server LocalDB for storing user data
- Includes OpenID Connect and OAuth 2.0 endpoints
- Supports both machine-to-machine and interactive user authentication
- Uses cookie-based authentication for web applications

### Client Configuration

Two default clients are configured:

1. Machine-to-machine client:
   - ClientId: "client"
   - Secret: "secret"
   - Grant Type: Client Credentials

2. Interactive Web Application:
   - ClientId: "web"
   - Secret: "secret"
   - Grant Type: Authorization Code
   - Redirect URIs: https://localhost:5002/signin-oidc
   - Post Logout Redirect URIs: https://localhost:5002/signout-callback-oidc

## Security Notes

- The current configuration uses a developer signing credential which is not suitable for production
- In production, you should:
  - Use proper SSL certificates
  - Use secure secrets and keys
  - Configure proper CORS policies
  - Use a production-grade database
  - Configure proper logging and monitoring 

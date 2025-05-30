# User Interaction Service

This service handles user interactions such as bookmarks, likes, and ratings for items in the Micano Store.

## Features

- Bookmarks: Users can bookmark items and add notes
- Likes: Users can like/unlike items
- Ratings: Users can rate items (1-5 stars) and add comments

## Technologies

- .NET 8.0
- Entity Framework Core
- MediatR (CQRS pattern)
- FluentValidation
- JWT Authentication with IdentityServer

## Project Structure

The project follows vertical slice architecture:

```
UserInteraction/
├── Domain/
│   └── Models/
├── Features/
│   ├── Bookmarks/
│   ├── Likes/
│   └── Ratings/
└── Infrastructure/
    └── Data/
```

## Setup

1. Update the connection string in `appsettings.json` if needed
2. Run database migrations:
   ```bash
   dotnet ef database update
   ```
3. Configure the IdentityServer URL in `appsettings.json`
4. Run the application:
   ```bash
   dotnet run
   ```

## API Endpoints

### Bookmarks
- POST `/api/bookmarks` - Add a bookmark
- DELETE `/api/bookmarks/{itemId}` - Remove a bookmark

### Likes
- POST `/api/likes/{itemId}/toggle` - Toggle like status

### Ratings
- POST `/api/ratings` - Add or update a rating
- DELETE `/api/ratings/{itemId}` - Remove a rating

## Authentication

All endpoints require authentication. The service expects a valid JWT token from the configured IdentityServer.

## Dependencies

Make sure you have the following prerequisites installed:
- .NET 8.0 SDK
- SQL Server (or SQL Server LocalDB)
- IdentityServer (running on the configured URL) 
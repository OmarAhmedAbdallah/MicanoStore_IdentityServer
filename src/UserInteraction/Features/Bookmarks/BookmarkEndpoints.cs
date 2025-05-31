using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using System.Security.Claims;
using UserInteraction.Features.Bookmarks.Commands;

namespace UserInteraction.Features.Bookmarks;

public static class BookmarkEndpoints
{
    public static void MapBookmarkEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookmarks")
            .WithTags("Bookmarks")
            .RequireAuthorization();

        group.MapPost("/", async (
            [FromBody] AddBookmarkCommand command,
            ISender mediator,
            ClaimsPrincipal user) =>
        {
             var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? user.FindFirst("sub")?.Value 
                ?? user.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            command = command with { UserId = userId };
            var result = await mediator.Send(command);
            
            return !result.Success 
                ? Results.BadRequest(result.Error) 
                : Results.Ok();
        })
        .WithName("AddBookmark")
        .WithOpenApi();

        group.MapDelete("/{itemId}", async (
            string itemId,
            ISender mediator,
            ClaimsPrincipal user) =>
        {
             var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? user.FindFirst("sub")?.Value 
                ?? user.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var command = new RemoveBookmarkCommand(userId, itemId);
            var result = await mediator.Send(command);
            
            return !result.Success 
                ? Results.NotFound(result.Error) 
                : Results.Ok();
        })
        .WithName("RemoveBookmark")
        .WithOpenApi();
    }
} 
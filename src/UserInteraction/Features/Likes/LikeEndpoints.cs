using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using System.Security.Claims;
using UserInteraction.Features.Likes.Commands;

namespace UserInteraction.Features.Likes;

public static class LikeEndpoints
{
    public static void MapLikeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/likes")
            .WithTags("Likes")
            .RequireAuthorization();

        group.MapPost("/{itemId}/toggle", async (
            string itemId,
            ISender mediator,
            ClaimsPrincipal user) =>
        {
            var command = new ToggleLikeCommand(user.FindFirst("sub")?.Value!, itemId);
            var result = await mediator.Send(command);
            
            return !result.Success 
                ? Results.BadRequest(result.Error) 
                : Results.Ok(new { isLiked = result.IsLiked });
        })
        .WithName("ToggleLike")
        .WithOpenApi();
    }
} 
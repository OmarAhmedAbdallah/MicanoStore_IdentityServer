using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using System.Security.Claims;
using UserInteraction.Features.Ratings.Commands;

namespace UserInteraction.Features.Ratings;

public static class RatingEndpoints
{
    public static void MapRatingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ratings")
            .WithTags("Ratings")
            .RequireAuthorization();

        group.MapPost("/", async (
            [FromBody] AddOrUpdateRatingCommand command,
            ISender mediator,
            ClaimsPrincipal user) =>
        {
            command = command with { UserId = user.FindFirst("sub")?.Value! };
            var result = await mediator.Send(command);
            
            return !result.Success 
                ? Results.BadRequest(result.Error) 
                : Results.Ok();
        })
        .WithName("AddOrUpdateRating")
        .WithOpenApi();

        group.MapDelete("/{itemId}", async (
            string itemId,
            ISender mediator,
            ClaimsPrincipal user) =>
        {
            var command = new RemoveRatingCommand(user.FindFirst("sub")?.Value!, itemId);
            var result = await mediator.Send(command);
            
            return !result.Success 
                ? Results.NotFound(result.Error) 
                : Results.Ok();
        })
        .WithName("RemoveRating")
        .WithOpenApi();
    }
} 
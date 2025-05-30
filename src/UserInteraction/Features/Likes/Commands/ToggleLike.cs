using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserInteraction.Domain.Models;
using UserInteraction.Infrastructure.Data;

namespace UserInteraction.Features.Likes.Commands;

public record ToggleLikeCommand(string UserId, string ItemId) : IRequest<ToggleLikeResult>;

public record ToggleLikeResult(bool Success, bool IsLiked, string? Error = null);

public class ToggleLikeCommandValidator : AbstractValidator<ToggleLikeCommand>
{
    public ToggleLikeCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
    }
}

public class ToggleLikeCommandHandler : IRequestHandler<ToggleLikeCommand, ToggleLikeResult>
{
    private readonly UserInteractionDbContext _context;

    public ToggleLikeCommandHandler(UserInteractionDbContext context)
    {
        _context = context;
    }

    public async Task<ToggleLikeResult> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        var like = await _context.Likes
            .FirstOrDefaultAsync(l => l.UserId == request.UserId && l.ItemId == request.ItemId, cancellationToken);

        if (like == null)
        {
            like = new Like
            {
                UserId = request.UserId,
                ItemId = request.ItemId,
                IsLiked = true
            };
            _context.Likes.Add(like);
        }
        else
        {
            like.IsLiked = !like.IsLiked;
            like.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new ToggleLikeResult(true, like.IsLiked);
    }
} 
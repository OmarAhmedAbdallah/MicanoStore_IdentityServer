using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserInteraction.Infrastructure.Data;

namespace UserInteraction.Features.Ratings.Commands;

public record RemoveRatingCommand(string UserId, string ItemId) : IRequest<RemoveRatingResult>;

public record RemoveRatingResult(bool Success, string? Error = null);

public class RemoveRatingCommandValidator : AbstractValidator<RemoveRatingCommand>
{
    public RemoveRatingCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
    }
}

public class RemoveRatingCommandHandler : IRequestHandler<RemoveRatingCommand, RemoveRatingResult>
{
    private readonly UserInteractionDbContext _context;

    public RemoveRatingCommandHandler(UserInteractionDbContext context)
    {
        _context = context;
    }

    public async Task<RemoveRatingResult> Handle(RemoveRatingCommand request, CancellationToken cancellationToken)
    {
        var rating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ItemId == request.ItemId, cancellationToken);

        if (rating == null)
        {
            return new RemoveRatingResult(false, "Rating not found");
        }

        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync(cancellationToken);

        return new RemoveRatingResult(true);
    }
} 
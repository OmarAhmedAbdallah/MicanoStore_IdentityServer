using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserInteraction.Domain.Models;
using UserInteraction.Infrastructure.Data;

namespace UserInteraction.Features.Ratings.Commands;

public record AddOrUpdateRatingCommand(string UserId, string ItemId, int Value, string? Comment) : IRequest<AddOrUpdateRatingResult>;

public record AddOrUpdateRatingResult(bool Success, string? Error = null);

public class AddOrUpdateRatingCommandValidator : AbstractValidator<AddOrUpdateRatingCommand>
{
    public AddOrUpdateRatingCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Value).InclusiveBetween(1, 5);
    }
}

public class AddOrUpdateRatingCommandHandler : IRequestHandler<AddOrUpdateRatingCommand, AddOrUpdateRatingResult>
{
    private readonly UserInteractionDbContext _context;

    public AddOrUpdateRatingCommandHandler(UserInteractionDbContext context)
    {
        _context = context;
    }

    public async Task<AddOrUpdateRatingResult> Handle(AddOrUpdateRatingCommand request, CancellationToken cancellationToken)
    {
        var rating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ItemId == request.ItemId, cancellationToken);

        if (rating == null)
        {
            rating = new Rating
            {
                UserId = request.UserId,
                ItemId = request.ItemId,
                Value = request.Value,
                Comment = request.Comment
            };
            _context.Ratings.Add(rating);
        }
        else
        {
            rating.Value = request.Value;
            rating.Comment = request.Comment;
            rating.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new AddOrUpdateRatingResult(true);
    }
} 
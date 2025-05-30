using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserInteraction.Infrastructure.Data;

namespace UserInteraction.Features.Bookmarks.Commands;

public record RemoveBookmarkCommand(string UserId, string ItemId) : IRequest<RemoveBookmarkResult>;

public record RemoveBookmarkResult(bool Success, string? Error = null);

public class RemoveBookmarkCommandValidator : AbstractValidator<RemoveBookmarkCommand>
{
    public RemoveBookmarkCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
    }
}

public class RemoveBookmarkCommandHandler : IRequestHandler<RemoveBookmarkCommand, RemoveBookmarkResult>
{
    private readonly UserInteractionDbContext _context;

    public RemoveBookmarkCommandHandler(UserInteractionDbContext context)
    {
        _context = context;
    }

    public async Task<RemoveBookmarkResult> Handle(RemoveBookmarkCommand request, CancellationToken cancellationToken)
    {
        var bookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == request.UserId && b.ItemId == request.ItemId, cancellationToken);

        if (bookmark == null)
        {
            return new RemoveBookmarkResult(false, "Bookmark not found");
        }

        _context.Bookmarks.Remove(bookmark);
        await _context.SaveChangesAsync(cancellationToken);

        return new RemoveBookmarkResult(true);
    }
} 
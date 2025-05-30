using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserInteraction.Domain.Models;
using UserInteraction.Infrastructure.Data;

namespace UserInteraction.Features.Bookmarks.Commands;

public record AddBookmarkCommand(string UserId, string ItemId, string? Notes) : IRequest<AddBookmarkResult>;

public record AddBookmarkResult(bool Success, string? Error = null);

public class AddBookmarkCommandValidator : AbstractValidator<AddBookmarkCommand>
{
    public AddBookmarkCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
    }
}

public class AddBookmarkCommandHandler : IRequestHandler<AddBookmarkCommand, AddBookmarkResult>
{
    private readonly UserInteractionDbContext _context;

    public AddBookmarkCommandHandler(UserInteractionDbContext context)
    {
        _context = context;
    }

    public async Task<AddBookmarkResult> Handle(AddBookmarkCommand request, CancellationToken cancellationToken)
    {
        var existingBookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == request.UserId && b.ItemId == request.ItemId, cancellationToken);

        if (existingBookmark != null)
        {
            return new AddBookmarkResult(false, "Bookmark already exists");
        }

        var bookmark = new Bookmark
        {
            UserId = request.UserId,
            ItemId = request.ItemId,
            Notes = request.Notes
        };

        _context.Bookmarks.Add(bookmark);
        await _context.SaveChangesAsync(cancellationToken);

        return new AddBookmarkResult(true);
    }
} 
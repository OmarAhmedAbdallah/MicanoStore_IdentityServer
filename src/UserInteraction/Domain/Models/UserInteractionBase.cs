namespace UserInteraction.Domain.Models;

public abstract class UserInteractionBase
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string ItemId { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
} 
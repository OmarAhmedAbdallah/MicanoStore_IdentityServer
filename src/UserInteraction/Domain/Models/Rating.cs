namespace UserInteraction.Domain.Models;

public class Rating : UserInteractionBase
{
    public int Value { get; set; }
    public string? Comment { get; set; }
} 
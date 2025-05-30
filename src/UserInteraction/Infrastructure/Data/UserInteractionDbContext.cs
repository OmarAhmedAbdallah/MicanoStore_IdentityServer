using Microsoft.EntityFrameworkCore;
using UserInteraction.Domain.Models;

namespace UserInteraction.Infrastructure.Data;

public class UserInteractionDbContext : DbContext
{
    public UserInteractionDbContext(DbContextOptions<UserInteractionDbContext> options) : base(options)
    {
    }

    public DbSet<Bookmark> Bookmarks { get; set; } = default!;
    public DbSet<Like> Likes { get; set; } = default!;
    public DbSet<Rating> Ratings { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bookmark>()
            .HasIndex(b => new { b.UserId, b.ItemId })
            .IsUnique();

        modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.UserId, l.ItemId })
            .IsUnique();

        modelBuilder.Entity<Rating>()
            .HasIndex(r => new { r.UserId, r.ItemId })
            .IsUnique();

        modelBuilder.Entity<Rating>()
            .Property(r => r.Value)
            .HasAnnotation("Range", new[] { 1, 5 });
    }
} 
using Microsoft.EntityFrameworkCore;
using MentorMenteeApp.API.Models.Entities;

namespace MentorMenteeApp.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<MatchRequest> MatchRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.Role).HasConversion<string>();
        });

        // MatchRequest entity configuration
        modelBuilder.Entity<MatchRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.Status).HasConversion<string>();

            // Configure relationships
            entity.HasOne(e => e.Mentor)
                  .WithMany(u => u.ReceivedRequests)
                  .HasForeignKey(e => e.MentorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Mentee)
                  .WithMany(u => u.SentRequests)
                  .HasForeignKey(e => e.MenteeId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate requests from same mentee to same mentor
            entity.HasIndex(e => new { e.MentorId, e.MenteeId })
                  .IsUnique()
                  .HasFilter("[Status] = 'Pending' OR [Status] = 'Accepted'");
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is User || e.Entity is MatchRequest)
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is User user)
                {
                    user.CreatedAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is MatchRequest request)
                {
                    request.CreatedAt = DateTime.UtcNow;
                    request.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is User user)
                    user.UpdatedAt = DateTime.UtcNow;
                else if (entry.Entity is MatchRequest request)
                    request.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

using Drift.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drift.Infrastructure.Data;

public class DriftDbContext(DbContextOptions<DriftDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<SleepGoal> SleepGoals => Set<SleepGoal>();
    public DbSet<SleepLog> SleepLogs => Set<SleepLog>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(320);
            e.Property(u => u.DisplayName).HasMaxLength(80);
            e.Property(u => u.TimeZone).HasMaxLength(64);
        });

        b.Entity<SleepGoal>(e =>
        {
            e.HasOne<User>().WithMany(u => u.Goals).HasForeignKey(g => g.UserId).OnDelete(DeleteBehavior.Cascade);
            e.Property(g => g.TargetHours).HasPrecision(3, 1);
            e.HasIndex(g => new { g.UserId, g.EffectiveFrom }).IsUnique();
            e.ToTable(t => t.HasCheckConstraint("ck_sleep_goals_target_hours", "target_hours BETWEEN 4 AND 12"));
        });

        b.Entity<SleepLog>(e =>
        {
            e.HasOne<User>().WithMany(u => u.SleepLogs).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
            e.Ignore(l => l.Duration);
            // One log per night per user: the database enforces it even if the API has a bug.
            e.HasIndex(l => new { l.UserId, l.NightOf }).IsUnique();
            e.ToTable(t => t.HasCheckConstraint("ck_sleep_logs_woke_after_slept", "woke_at > slept_at"));
        });

        SeedData.Apply(b);
    }
}

using Drift.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Drift.Infrastructure.Data;

/// <summary>
/// A demo user so the app works before accounts exist (Milestone 2 replaces this with real sign-up).
/// Fixed ids and dates keep migrations stable: seed data that changes on every build creates a new migration every time.
/// </summary>
public static class SeedData
{
    public static readonly Guid DemoUserId = Guid.Parse("6f1b2c3d-0000-4000-8000-000000000001");
    private static readonly Guid DemoGoalId = Guid.Parse("6f1b2c3d-0000-4000-8000-000000000002");

    public static void Apply(ModelBuilder b)
    {
        b.Entity<User>().HasData(new User
        {
            Id = DemoUserId,
            Email = "demo@drift.local",
            DisplayName = "Demo",
            TimeZone = "Asia/Jerusalem",
            CreatedAt = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero),
        });

        b.Entity<SleepGoal>().HasData(new SleepGoal
        {
            Id = DemoGoalId,
            UserId = DemoUserId,
            TargetHours = 7.0m,
            TargetBedtime = new TimeOnly(23, 0),
            TargetWakeTime = new TimeOnly(6, 30),
            BedtimeWindowMinutes = 30,
            EffectiveFrom = new DateOnly(2026, 11, 1),
        });
    }
}

using Drift.Core.Entities;
using Drift.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Drift.Api.Services;

/// <summary>Loads the current user with what most endpoints need: their time zone and goal history.</summary>
public sealed class UserContext(DriftDbContext db, ICurrentUser currentUser)
{
    public async Task<(User User, TimeZoneInfo Zone)> LoadAsync(CancellationToken ct)
    {
        var user = await db.Users.Include(u => u.Goals).AsNoTracking()
            .SingleAsync(u => u.Id == currentUser.Id, ct);
        return (user, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone));
    }

    /// <summary>The goal that applied on a given night: the latest one that took effect on or before it.</summary>
    public static SleepGoal GoalFor(User user, DateOnly nightOf) =>
        user.Goals.Where(g => g.EffectiveFrom <= nightOf).MaxBy(g => g.EffectiveFrom)
        ?? user.Goals.MinBy(g => g.EffectiveFrom)
        ?? new SleepGoal { UserId = user.Id, EffectiveFrom = nightOf };

    public static DateOnly LocalToday(TimeZoneInfo zone, TimeProvider clock) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.GetUtcNow(), zone).DateTime);
}

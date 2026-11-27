using Drift.Api.Contracts;
using Drift.Api.Services;
using Drift.Core.Sleep;
using Drift.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Drift.Api.Controllers;

[ApiController]
[Route("api/summary")]
public sealed class SummaryController(DriftDbContext db, UserContext users, TimeProvider clock) : ControllerBase
{
    /// <summary>Today's view for the Home screen: the goal and how last night went.</summary>
    [HttpGet("today")]
    public async Task<ActionResult<TodaySummaryResponse>> Today(CancellationToken ct)
    {
        var (user, zone) = await users.LoadAsync(ct);
        var today = UserContext.LocalToday(zone, clock);
        var goal = UserContext.GoalFor(user, today);

        var last = await db.SleepLogs.AsNoTracking()
            .Where(l => l.UserId == user.Id)
            .OrderByDescending(l => l.NightOf)
            .FirstOrDefaultAsync(ct);

        var nightToLog = SleepNight.NightToLog(clock.GetUtcNow(), zone);
        var lastNight = last is null
            ? null
            : SleepLogResponse.From(last.Id, NightSummary.For(last, UserContext.GoalFor(user, last.NightOf), zone));

        return new TodaySummaryResponse(
            user.DisplayName,
            today,
            nightToLog,
            new GoalResponse(goal.TargetHours, goal.TargetBedtime, goal.TargetWakeTime, goal.BedtimeWindowMinutes),
            lastNight,
            LastNightLogged: last?.NightOf >= nightToLog);
    }
}

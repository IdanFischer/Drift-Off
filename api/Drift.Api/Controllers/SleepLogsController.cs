using Drift.Api.Contracts;
using Drift.Api.Services;
using Drift.Core.Entities;
using Drift.Core.Sleep;
using Drift.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Drift.Api.Controllers;

[ApiController]
[Route("api/sleep-logs")]
public sealed class SleepLogsController(DriftDbContext db, UserContext users, TimeProvider clock) : ControllerBase
{
    /// <summary>Log one night of sleep.</summary>
    [HttpPost]
    [ProducesResponseType<SleepLogResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SleepLogResponse>> Create(CreateSleepLogRequest request, CancellationToken ct)
    {
        var (user, zone) = await users.LoadAsync(ct);
        var nightOf = request.NightOf!.Value; // [Required] guarantees these are present
        var (sleptAt, wokeAt) = SleepNight.ToInstants(nightOf, request.Bedtime!.Value, request.WakeTime!.Value, zone);

        var errors = SleepLogRules.Validate(nightOf, sleptAt, wokeAt, clock.GetUtcNow());
        if (errors.Count > 0)
        {
            foreach (var e in errors) ModelState.AddModelError(nameof(request.WakeTime), e);
            return ValidationProblem(ModelState);
        }

        if (await db.SleepLogs.AnyAsync(l => l.UserId == user.Id && l.NightOf == nightOf, ct))
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "That night is already logged.");

        var log = new SleepLog
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            NightOf = nightOf,
            SleptAt = sleptAt,
            WokeAt = wokeAt,
            CreatedAt = clock.GetUtcNow(),
        };
        db.SleepLogs.Add(log);
        await db.SaveChangesAsync(ct);

        var body = SleepLogResponse.From(log.Id, NightSummary.For(log, UserContext.GoalFor(user, log.NightOf), zone));
        return CreatedAtAction(nameof(List), new { from = log.NightOf, to = log.NightOf }, body);
    }

    /// <summary>Nights between two dates (inclusive), newest first. Defaults to the last 7 nights.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SleepLogResponse>>> List(DateOnly? from, DateOnly? to, CancellationToken ct)
    {
        var (user, zone) = await users.LoadAsync(ct);
        var end = to ?? UserContext.LocalToday(zone, clock);
        var start = from ?? end.AddDays(-6);
        if (start > end) return ValidationProblem("'from' must be on or before 'to'.");

        var logs = await db.SleepLogs.AsNoTracking()
            .Where(l => l.UserId == user.Id && l.NightOf >= start && l.NightOf <= end)
            .OrderByDescending(l => l.NightOf)
            .ToListAsync(ct);

        return logs.Select(l => SleepLogResponse.From(l.Id, NightSummary.For(l, UserContext.GoalFor(user, l.NightOf), zone))).ToList();
    }
}

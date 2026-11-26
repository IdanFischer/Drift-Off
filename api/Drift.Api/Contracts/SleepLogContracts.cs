using System.ComponentModel.DataAnnotations;
using Drift.Core.Sleep;

namespace Drift.Api.Contracts;

/// <summary>
/// What the Log sleep screen sends: local clock times plus the night they belong to.
/// The server works out the exact instants from the user's time zone, so the browser never does date math.
/// </summary>
/// <remarks>Fields are nullable so a missing value fails validation with a 400 instead of silently becoming 00:00.</remarks>
public sealed record CreateSleepLogRequest(
    [Required] DateOnly? NightOf,
    [Required] TimeOnly? Bedtime,
    [Required] TimeOnly? WakeTime);

public sealed record SleepLogResponse(
    Guid Id,
    DateOnly NightOf,
    TimeOnly Bedtime,
    TimeOnly WakeTime,
    int DurationMinutes,
    bool HoursGoalMet,
    int BedtimeMinutesFromTarget,
    bool BedtimeOnTarget,
    int WakeMinutesFromTarget)
{
    public static SleepLogResponse From(Guid id, NightSummary s) => new(
        id, s.NightOf, s.Bedtime, s.WakeTime, s.DurationMinutes, s.HoursGoalMet,
        s.BedtimeMinutesFromTarget, s.BedtimeOnTarget, s.WakeMinutesFromTarget);
}

public sealed record GoalResponse(decimal TargetHours, TimeOnly TargetBedtime, TimeOnly TargetWakeTime, int BedtimeWindowMinutes);

/// <summary>Everything the Home screen needs in one request.</summary>
public sealed record TodaySummaryResponse(
    string DisplayName,
    DateOnly Today,
    DateOnly NightToLog,
    GoalResponse Goal,
    SleepLogResponse? LastNight,
    bool LastNightLogged);

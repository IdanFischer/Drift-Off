using Drift.Core.Entities;

namespace Drift.Core.Sleep;

/// <summary>How one night compares with the goal: the numbers behind the Home screen.</summary>
public sealed record NightSummary(
    DateOnly NightOf,
    TimeOnly Bedtime,
    TimeOnly WakeTime,
    int DurationMinutes,
    int GoalMinutes,
    bool HoursGoalMet,
    int BedtimeMinutesFromTarget,
    bool BedtimeOnTarget,
    int WakeMinutesFromTarget)
{
    public static NightSummary For(SleepLog log, SleepGoal goal, TimeZoneInfo zone)
    {
        var bed = SleepNight.LocalTimeOf(log.SleptAt, zone);
        var wake = SleepNight.LocalTimeOf(log.WokeAt, zone);
        var duration = (int)Math.Round(log.Duration.TotalMinutes);
        var goalMinutes = (int)Math.Round(goal.TargetHours * 60);
        var bedDelta = SleepNight.MinutesFromTarget(bed, goal.TargetBedtime);

        return new NightSummary(
            log.NightOf, bed, wake, duration, goalMinutes,
            HoursGoalMet: duration >= goalMinutes,
            BedtimeMinutesFromTarget: bedDelta,
            BedtimeOnTarget: Math.Abs(bedDelta) <= goal.BedtimeWindowMinutes,
            WakeMinutesFromTarget: SleepNight.MinutesFromTarget(wake, goal.TargetWakeTime));
    }
}

namespace Drift.Core.Entities;

/// <summary>
/// A user's targets. Changing a goal adds a new row instead of editing the old one,
/// so past nights are always scored against the goal that applied at the time.
/// </summary>
public class SleepGoal
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public decimal TargetHours { get; set; } = 7.0m;
    public TimeOnly TargetBedtime { get; set; } = new(23, 0);
    public TimeOnly TargetWakeTime { get; set; } = new(6, 30);

    /// <summary>How close to the target bedtime still counts as "on time".</summary>
    public int BedtimeWindowMinutes { get; set; } = 30;

    public DateOnly EffectiveFrom { get; set; }
}

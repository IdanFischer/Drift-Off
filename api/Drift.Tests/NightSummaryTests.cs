using Drift.Core.Entities;
using Drift.Core.Sleep;

namespace Drift.Tests;

public class NightSummaryTests
{
    private static readonly TimeZoneInfo Utc = TimeZoneInfo.Utc;
    private static readonly SleepGoal Goal = new() { TargetHours = 7.0m, TargetBedtime = new(23, 0), TargetWakeTime = new(6, 30), BedtimeWindowMinutes = 30 };

    private static SleepLog Log(TimeOnly bed, TimeOnly wake)
    {
        var (slept, woke) = SleepNight.ToInstants(new DateOnly(2026, 11, 25), bed, wake, Utc);
        return new SleepLog { NightOf = new DateOnly(2026, 11, 25), SleptAt = slept, WokeAt = woke };
    }

    [Fact]
    public void Figma_example_meets_both_goals()
    {
        var s = NightSummary.For(Log(new(22, 48), new(6, 30)), Goal, Utc);

        Assert.Equal(462, s.DurationMinutes);
        Assert.True(s.HoursGoalMet);
        Assert.Equal(-12, s.BedtimeMinutesFromTarget);
        Assert.True(s.BedtimeOnTarget);
        Assert.Equal(0, s.WakeMinutesFromTarget);
    }

    [Fact]
    public void Exactly_seven_hours_counts_and_one_minute_less_does_not()
    {
        Assert.True(NightSummary.For(Log(new(23, 0), new(6, 0)), Goal, Utc).HoursGoalMet);
        Assert.False(NightSummary.For(Log(new(23, 1), new(6, 0)), Goal, Utc).HoursGoalMet);
    }

    [Theory]
    [InlineData(23, 30, true)]   // edge of the 30 min window
    [InlineData(23, 31, false)]
    [InlineData(22, 30, true)]
    [InlineData(22, 29, false)]
    public void Bedtime_window_is_inclusive(int h, int m, bool onTarget)
    {
        Assert.Equal(onTarget, NightSummary.For(Log(new(h, m), new(7, 0)), Goal, Utc).BedtimeOnTarget);
    }
}

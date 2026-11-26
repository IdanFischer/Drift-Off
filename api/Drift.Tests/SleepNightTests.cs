using Drift.Core.Sleep;

namespace Drift.Tests;

public class SleepNightTests
{
    private static readonly TimeZoneInfo Jerusalem = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jerusalem");
    private static readonly TimeZoneInfo NewYork = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

    [Fact]
    public void Evening_bedtime_wakes_the_next_morning()
    {
        // The Figma example: 10:48 PM to 6:30 AM is 7h 42m.
        var (slept, woke) = SleepNight.ToInstants(new DateOnly(2026, 11, 25), new TimeOnly(22, 48), new TimeOnly(6, 30), Jerusalem);

        Assert.Equal(new DateTimeOffset(2026, 11, 25, 20, 48, 0, TimeSpan.Zero), slept); // Jerusalem is UTC+2 in November
        Assert.Equal(TimeSpan.FromMinutes(7 * 60 + 42), woke - slept);
    }

    [Fact]
    public void Bedtime_after_midnight_belongs_to_the_evening_before()
    {
        var (slept, woke) = SleepNight.ToInstants(new DateOnly(2026, 11, 25), new TimeOnly(1, 15), new TimeOnly(8, 0), Jerusalem);

        Assert.Equal(new DateTimeOffset(2026, 11, 26, 1, 15, 0, TimeSpan.FromHours(2)), slept);
        Assert.Equal(TimeSpan.FromMinutes(6 * 60 + 45), woke - slept);
        Assert.Equal(new DateOnly(2026, 11, 25), SleepNight.NightOfFor(slept, Jerusalem));
    }

    [Fact]
    public void Night_across_a_clock_change_uses_real_elapsed_time()
    {
        // US clocks fall back at 2 AM on Nov 1 2026, so 11 PM to 7 AM is 9 real hours, not 8.
        var (slept, woke) = SleepNight.ToInstants(new DateOnly(2026, 10, 31), new TimeOnly(23, 0), new TimeOnly(7, 0), NewYork);

        Assert.Equal(TimeSpan.FromHours(9), woke - slept);
    }

    [Theory]
    [InlineData(22, 48, 23, 0, -12)]   // 12 min early
    [InlineData(23, 20, 23, 0, 20)]    // 20 min late
    [InlineData(0, 10, 23, 30, 40)]    // past midnight is still "late", not 23 hours early
    [InlineData(23, 50, 0, 15, -25)]   // before a just-after-midnight target is "early"
    public void Minutes_from_target_takes_the_short_way_round_midnight(int h, int m, int th, int tm, int expected)
    {
        Assert.Equal(expected, SleepNight.MinutesFromTarget(new TimeOnly(h, m), new TimeOnly(th, tm)));
    }

    [Theory]
    [InlineData("2026-11-27T09:00:00+02:00", "2026-11-26")] // morning: the night that just ended
    [InlineData("2026-11-27T22:30:00+02:00", "2026-11-26")] // evening: still last night until you sleep
    [InlineData("2026-11-27T00:10:00+02:00", "2026-11-25")] // just after midnight: tonight hasn't ended yet
    public void Night_to_log_is_the_most_recent_night_that_has_ended(string now, string expected)
    {
        Assert.Equal(DateOnly.Parse(expected), SleepNight.NightToLog(DateTimeOffset.Parse(now), Jerusalem));
    }
}

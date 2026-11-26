namespace Drift.Core.Sleep;

/// <summary>
/// Turns what the Log sleep screen collects (a night, a bedtime and a wake-up time, all in local time)
/// into exact UTC instants. Pure logic with no database, so it is easy to unit test.
/// </summary>
public static class SleepNight
{
    /// <summary>Bedtimes before this hour are treated as "after midnight", i.e. the next calendar day.</summary>
    public static readonly TimeOnly AfterMidnightCutoff = new(12, 0);

    public static (DateTimeOffset SleptAt, DateTimeOffset WokeAt) ToInstants(
        DateOnly nightOf, TimeOnly bedtime, TimeOnly wakeTime, TimeZoneInfo zone)
    {
        var bedDate = bedtime < AfterMidnightCutoff ? nightOf.AddDays(1) : nightOf;
        var sleptLocal = bedDate.ToDateTime(bedtime);

        // Wake-up is the first occurrence of wakeTime after falling asleep.
        var wakeLocal = bedDate.ToDateTime(wakeTime);
        if (wakeLocal <= sleptLocal) wakeLocal = wakeLocal.AddDays(1);

        return (ToUtc(sleptLocal, zone), ToUtc(wakeLocal, zone));
    }

    /// <summary>The evening a sleep start belongs to, in the user's local time.</summary>
    public static DateOnly NightOfFor(DateTimeOffset sleptAt, TimeZoneInfo zone)
    {
        var local = TimeZoneInfo.ConvertTime(sleptAt, zone);
        var date = DateOnly.FromDateTime(local.DateTime);
        return TimeOnly.FromDateTime(local.DateTime) < AfterMidnightCutoff ? date.AddDays(-1) : date;
    }

    /// <summary>
    /// The night the Log sleep screen should offer: the most recent one that has ended.
    /// At 9 AM on Friday that is Thursday night. Just after midnight you haven't slept yet,
    /// so until 4 AM it is still the night before.
    /// </summary>
    public static DateOnly NightToLog(DateTimeOffset now, TimeZoneInfo zone)
    {
        var local = TimeZoneInfo.ConvertTime(now, zone).DateTime;
        return DateOnly.FromDateTime(local).AddDays(local.Hour < 4 ? -2 : -1);
    }

    public static TimeOnly LocalTimeOf(DateTimeOffset instant, TimeZoneInfo zone) =>
        TimeOnly.FromDateTime(TimeZoneInfo.ConvertTime(instant, zone).DateTime);

    /// <summary>
    /// Signed minutes between an actual and a target clock time, taking the shortest way round midnight.
    /// Negative means earlier than the target: 22:48 against 23:00 is -12.
    /// </summary>
    public static int MinutesFromTarget(TimeOnly actual, TimeOnly target)
    {
        var diff = (int)Math.Round((actual - target).TotalMinutes); // TimeOnly subtraction wraps to 0..24h
        return diff > 12 * 60 ? diff - 24 * 60 : diff;
    }

    private static DateTimeOffset ToUtc(DateTime local, TimeZoneInfo zone)
    {
        // A clock time skipped by a daylight-saving jump does not exist; move it past the gap.
        while (zone.IsInvalidTime(local)) local = local.AddMinutes(30);
        var utc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), zone);
        return new DateTimeOffset(utc, TimeSpan.Zero);
    }
}

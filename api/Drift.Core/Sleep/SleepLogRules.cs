namespace Drift.Core.Sleep;

/// <summary>Validation rules for a night of sleep, kept out of the controller so they can be tested directly.</summary>
public static class SleepLogRules
{
    public static readonly TimeSpan MinDuration = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(16);

    public static IReadOnlyList<string> Validate(DateOnly nightOf, DateTimeOffset sleptAt, DateTimeOffset wokeAt, DateTimeOffset now)
    {
        var errors = new List<string>();
        var duration = wokeAt - sleptAt;

        if (duration < MinDuration) errors.Add("Sleep must last at least 30 minutes.");
        if (duration > MaxDuration) errors.Add("Sleep can't be longer than 16 hours. Check the bedtime and wake-up times.");
        if (wokeAt > now.AddMinutes(5)) errors.Add("Wake-up time can't be in the future.");
        if (nightOf < DateOnly.FromDateTime(now.UtcDateTime).AddDays(-60)) errors.Add("Nights older than 60 days can't be logged.");

        return errors;
    }
}

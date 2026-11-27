namespace Drift.Core.Entities;

/// <summary>One night of sleep. A user has at most one log per <see cref="NightOf"/>.</summary>
public class SleepLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    /// <summary>The local calendar date of the evening you went to bed (a 1 AM bedtime still belongs to the evening before).</summary>
    public DateOnly NightOf { get; set; }

    /// <summary>Stored in UTC; converted to the user's time zone for display.</summary>
    public DateTimeOffset SleptAt { get; set; }
    public DateTimeOffset WokeAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public TimeSpan Duration => WokeAt - SleptAt;
}

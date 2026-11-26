namespace Drift.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }

    /// <summary>IANA time zone id, e.g. "Asia/Jerusalem". Bedtimes only make sense in local time.</summary>
    public required string TimeZone { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public List<SleepGoal> Goals { get; set; } = [];
    public List<SleepLog> SleepLogs { get; set; } = [];
}

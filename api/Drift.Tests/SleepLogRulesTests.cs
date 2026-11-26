using Drift.Core.Sleep;

namespace Drift.Tests;

public class SleepLogRulesTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 26, 9, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly Night = new(2026, 11, 25);

    [Fact]
    public void Normal_night_is_valid() =>
        Assert.Empty(SleepLogRules.Validate(Night, Now.AddHours(-10), Now.AddHours(-2), Now));

    [Fact]
    public void Too_short_is_rejected() =>
        Assert.Contains(SleepLogRules.Validate(Night, Now.AddHours(-3), Now.AddHours(-3).AddMinutes(10), Now), e => e.Contains("30 minutes"));

    [Fact]
    public void Too_long_is_rejected() =>
        Assert.Contains(SleepLogRules.Validate(Night, Now.AddHours(-20), Now.AddHours(-2), Now), e => e.Contains("16 hours"));

    [Fact]
    public void Waking_in_the_future_is_rejected() =>
        Assert.Contains(SleepLogRules.Validate(Night, Now.AddHours(-1), Now.AddHours(6), Now), e => e.Contains("future"));
}

using System.Net;
using System.Net.Http.Json;
using Drift.Api.Contracts;
using Drift.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

namespace Drift.Tests;

/// <summary>Starts the real API in memory, with an in-memory database and a fixed clock.</summary>
public sealed class DriftApiFactory : WebApplicationFactory<Program>
{
    // 9 AM in Jerusalem on Thu Nov 26 2026, the morning shown in the Figma frames.
    public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 11, 26, 7, 0, 0, TimeSpan.Zero));
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Drift", "Host=unused");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DriftDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<DriftDbContext>>();
            services.AddDbContext<DriftDbContext>(o => o.UseInMemoryDatabase(_dbName));
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(Clock);

            using var scope = services.BuildServiceProvider().CreateScope();
            scope.ServiceProvider.GetRequiredService<DriftDbContext>().Database.EnsureCreated(); // applies the seed data
        });
    }
}

public class ApiTests
{
    private static object Night(string nightOf, string bed, string wake) => new { nightOf, bedtime = bed, wakeTime = wake };

    [Fact]
    public async Task Logging_a_night_shows_up_on_the_home_summary()
    {
        await using var app = new DriftApiFactory();
        var http = app.CreateClient();

        var created = await http.PostAsJsonAsync("/api/sleep-logs", Night("2026-11-25", "22:48", "06:30"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var summary = await http.GetFromJsonAsync<TodaySummaryResponse>("/api/summary/today");
        Assert.NotNull(summary!.LastNight);
        Assert.True(summary.LastNightLogged);
        Assert.Equal(462, summary.LastNight!.DurationMinutes);
        Assert.Equal(-12, summary.LastNight.BedtimeMinutesFromTarget);
        Assert.True(summary.LastNight.HoursGoalMet);
    }

    [Fact]
    public async Task The_same_night_cannot_be_logged_twice()
    {
        await using var app = new DriftApiFactory();
        var http = app.CreateClient();

        await http.PostAsJsonAsync("/api/sleep-logs", Night("2026-11-25", "22:48", "06:30"));
        var again = await http.PostAsJsonAsync("/api/sleep-logs", Night("2026-11-25", "23:30", "07:00"));

        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
    }

    [Fact]
    public async Task Missing_fields_and_impossible_nights_return_400()
    {
        await using var app = new DriftApiFactory();
        var http = app.CreateClient();

        var missing = await http.PostAsJsonAsync("/api/sleep-logs", new { nightOf = "2026-11-25" });
        var tooShort = await http.PostAsJsonAsync("/api/sleep-logs", Night("2026-11-25", "06:10", "06:20"));

        Assert.Equal(HttpStatusCode.BadRequest, missing.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, tooShort.StatusCode);
    }

    [Fact]
    public async Task List_returns_the_last_seven_nights_newest_first()
    {
        await using var app = new DriftApiFactory();
        var http = app.CreateClient();

        foreach (var night in new[] { "2026-11-18", "2026-11-20", "2026-11-25" })
            (await http.PostAsJsonAsync("/api/sleep-logs", Night(night, "23:00", "06:30"))).EnsureSuccessStatusCode();

        var list = await http.GetFromJsonAsync<List<SleepLogResponse>>("/api/sleep-logs");

        Assert.Equal(["2026-11-25", "2026-11-20"], list!.Select(l => l.NightOf.ToString("yyyy-MM-dd")));
    }
}

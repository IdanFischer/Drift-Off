using System.Text.Json.Serialization;
using Drift.Api.Services;
using Drift.Infrastructure;
using Drift.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    // Numbers must be JSON numbers. The web default also accepts "42" as a string, which makes the OpenAPI types vaguer.
    .AddJsonOptions(o => o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict);
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.NumberHandling = JsonNumberHandling.Strict); // used by the OpenAPI generator
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Drift")
    ?? throw new InvalidOperationException("Set ConnectionStrings:Drift (see appsettings.Development.json).");
builder.Services.AddDriftDatabase(connectionString);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ICurrentUser, DemoCurrentUser>();
builder.Services.AddScoped<UserContext>();

const string WebOrigin = "web";
builder.Services.AddCors(o => o.AddPolicy(WebOrigin, p => p
    .WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:3000"])
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                 // the spec, at /openapi/v1.json (the frontend generates its types from it)
    app.MapScalarApiReference();      // interactive API docs, at /scalar

    // Apply pending migrations on start-up in development only. In production, migrations run as a deploy step.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DriftDbContext>();
    if (db.Database.IsRelational()) db.Database.Migrate();
}

app.UseExceptionHandler();
app.UseCors(WebOrigin);
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

/// <summary>Lets the test project start the API in memory with WebApplicationFactory.</summary>
public partial class Program;

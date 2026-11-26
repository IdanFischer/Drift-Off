using Drift.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Drift.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDriftDatabase(this IServiceCollection services, string connectionString) =>
        services.AddDbContext<DriftDbContext>(o => o
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());
}

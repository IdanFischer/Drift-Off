using Drift.Infrastructure.Data;

namespace Drift.Api.Services;

/// <summary>
/// Who is making the request. Milestone 1 has no sign-in, so this always returns the seeded demo user.
/// Controllers depend on this interface, so adding real authentication in Milestone 2 changes only this file.
/// </summary>
public interface ICurrentUser
{
    Guid Id { get; }
}

public sealed class DemoCurrentUser : ICurrentUser
{
    public Guid Id => SeedData.DemoUserId;
}

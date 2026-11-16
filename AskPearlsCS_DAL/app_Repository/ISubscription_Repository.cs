namespace AskPearlsCS_DAL.app_Repository;

public interface ISubscription_Repository
{
    // Idempotent registration/backfill operation; never changes existing subscription history.
    Task<bool> EnsureFreeAsync(string owner, CancellationToken ct = default);
}

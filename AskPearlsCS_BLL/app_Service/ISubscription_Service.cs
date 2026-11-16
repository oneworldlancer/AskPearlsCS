namespace AskPearlsCS_BLL.app_Service;

public interface ISubscription_Service
{
    Task<bool> EnsureFreeAsync(string owner, CancellationToken ct = default);
}

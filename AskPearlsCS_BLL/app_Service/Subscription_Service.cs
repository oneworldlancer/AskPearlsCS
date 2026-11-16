using AskPearlsCS_DAL.app_Repository;
namespace AskPearlsCS_BLL.app_Service;

public sealed class Subscription_Service : ISubscription_Service
{
    private readonly ISubscription_Repository _repository;
    public Subscription_Service(ISubscription_Repository repository) => _repository = repository;
    public Task<bool> EnsureFreeAsync(string owner, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(owner) || owner.Trim() == "0")
            throw new ArgumentException("A valid authenticated owner is required.");
        return _repository.EnsureFreeAsync(owner.Trim(), ct);
    }
}

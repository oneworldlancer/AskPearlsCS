using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_BLL.app_Service;

public sealed class Subscription_Service : ISubscription_Service
{
    private readonly ISubscription_Repository _repository;
    public Subscription_Service(ISubscription_Repository repository) => _repository = repository;

    private static string Owner(string? owner) =>
        string.IsNullOrWhiteSpace(owner) || owner.Trim() == "0"
            ? throw new ArgumentException("A valid authenticated owner is required.")
            : owner.Trim();

    // Catalog values are MVP demo prices, not chargeable offers. Centralized here
    // until a real plans table and verified billing-provider prices are configured.
    private static readonly IReadOnlyList<SubscriptionPlanDto> Plans = Array.AsReadOnly(new[]
    {
        new SubscriptionPlanDto("free", "Free", 0m, 5, "Explore the essentials and build your first widgets.", false),
        new SubscriptionPlanDto("starter", "Starter", 19m, 10, "For small, growing businesses.", false),
        new SubscriptionPlanDto("professional", "Professional", 49m, 25, "Advanced control and insights.", false),
        new SubscriptionPlanDto("premium", "Premium", 99m, null, "Unlimited widgets and full customization.", false)
    });

    public IReadOnlyList<SubscriptionPlanDto> GetPlans() => Plans;

    public Task<bool> EnsureFreeAsync(string owner, CancellationToken ct = default) =>
        _repository.EnsureFreeAsync(Owner(owner), ct);

    public Task<SubscriptionCurrentDto?> GetCurrentAsync(string owner, CancellationToken ct = default) =>
        _repository.GetCurrentAsync(Owner(owner), ct);

    public Task<IReadOnlyList<SubscriptionHistoryDto>> GetHistoryAsync(string owner, CancellationToken ct = default) =>
        _repository.GetHistoryAsync(Owner(owner), ct);
}

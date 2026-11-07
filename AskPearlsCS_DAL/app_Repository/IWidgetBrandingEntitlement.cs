namespace AskPearlsCS_DAL.app_Repository;

/// <summary>Implement against your real subscription/entitlement tables; do not trust client plan flags.</summary>
public interface IWidgetBrandingEntitlement
{
    Task<bool> CanUseWhiteLabelAsync(string ownerUserTokenId);
}

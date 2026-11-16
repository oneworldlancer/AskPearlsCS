using System.Text.RegularExpressions;
using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DOMAIN.app_ContractsManager;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_DAL.app_Repository;

public interface IWidgetDomain_Repository
{
    Task<WidgetOperationResult> GetDomainsAsync(string owner, string publicKey);
    Task<WidgetOperationResult> AddDomainAsync(string owner, string publicKey, string? domain);
    Task<WidgetOperationResult> DeleteDomainAsync(string owner, string publicKey, string? domain);
}

public sealed class WidgetDomain_Repository : IWidgetDomain_Repository
{
    private readonly DBContextModel _db;
    private const string Table = "dbo.ASKPEARLS_WIDGET_ALLOWED_DOMAIN_LIST";
    public WidgetDomain_Repository(DBContextModel db) => _db = db;

    private Task<bool> OwnsWidget(string owner, string key) =>
        _db.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking().AnyAsync(w =>
            w.OwnerUserTokenId == owner && w.WidgetPublicKey == key &&
            w.IsActive == true && w.IsVisible == true);

    private static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var s = value.Trim().TrimEnd('.').ToLowerInvariant();
        if (Uri.TryCreate(s, UriKind.Absolute, out var url))
        {
            if (url.Scheme is not ("http" or "https") || url.UserInfo.Length > 0 ||
                url.Port != (url.Scheme == "https" ? 443 : 80) || url.AbsolutePath != "/" ||
                !string.IsNullOrEmpty(url.Query) || !string.IsNullOrEmpty(url.Fragment))
                return null;
            s = url.IdnHost.ToLowerInvariant();
        }
        if (s.Length > 253 || !s.Contains('.') || s.Contains('*')) return null;
        var parts = s.Split('.');
        return parts.All(part => part.Length is >= 1 and <= 63 &&
            Regex.IsMatch(part, @"^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$") &&
            !part.Contains("--") || part.StartsWith("xn--", StringComparison.Ordinal) &&
            part.Length <= 63 && Regex.IsMatch(part, @"^[a-z0-9-]+$")) ? s : null;
    }

    private static WidgetOperationResult Result(WidgetOperationOutcome outcome, object body) => new(outcome, body);

    public async Task<WidgetOperationResult> GetDomainsAsync(string owner, string publicKey)
    {
        if (!await OwnsWidget(owner, publicKey))
            return Result(WidgetOperationOutcome.NotFound, new { success = false, message = "Widget not found." });
        var domains = await _db.Database.SqlQueryRaw<string>(
            "SELECT NormalizedDomain AS Value FROM " + Table +
            " WHERE OwnerUserTokenId = @owner AND WidgetPublicKey = @key ORDER BY NormalizedDomain",
            new SqlParameter("@owner", owner), new SqlParameter("@key", publicKey)).ToListAsync();
        return Result(WidgetOperationOutcome.Ok, new { success = true, domains });
    }

    public async Task<WidgetOperationResult> AddDomainAsync(string owner, string publicKey, string? value)
    {
        if (!await OwnsWidget(owner, publicKey))
            return Result(WidgetOperationOutcome.NotFound, new { success = false, message = "Widget not found." });
        var domain = Normalize(value);
        if (domain == null)
            return Result(WidgetOperationOutcome.Invalid, new { success = false, message = "Provide a valid hostname without a path, wildcard, or port." });
        try
        {
            await _db.Database.ExecuteSqlRawAsync(
                "INSERT INTO " + Table + " (WidgetPublicKey, OwnerUserTokenId, NormalizedDomain) VALUES (@key, @owner, @domain)",
                new SqlParameter("@key", publicKey), new SqlParameter("@owner", owner), new SqlParameter("@domain", domain));
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return Result(WidgetOperationOutcome.Conflict, new { success = false, message = "Domain already exists." });
        }
        return Result(WidgetOperationOutcome.Created, new { success = true, domain });
    }

    public async Task<WidgetOperationResult> DeleteDomainAsync(string owner, string publicKey, string? value)
    {
        if (!await OwnsWidget(owner, publicKey))
            return Result(WidgetOperationOutcome.NotFound, new { success = false, message = "Widget not found." });
        var domain = Normalize(value);
        if (domain == null)
            return Result(WidgetOperationOutcome.Invalid, new { success = false, message = "Invalid domain." });
        var affected = await _db.Database.ExecuteSqlRawAsync(
            "DELETE FROM " + Table + " WHERE WidgetPublicKey = @key AND OwnerUserTokenId = @owner AND NormalizedDomain = @domain",
            new SqlParameter("@key", publicKey), new SqlParameter("@owner", owner), new SqlParameter("@domain", domain));
        return affected == 0
            ? Result(WidgetOperationOutcome.NotFound, new { success = false, message = "Domain not found." })
            : Result(WidgetOperationOutcome.Ok, new { success = true, domain });
    }
}

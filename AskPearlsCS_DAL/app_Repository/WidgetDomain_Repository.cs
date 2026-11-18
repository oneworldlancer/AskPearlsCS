using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;

using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;
using System.Net.Http;
using AskPearlsCS_DOMAIN.app_ContractsManager;

using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_DAL.app_Repository;

public sealed class WidgetDomain_Repository(DBContextModel db, IHttpClientFactory clients) : IWidgetDomain_Repository
{
    private static WidgetOperationResult R(WidgetOperationOutcome outcome, object body) => new(outcome, body);
    private static string Token() => Guid.NewGuid().ToString("N");
    private static string? Host(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        var s = input.Trim().TrimEnd('.');
        if (s.Contains('/') || s.Contains('\\') || s.Contains(':') || s.Contains('@') || s.Contains('?') || s.Contains('#') || s.Contains('*')) return null;
        try { s = new IdnMapping().GetAscii(s).ToLowerInvariant(); } catch { return null; }
        if (s.Length > 253 || !s.Contains('.') || s.EndsWith(".local") || s.EndsWith(".internal")) return null;
        var labels = s.Split('.');
        if (labels.Any(x => x.Length < 1 || x.Length > 63 || x[0] == '-' || x[^1] == '-' || x.Any(c => !(char.IsAsciiLetterOrDigit(c) || c == '-')))) return null;
        if (labels[^1].All(char.IsDigit)) return null;
        return s;
    }
    private Task<bool> Owns(string owner, string key) => db.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking().AnyAsync(w => w.OwnerUserTokenId == owner && w.WidgetPublicKey == key && w.IsActive == true && w.IsVisible == true);
    private IQueryable<AskpearlsPortalUserAccountDomainList> Domains(string owner) => db.AskpearlsPortalUserAccountDomainLists.Where(x => x.OwnerUserTokenId == owner && x.IsActive == true && x.IsVisible == true);
    private IQueryable<AskpearlsPortalUserWebsiteWidgetDomainMapList> Maps(string owner, string key) => db.AskpearlsPortalUserWebsiteWidgetDomainMapLists.Where(x => x.OwnerUserTokenId == owner && x.WidgetPublicKey == key && x.IsActive == true && x.IsVisible == true);
    private async Task<DomainQuotaDto> Quota(string owner)
    {
        var subscription = await db.AskpearlsPortalUserSubscriptionLists.AsNoTracking().Where(x => x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false).OrderByDescending(x => x.UsubscribeId).FirstOrDefaultAsync();
        var plan = (subscription?.SubscriptionPlan ?? "free").Trim().ToLowerInvariant();
        if (plan.Length == 0 || plan == "0") plan = "free";
        var configured = await db.AskpearlsPortalApplicationSubscriptionPlanLimitLists.AsNoTracking().Where(x => x.SubscriptionPlanCode == plan && x.SubscriptionPlanLimitCode == "domains" && x.IsActive != false && x.IsVisible != false).Select(x => x.SubscriptionPlanLimitValue).FirstOrDefaultAsync();
        // Fail closed when plan limits have not been seeded; 0 is not unlimited.
        long? limit = configured is > 0 ? configured : 0;
        var used = await Domains(owner).LongCountAsync();
        return new(plan, used, limit, Math.Max(0, limit.Value - used));
    }
    public async Task<WidgetOperationResult> GetDomainsAsync(string owner, string key)
    {
        if (!await Owns(owner, key)) return R(WidgetOperationOutcome.NotFound, new { success = false, message = "Widget not found." });
        var mapped = await (from m in Maps(owner, key) join d in Domains(owner) on m.DomainTokenId equals d.DomainTokenId orderby d.DomainNormalized select new DomainEntryDto(d.DomainNormalized!, d.DomainTokenId!, d.VerificationStatus ?? "pending", d.VerifiedAtUtc)).ToListAsync();
        return R(WidgetOperationOutcome.Ok, new { success = true, domains = mapped, quota = await Quota(owner) });
    }
    public async Task<WidgetOperationResult> AddDomainAsync(string owner, string key, string? value)
    {
        if (!await Owns(owner, key)) return R(WidgetOperationOutcome.NotFound, new { success = false, message = "Widget not found." });
        var host = Host(value); if (host is null) return R(WidgetOperationOutcome.Invalid, new { success = false, message = "Enter a valid hostname, without scheme, port, wildcard or path." });
        // Serializable transaction protects quota and duplicate domain inserts within a single database.
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var domain = await Domains(owner).FirstOrDefaultAsync(d => d.DomainNormalized == host);
        if (domain is null)
        {
            var quota = await Quota(owner);
            if (quota.Limit is 0 || quota.Used >= quota.Limit) return R(WidgetOperationOutcome.Conflict, new { success = false, code = "PLAN_DOMAIN_LIMIT_REACHED", message = "Your plan's domain limit has been reached or plan limits are not configured." });
            domain = new AskpearlsPortalUserAccountDomainList { RowViewTokenId = Guid.NewGuid(), RowDataTokenId = Token(), OwnerUserTokenId = owner, DomainTokenId = Token(), DomainNormalized = host, DomainText = host, DomainCode = "website", DomainType = "hostname", VerificationCode = "pending", VerificationType = "dns_txt", VerificationText = "Pending verification", VerificationStatus = "pending", AccountType = "personal", IsActive = true, IsVisible = true, ActiveStatus = "active", CreatedAtUtc = DateTime.UtcNow };
            db.AskpearlsPortalUserAccountDomainLists.Add(domain);
        }
        var already = await Maps(owner, key).AnyAsync(m => m.DomainTokenId == domain.DomainTokenId);
        if (already) return R(WidgetOperationOutcome.Conflict, new { success = false, message = "Domain already assigned to this widget." });
        db.AskpearlsPortalUserWebsiteWidgetDomainMapLists.Add(new AskpearlsPortalUserWebsiteWidgetDomainMapList { RowViewTokenId = Guid.NewGuid(), RowDataTokenId = Token(), OwnerUserTokenId = owner, DomainTokenId = domain.DomainTokenId, WidgetDomainMapTokenId = Token(), WidgetPublicKey = key, AccountType = "personal", IsActive = true, IsVisible = true, ActiveStatus = "active", CreatedAtUtc = DateTime.UtcNow });
        await db.SaveChangesAsync(); await tx.CommitAsync();
        return R(WidgetOperationOutcome.Created, new { success = true, domain = host, domainTokenId = domain.DomainTokenId, verificationStatus = domain.VerificationStatus });
    }
    public async Task<WidgetOperationResult> DeleteDomainAsync(string owner, string key, string? value)
    {
        if (!await Owns(owner, key)) return R(WidgetOperationOutcome.NotFound, new { success = false, message = "Widget not found." });
        var host = Host(value); if (host is null) return R(WidgetOperationOutcome.Invalid, new { success = false, message = "Invalid domain." });
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var domain = await Domains(owner).FirstOrDefaultAsync(x => x.DomainNormalized == host);
        if (domain is null) return R(WidgetOperationOutcome.NotFound, new { success = false, message = "Domain not found." });
        var map = await Maps(owner, key).FirstOrDefaultAsync(x => x.DomainTokenId == domain.DomainTokenId);
        if (map is null) return R(WidgetOperationOutcome.NotFound, new { success = false, message = "Domain is not assigned to widget." });
        map.IsActive = false; map.IsVisible = false;
        var other = await db.AskpearlsPortalUserWebsiteWidgetDomainMapLists.AnyAsync(x => x.OwnerUserTokenId == owner && x.DomainTokenId == domain.DomainTokenId && x.WidgetDomainMapTokenId != map.WidgetDomainMapTokenId && x.IsActive == true && x.IsVisible == true);
        if (!other) { domain.IsActive = false; domain.IsVisible = false; }
        await db.SaveChangesAsync(); await tx.CommitAsync();
        return R(WidgetOperationOutcome.Ok, new { success = true, domain = host });
    }
    private async Task<AskpearlsPortalUserAccountDomainList?> Mapped(string owner, string key, string? value)
    {
        var host = Host(value); if (host is null || !await Owns(owner, key)) return null;
        return await (from d in Domains(owner) join m in Maps(owner, key) on d.DomainTokenId equals m.DomainTokenId where d.DomainNormalized == host select d).FirstOrDefaultAsync();
    }
    public async Task<WidgetOperationResult> BeginVerificationAsync(string owner, string key, string? value)
    {
        var domain = await Mapped(owner, key, value); if (domain is null) return R(WidgetOperationOutcome.NotFound, new { success = false, message = "Mapped domain not found." });
        var now = DateTime.UtcNow;
        var latest = await db.AskpearlsPortalUserAccountDomainVerificationLists.Where(x => x.OwnerUserTokenId == owner && x.DomainTokenId == domain.DomainTokenId && x.IsActive == true).OrderByDescending(x => x.UwebsiteWidgetDomainVerificationId).FirstOrDefaultAsync();
        if (latest is null || latest.ExpiresAtUtc <= now || latest.VerificationStatus == "verified")
        {
            var code = Convert.ToHexString(RandomNumberGenerator.GetBytes(24)).ToLowerInvariant();
            latest = new AskpearlsPortalUserAccountDomainVerificationList { RowViewTokenId = Guid.NewGuid(), RowDataTokenId = Token(), OwnerUserTokenId = owner, DomainTokenId = domain.DomainTokenId, VerificationTokenId = Token(), MethodCode = "dns_txt", MethodType = "dns", MethodText = "DNS TXT", VerificationCode = code, VerificationType = "dns_txt", VerificationText = "askpearls-verify=" + code, VerificationStatus = "pending", RequestedAtUtc = now, ExpiresAtUtc = now.AddDays(7), AttemptCount = 0, IsActive = true, IsVisible = true, ActiveStatus = "active", AccountType = "personal" };
            db.AskpearlsPortalUserAccountDomainVerificationLists.Add(latest);
            domain.VerificationStatus = "pending"; domain.VerifiedAtUtc = null;
            await db.SaveChangesAsync();
        }
        return R(WidgetOperationOutcome.Ok, new { success = true, domain = domain.DomainNormalized, verificationStatus = domain.VerificationStatus, dns = new { type = "TXT", host = "_askpearls." + domain.DomainNormalized, value = latest.VerificationText }, expiresAtUtc = latest.ExpiresAtUtc });
    }
    public async Task<WidgetOperationResult> VerifyDomainAsync(string owner, string key, string? value)
    {
        var domain = await Mapped(owner, key, value); if (domain is null) return R(WidgetOperationOutcome.NotFound, new { success = false, message = "Mapped domain not found." });
        var now = DateTime.UtcNow;
        var challenge = await db.AskpearlsPortalUserAccountDomainVerificationLists.Where(x => x.OwnerUserTokenId == owner && x.DomainTokenId == domain.DomainTokenId && x.IsActive == true && x.VerificationStatus == "pending").OrderByDescending(x => x.UwebsiteWidgetDomainVerificationId).FirstOrDefaultAsync();
        if (challenge is null || challenge.ExpiresAtUtc <= now) return R(WidgetOperationOutcome.Conflict, new { success = false, message = "No active DNS verification challenge. Request a new one." });
        if (challenge.LastCheckedAtUtc is { } last && last > now.AddSeconds(-20)) return R(WidgetOperationOutcome.Conflict, new { success = false, message = "Wait 20 seconds before checking again." });
        challenge.LastCheckedAtUtc = now; challenge.AttemptCount = (challenge.AttemptCount ?? 0) + 1;
        bool found = false;
        try
        {
            var client = clients.CreateClient("AskPearlsDnsVerification");
            using var response = await client.GetAsync("https://cloudflare-dns.com/dns-query?name=" + Uri.EscapeDataString("_askpearls." + domain.DomainNormalized) + "&type=TXT", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (doc.RootElement.TryGetProperty("Answer", out var answers) && answers.ValueKind == JsonValueKind.Array)
                foreach (var a in answers.EnumerateArray()) if (a.TryGetProperty("type", out var type) && type.GetInt32() == 16 && a.TryGetProperty("data", out var data))
                {
                    var txt = data.GetString()?.Replace("\\\"", "").Replace("\"", "").Replace(" ", "");
                    if (string.Equals(txt, challenge.VerificationText, StringComparison.Ordinal)) found = true;
                }
        }
        catch (HttpRequestException) { await db.SaveChangesAsync(); return R(WidgetOperationOutcome.Conflict, new { success = false, message = "DNS resolver unavailable. Try again later." }); }
        catch (JsonException) { await db.SaveChangesAsync(); return R(WidgetOperationOutcome.Conflict, new { success = false, message = "DNS response invalid. Try again later." }); }
        if (found) { domain.VerificationStatus = "verified"; domain.VerificationCode = "verified"; domain.VerifiedAtUtc = now; challenge.VerificationStatus = "verified"; challenge.VerifiedAtUtc = now; }
        else { challenge.LastFailureCode = "TXT_NOT_FOUND"; challenge.LastFailureMessage = "DNS TXT record not found or not propagated."; }
        await db.SaveChangesAsync();
        return R(found ? WidgetOperationOutcome.Ok : WidgetOperationOutcome.Conflict, new { success = found, domain = domain.DomainNormalized, verificationStatus = domain.VerificationStatus, message = found ? "Domain verified." : "DNS TXT record not found. Check DNS propagation." });
    }
}

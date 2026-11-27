using System.Net.Http.Headers;
using System.Text.Json;
using AskPearlsCS_DAL.app_Repository;
using AskPearlsCS_DOMAIN.app_ContractsManager;
using Microsoft.Extensions.Options;
namespace AskPearlsCS_BLL.app_Service;
public sealed class PaddleWebhook_Service(IPaddleWebhook_Repository repository, HttpClient client, IOptions<PaddleOptions> options)
{
    private static string? Str(JsonElement e, string key) => e.ValueKind == JsonValueKind.Object && e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    private static JsonElement Obj(JsonElement e, string key) => e.ValueKind == JsonValueKind.Object && e.TryGetProperty(key, out var v) ? v : default;
    private static DateTimeOffset? Date(JsonElement e, string key) => DateTimeOffset.TryParse(Str(e,key), out var d) ? d : null;
    private async Task<JsonDocument> Get(string path, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "https://sandbox-api.paddle.com/" + path);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.ApiKey);
        using var res = await client.SendAsync(req, ct); res.EnsureSuccessStatusCode();
        return JsonDocument.Parse(await res.Content.ReadAsStringAsync(ct));
    }
    public async Task<bool> ProcessOneAsync(CancellationToken ct)
    {
        if (options.Value.Environment != "Sandbox" || string.IsNullOrWhiteSpace(options.Value.ApiKey)) return false;
        var work = await repository.ClaimAsync(ct); if (work == null) return false;
        try
        {
            using var evt = JsonDocument.Parse(work.Payload);
            var type = Str(evt.RootElement,"event_type") ?? "";
            var data = Obj(evt.RootElement,"data");
            var id = type.StartsWith("subscription.", StringComparison.Ordinal) ? Str(data,"id") : null;
            if (id == null) { await repository.CompleteAsync(work,null,ct); return true; }
            if (!id.StartsWith("sub_",StringComparison.Ordinal) || !id.All(c => char.IsAsciiLetterOrDigit(c) || c == '_')) throw new InvalidOperationException("Invalid subscription ID.");
            using var doc = await Get("subscriptions/" + id,ct);
            var s = doc.RootElement.GetProperty("data");
            var items = s.GetProperty("items");
            if (items.GetArrayLength() != 1 || items[0].GetProperty("quantity").GetInt32() != 1) throw new InvalidOperationException("Unsupported subscription items.");
            var period = Obj(s,"current_billing_period"); var scheduled = Obj(s,"scheduled_change");
            var snapshot = new PaddleSubscriptionSnapshot(id, Str(s,"customer_id") ?? throw new InvalidOperationException(), Str(s,"status") ?? throw new InvalidOperationException(), Str(Obj(items[0],"price"),"id") ?? throw new InvalidOperationException(), Str(Obj(s,"custom_data"),"askpearls_checkout_id"), Date(period,"starts_at"), Date(period,"ends_at"), Date(s,"next_billed_at"), Str(scheduled,"action"), Date(scheduled,"effective_at"), Date(s,"updated_at") ?? throw new InvalidOperationException(), s.GetRawText());
            await repository.CompleteAsync(work,snapshot,ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch { await repository.FailAsync(work,ct); }
        return true;
    }
}

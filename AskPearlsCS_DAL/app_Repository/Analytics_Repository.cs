using AskPearlsCS_DAL.DBContext;

using AskPearlsCS_DOMAIN.app_ContractsManager;

using Microsoft.EntityFrameworkCore;
namespace AskPearlsCS_DAL.app_Repository;

public sealed class Analytics_Repository(DBContextModel db) : IAnalytics_Repository
{
    static string Kind(string? s)
    {
        s = (s ?? "").Trim().ToLowerInvariant(); return s switch
        {
            "started" or "start" or "session_started" or "widget_started" => "started",
            "completed" or "complete" or "session_completed" or "widget_completed" => "completed",
            "recommended" or "recommendation_shown" or "recommendation_displayed" => "recommended",
            "published" or "widget_published" => "published",
            _ => "interaction"
        };
    }
    static AnalyticsTotalsDto Totals(IEnumerable<(string Key, DateTime Date, string Kind)> sessions, IEnumerable<(string Key, DateTime Date, string Kind)> events)
    {
        long views = sessions.LongCount(), interactions = events.LongCount(), started = events.LongCount(x => x.Kind == "started"), completed = events.LongCount(x => x.Kind == "completed"), recommendations = events.LongCount(x => x.Kind == "recommended");
        // Session creation is a reliable start fallback if the CDN doesn't emit started events.
        started = Math.Max(started, views);
        return new(views, interactions, started, completed, recommendations, started == 0 ? 0 : Math.Round(100.0 * completed / started, 1));
    }
    public async Task<AnalyticsDashboardDto> GetAsync(string owner, int days, string? publicKey, string? activityType, CancellationToken ct)
    {
        var end = DateTime.UtcNow; var start = end.AddDays(-days); var previousStart = start.AddDays(-days);
        var owned = await db.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking()
         .Where(w => w.OwnerUserTokenId == owner && w.IsVisible == true)
         .Select(w => new { w.WidgetTokenId, w.WidgetPublicKey, w.WidgetName, w.WidgetStatusCode, w.WidgetStatus }).ToListAsync(ct);
        if (!string.IsNullOrWhiteSpace(publicKey)) owned = owned.Where(w => w.WidgetPublicKey == publicKey).ToList();
        var keys = owned.Select(w => w.WidgetTokenId).Where(x => x != null).Select(x => x!).ToArray();
        var sessions = await db.AskpearlsPortalUserWebsiteWidgetSessionLists.AsNoTracking()
         .Where(s => s.OwnerUserTokenId == owner && keys.Contains(s.WidgetTokenId!) && s.CreatedAtUtc >= previousStart && s.CreatedAtUtc <= end)
         .Select(s => new { s.WidgetTokenId, s.CreatedAtUtc, s.WidgetSessionTokenId }).ToListAsync(ct);
        var events = await db.AskpearlsPortalUserWebsiteWidgetEventLists.AsNoTracking()
         .Where(e => e.OwnerUserTokenId == owner && keys.Contains(e.WidgetTokenId!) && e.CreatedAtUtc >= previousStart && e.CreatedAtUtc <= end)
         .Select(e => new { e.WidgetTokenId, e.CreatedAtUtc, e.WidgetEventCode, e.WidgetEventType, e.WidgetEventTokenId }).ToListAsync(ct);
        var ss = sessions.Where(s => s.CreatedAtUtc.HasValue).Select(s => (Key: s.WidgetTokenId ?? "", Date: s.CreatedAtUtc!.Value, Kind: "started")).ToArray();
        var es = events.Where(e => e.CreatedAtUtc.HasValue).Select(e => (Key: e.WidgetTokenId ?? "", Date: e.CreatedAtUtc!.Value, Kind: Kind(e.WidgetEventCode ?? e.WidgetEventType))).ToArray();
        var currentS = ss.Where(x => x.Date >= start).ToArray(); var currentE = es.Where(x => x.Date >= start).ToArray();
        var previousS = ss.Where(x => x.Date < start).ToArray(); var previousE = es.Where(x => x.Date < start).ToArray();
        var widgets = owned.Select(w => {
            var ws = currentS.Where(s => s.Key == w.WidgetTokenId).ToArray(); var we = currentE.Where(e => e.Key == w.WidgetTokenId).ToArray(); var t = Totals(ws, we);
            return new AnalyticsWidgetDto(w.WidgetPublicKey ?? "", w.WidgetName ?? "Unnamed widget", w.WidgetStatusCode ?? w.WidgetStatus ?? "draft", t.Views, t.Interactions, t.Started, t.Completed, t.Recommendations, t.CompletionRate);
        }).ToArray();
        var chart = Enumerable.Range(0, days).Select(i => { var day = start.Date.AddDays(i); return new AnalyticsDayDto(day.ToString("yyyy-MM-dd"), currentS.LongCount(s => s.Date.Date == day), currentE.LongCount(e => e.Date.Date == day)); }).ToArray();
        var names = owned.Where(w => w.WidgetTokenId != null).ToDictionary(w => w.WidgetTokenId!, w => new { Key = w.WidgetPublicKey ?? "", Name = w.WidgetName ?? "Unnamed widget" });
        var activity = events.Where(e => e.CreatedAtUtc >= start).Select(e => new { e.WidgetTokenId, e.CreatedAtUtc, e.WidgetEventTokenId, Type = Kind(e.WidgetEventCode ?? e.WidgetEventType) })
         .Where(e => string.IsNullOrWhiteSpace(activityType) || activityType == "all" || e.Type == activityType)
         .OrderByDescending(e => e.CreatedAtUtc).Take(100).Select(e => {
             var w = names.GetValueOrDefault(e.WidgetTokenId ?? ""); var title = e.Type switch { "started" => "Widget started", "completed" => "Recommendation session completed", "recommended" => "Recommendation displayed", "published" => "Widget published", _ => "Widget interaction" };
             return new AnalyticsEventDto(e.WidgetEventTokenId ?? "", w?.Key ?? "", w?.Name ?? "Widget", e.Type, title, (w?.Name ?? "Widget") + " · " + title, e.CreatedAtUtc!.Value);
         }).ToArray();
        return new(Totals(currentS, currentE), Totals(previousS, previousE), widgets, chart, activity);
    }
}

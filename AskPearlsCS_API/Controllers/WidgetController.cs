//using AskPearls.Infrastructure;
using AskPearlsCS_BLL.app_Service;
using AskPearlsCS_BLL.DecisionOSManager;

using Microsoft.AspNetCore.Mvc;
using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AskPearlsCS_API.Controllers;

public sealed record WidgetDesignDraftRequest(
    [property: JsonPropertyName("primary")] string Primary,
    [property: JsonPropertyName("background")] string Background,
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("font")] string Font,
    [property: JsonPropertyName("fontSize")] int FontSize,
    [property: JsonPropertyName("radius")] int Radius,
    [property: JsonPropertyName("shadow")] bool Shadow,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("position")] string Position,
    [property: JsonPropertyName("buttonShape")] string ButtonShape,
    [property: JsonPropertyName("heading")] string Heading,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("buttonText")] string ButtonText,
    [property: JsonPropertyName("footer")] string Footer,
    [property: JsonPropertyName("theme")] string Theme = "custom");

public sealed record WidgetQuestionDraftRequest(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("helper")] string Helper,
    [property: JsonPropertyName("required")] bool Required,
    [property: JsonPropertyName("options")] IReadOnlyList<string> Options);

public sealed record WidgetQuestionsDraftRequest(
    [property: JsonPropertyName("questions")] IReadOnlyList<WidgetQuestionDraftRequest> Questions);

public sealed record RecommendationDraftItemRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("imageUrl")] string? ImageUrl,
    [property: JsonPropertyName("url")] string? Url,
    [property: JsonPropertyName("ctaText")] string? CtaText,
    [property: JsonPropertyName("active")] bool Active);

public sealed record RecommendationDraftRequest(
    [property: JsonPropertyName("items")] IReadOnlyList<RecommendationDraftItemRequest> Items);

public sealed record DecisionRuleConditionDraftRequest(
    [property: JsonPropertyName("questionKey")] string QuestionKey,
    [property: JsonPropertyName("questionText")] string QuestionText,
    [property: JsonPropertyName("answerKey")] string AnswerKey,
    [property: JsonPropertyName("answerText")] string AnswerText);

public sealed record DecisionRuleDraftItemRequest(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("recommendationKey")] string RecommendationKey,
    [property: JsonPropertyName("recommendationName")] string RecommendationName,
    [property: JsonPropertyName("conditionLogic")] string ConditionLogic,
    [property: JsonPropertyName("priority")] int Priority,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("conditions")] IReadOnlyList<DecisionRuleConditionDraftRequest> Conditions);

public sealed record DecisionRulesDraftRequest(
    [property: JsonPropertyName("rules")] IReadOnlyList<DecisionRuleDraftItemRequest> Rules);

public sealed record CreateWidgetRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("websiteTokenId")] string WebsiteTokenId,
    [property: JsonPropertyName("websiteName")] string WebsiteName,
    [property: JsonPropertyName("websiteAddress")] string WebsiteAddress,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description);

[ApiController]
[Route("api/widgets")]
public class WidgetController : ControllerBase
//public class WidgetController(DBContextModel db) : ControllerBase
{

    private readonly IWidget_Service _widgetService;
    private readonly IWidgetVersion_Service _widgetVersionService;
    private readonly IQuestion_Service _questionService;
    private readonly DBContextModel _db;
    private readonly IWebHostEnvironment _environment;

    public WidgetController(
       IWidget_Service widgetService,
       IWidgetVersion_Service widgetVersionService,
       IQuestion_Service questionService,
       DBContextModel db,
       IWebHostEnvironment environment)
    {
        _widgetService = widgetService;
        _widgetVersionService = widgetVersionService;
        _questionService = questionService;
        _db = db;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWidgets()
    {
        if (!TryGetDemoOwner(out var error)) return error!;

        var widgets = await _db.AskpearlsPortalUserWebsiteWidgetLists
            .AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                        x.IsActive == true && x.IsVisible == true)
            .OrderBy(x => x.WidgetName)
            .Select(x => new
            {
                x.WidgetTokenId,
                x.WidgetPublicKey,
                x.WidgetName,
                x.WidgetTitle,
                x.WidgetDescription,
                x.WebsiteTokenId,
                x.WebsiteName,
                x.WebsiteAddress,
                x.WidgetStatusCode,
                x.WidgetPublishedVersion
            })
            .ToListAsync();

        var widgetKeys = widgets
            .Where(x => !string.IsNullOrWhiteSpace(x.WidgetPublicKey))
            .Select(x => x.WidgetPublicKey!)
            .ToArray();

        var versions = widgetKeys.Length == 0
            ? new List<AskpearlsPortalUserWebsiteWidgetVersionList>()
            : await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
                .AsNoTracking()
                .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                            widgetKeys.Contains(x.WidgetPublicKey!) &&
                            x.IsActive == true && x.IsVisible == true)
                .Select(x => new AskpearlsPortalUserWebsiteWidgetVersionList
                {
                    WidgetPublicKey = x.WidgetPublicKey,
                    WidgetVersionNumber = x.WidgetVersionNumber,
                    WidgetStatusCode = x.WidgetStatusCode,
                    UpdatedAtUtc = x.UpdatedAtUtc,
                    CreatedAtUtc = x.CreatedAtUtc
                })
                .ToListAsync();

        var questions = await _db.AskpearlsPortalUserWebsiteWidgetQuestionLists
            .AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                        widgetKeys.Contains(x.WidgetPublicKey!) &&
                        x.IsActive == true && x.IsVisible == true)
            .Select(x => new { x.WidgetPublicKey, x.WidgetVersionNumber })
            .ToListAsync();

        var recommendations = await _db.AskpearlsPortalUserWebsiteWidgetRecommendationLists
            .AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                        widgetKeys.Contains(x.WidgetPublicKey!) &&
                        x.IsActive == true && x.IsVisible == true)
            .Select(x => new { x.WidgetPublicKey, x.WidgetVersionNumber })
            .ToListAsync();

        var result = widgets.Select(widget =>
        {
            var widgetVersions = versions
                .Where(version => version.WidgetPublicKey == widget.WidgetPublicKey)
                .ToList();
            var draft = widgetVersions
                .Where(version => string.Equals(version.WidgetStatusCode, "draft", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(version => version.UpdatedAtUtc ?? version.CreatedAtUtc)
                .FirstOrDefault();
            var published = widgetVersions
                .Where(version => string.Equals(version.WidgetStatusCode, "published", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(version => version.WidgetVersionNumber == widget.WidgetPublishedVersion)
                .ThenByDescending(version => version.UpdatedAtUtc ?? version.CreatedAtUtc)
                .FirstOrDefault();

            var status = draft is not null
                ? "draft"
                : published is not null
                    ? "published"
                    : (widget.WidgetStatusCode ?? "draft").ToLowerInvariant();
            var currentVersion = draft ?? published;
            var currentVersionNumber = currentVersion?.WidgetVersionNumber ?? widget.WidgetPublishedVersion;
            var currentPublicKey = widget.WidgetPublicKey;
            var questionCount = questions.Count(x =>
                string.Equals(x.WidgetPublicKey, currentPublicKey, StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrWhiteSpace(currentVersionNumber) ||
                 string.Equals(x.WidgetVersionNumber, currentVersionNumber, StringComparison.OrdinalIgnoreCase)));
            var recommendationCount = recommendations.Count(x =>
                string.Equals(x.WidgetPublicKey, currentPublicKey, StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrWhiteSpace(currentVersionNumber) ||
                 string.Equals(x.WidgetVersionNumber, currentVersionNumber, StringComparison.OrdinalIgnoreCase)));

            return new
            {
                publicKey = widget.WidgetPublicKey,
                name = widget.WidgetName ?? widget.WidgetTitle ?? "Untitled widget",
                description = widget.WidgetDescription ?? string.Empty,
                status,
                versionNumber = currentVersionNumber,
                draftVersionNumber = draft?.WidgetVersionNumber,
                publishedVersionNumber = published?.WidgetVersionNumber ?? widget.WidgetPublishedVersion,
                hasDraft = draft is not null,
                hasPublishedVersion = published is not null || !string.IsNullOrWhiteSpace(widget.WidgetPublishedVersion),
                updatedAt = currentVersion?.UpdatedAtUtc ?? currentVersion?.CreatedAtUtc,
                questionCount,
                recommendationCount
            };
        });

        return Ok(new { success = true, widgets = result });
    }

    [HttpGet("websites")]
    public async Task<IActionResult> GetMyWebsites()
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        var websites = await _db.AskpearlsPortalUserWebsiteLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true && x.IsVisible == true)
            .OrderBy(x => x.WebsiteName)
            .Select(x => new { websiteTokenId = x.WebsiteTokenId, name = x.WebsiteName, address = x.WebsiteAddress })
            .ToListAsync();
        return Ok(new { success = true, websites });
    }

    [HttpPost]
    public async Task<IActionResult> CreateWidget([FromBody] CreateWidgetRequest? request)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        if (request is null || string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 100 ||
            request.Type != "decision" || string.IsNullOrWhiteSpace(request.WebsiteTokenId) ||
            string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length > 120 ||
            request.Description?.Length > 500)
            return BadRequest(new { success = false, message = "Enter a widget name, supported type, target website, title, and description of up to 500 characters." });

        AskpearlsPortalUserWebsiteList? website = null;
        if (!string.IsNullOrWhiteSpace(request.WebsiteTokenId))
        {
            website = await _db.AskpearlsPortalUserWebsiteLists.FirstOrDefaultAsync(x =>
                x.WebsiteTokenId == request.WebsiteTokenId && x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                x.IsActive == true && x.IsVisible == true);
            if (website is null)
                return BadRequest(new { success = false, message = "Choose a website belonging to the current portal account." });
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.WebsiteAddress) || request.WebsiteAddress.Length > 250 ||
                !Uri.TryCreate(request.WebsiteAddress, UriKind.Absolute, out var siteUri) ||
                siteUri.Scheme is not ("http" or "https") || string.IsNullOrWhiteSpace(siteUri.Host))
                return BadRequest(new { success = false, message = "Enter a valid website address beginning with http:// or https://." });
            website = await _db.AskpearlsPortalUserWebsiteLists.FirstOrDefaultAsync(x =>
                x.OwnerUserTokenId == DemoOwnerUserTokenId && x.WebsiteAddress == request.WebsiteAddress &&
                x.IsActive == true && x.IsVisible == true);
            if (website is null)
            {
                website = new AskpearlsPortalUserWebsiteList
                {
                    RowViewTokenId = Guid.NewGuid(),
                    RowDataTokenId = Guid.NewGuid().ToString("N"),
                    AccountType = "portal",
                    OwnerUserTokenId = DemoOwnerUserTokenId,
                    WebsiteTokenId = Guid.NewGuid().ToString("N"),
                    WebsiteName = string.IsNullOrWhiteSpace(request.WebsiteName) ? siteUri.Host : request.WebsiteName.Trim(),
                    WebsiteAddress = request.WebsiteAddress.Trim(),
                    ActiveStatus = "active",
                    IsNew = true,
                    IsActive = true,
                    IsVisible = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };
                _db.AskpearlsPortalUserWebsiteLists.Add(website);
            }
        }

        var now = DateTime.UtcNow;
        var widgetToken = Guid.NewGuid().ToString("N");
        var publicKey = "pk_" + Guid.NewGuid().ToString("N");
        var versionToken = Guid.NewGuid().ToString("N");
        var widget = new AskpearlsPortalUserWebsiteWidgetList
        {
            RowViewTokenId = Guid.NewGuid(),
            RowDataTokenId = Guid.NewGuid().ToString("N"),
            AccountType = website.AccountType,
            OwnerUserTokenId = DemoOwnerUserTokenId,
            OwnerMobileNumberTokenId = website.OwnerMobileNumberTokenId,
            OrganizationTokenId = website.OrganizationTokenId,
            CompanyTokenId = website.CompanyTokenId,
            CompanyName = website.CompanyName,
            WidgetTokenId = widgetToken,
            WidgetCode = "decisionos",
            WidgetType = "decision",
            WidgetName = request.Name.Trim(),
            WidgetTitle = request.Title.Trim(),
            WidgetDescription = (request.Description ?? string.Empty).Trim(),
            WebsiteTokenId = website.WebsiteTokenId,
            WebsiteName = website.WebsiteName,
            WebsiteAddress = website.WebsiteAddress,
            WidgetPublicKey = publicKey,
            WidgetStatusCode = "draft",
            WidgetStatusText = "draft",
            WidgetStatus = "draft",
            WidgetPublishedVersion = null,
            ActiveStatus = "pending",
            IsNew = true,
            IsActive = true,
            IsVisible = true
        };
        var design = new WidgetDesignDraftRequest(
            "#0DA88B", "#FFFFFF", "#17212B", "Inter, Arial, sans-serif", 14, 12, true, 360,
            "bottom-right", "rounded", request.Title.Trim(), (request.Description ?? string.Empty).Trim(),
            "Get started", "Powered by AskPearls", "custom");
        var version = new AskpearlsPortalUserWebsiteWidgetVersionList
        {
            RowViewTokenId = Guid.NewGuid(),
            RowDataTokenId = Guid.NewGuid().ToString("N"),
            AccountType = widget.AccountType,
            OwnerUserTokenId = widget.OwnerUserTokenId,
            OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
            OrganizationTokenId = widget.OrganizationTokenId,
            CompanyTokenId = widget.CompanyTokenId,
            CompanyName = widget.CompanyName,
            WidgetTokenId = widgetToken,
            WebsiteTokenId = website.WebsiteTokenId,
            WebsiteName = website.WebsiteName,
            WidgetPublicKey = publicKey,
            WidgetVersionTokenId = versionToken,
            WidgetVersionNumber = "1",
            WidgetConfigurationJson = JsonSerializer.Serialize(design),
            WidgetStatusCode = "draft",
            WidgetStatusText = "draft",
            WidgetStatus = "draft",
            ActiveStatus = "pending",
            IsNew = true,
            IsActive = true,
            IsVisible = true,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
        _db.AskpearlsPortalUserWebsiteWidgetLists.Add(widget);
        _db.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(version);
        await _db.SaveChangesAsync();
        return Created($"/api/widgets/{publicKey}", new
        {
            success = true,
            publicKey,
            name = widget.WidgetName,
            status = "draft",
            versionNumber = version.WidgetVersionNumber,
            websiteTokenId = website.WebsiteTokenId
        });
    }

    // Local demo identity from the user's test data. These routes are disabled
    // outside Development and must be replaced by the portal's Google login model.
    private const string DemoOwnerUserTokenId = "1101";

    [HttpPut("{publicKey}/draft/configuration")]
    public async Task<IActionResult> SaveDesignDraft(
        string publicKey,
        [FromBody] WidgetDesignDraftRequest? request)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        if (string.IsNullOrWhiteSpace(publicKey) || !IsValidDesign(request))
            return BadRequest(new { success = false, message = "Widget key or design settings are invalid." });

        var widget = await _db.AskpearlsPortalUserWebsiteWidgetLists
            .FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
                                      x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                                      x.IsActive == true && x.IsVisible == true);
        if (widget is null)
            return NotFound(new { success = false, message = "Widget was not found for the demo owner." });

        var versions = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey)
            .ToListAsync();
        var drafts = versions.Where(x => x.WidgetStatusCode == "draft" &&
                                         x.IsActive == true && x.IsVisible == true).ToList();
        if (drafts.Count > 1)
            return Conflict(new { success = false, message = "More than one active draft exists for this widget." });

        var json = JsonSerializer.Serialize(request);
        var draft = drafts.SingleOrDefault();
        if (draft is null)
        {
            var publishedVersions = versions.Where(x => x.WidgetStatusCode == "published" &&
                                                        x.IsActive == true && x.IsVisible == true).ToList();
            if (publishedVersions.Count != 1 || string.IsNullOrWhiteSpace(publishedVersions[0].WidgetVersionTokenId))
                return Conflict(new { success = false, message = "A single published content version is required before saving a design draft." });

            var publishedNumber = int.TryParse(widget.WidgetPublishedVersion, out var n) ? n : 0;
            var maxNumber = versions.Select(x => int.TryParse(x.WidgetVersionNumber, out var v) ? v : 0)
                                     .DefaultIfEmpty(0).Max();
            var number = Math.Max(publishedNumber, maxNumber) + 1;
            draft = new AskpearlsPortalUserWebsiteWidgetVersionList
            {
                AccountType = widget.AccountType,
                OwnerUserTokenId = widget.OwnerUserTokenId,
                OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
                OrganizationTokenId = widget.OrganizationTokenId,
                CompanyTokenId = widget.CompanyTokenId,
                CompanyName = widget.CompanyName,
                WidgetTokenId = widget.WidgetTokenId,
                WebsiteTokenId = widget.WebsiteTokenId,
                WebsiteName = widget.WebsiteName,
                WidgetPublicKey = widget.WidgetPublicKey,
                // Design-only drafts reuse the published content token so the
                // existing questions, answers, rules and recommendations remain linked.
                WidgetVersionTokenId = publishedVersions[0].WidgetVersionTokenId,
                WidgetVersionNumber = number.ToString(),
                WidgetConfigurationJson = json,
                WidgetStatusCode = "draft",
                WidgetStatusText = "draft",
                WidgetStatus = "draft",
                WidgetPublishedVersion = widget.WidgetPublishedVersion,
                ActiveStatus = "pending",
                IsNew = true,
                IsActive = true,
                IsVisible = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };
            _db.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(draft);
        }
        else
        {
            draft.WidgetConfigurationJson = json;
            draft.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return Ok(new { success = true, status = "draft", versionNumber = draft.WidgetVersionNumber });
    }

    [HttpPost("{publicKey}/publish")]
    public async Task<IActionResult> PublishDesignDraft(string publicKey)
    {
        if (!TryGetDemoOwner(out var error)) return error!;

        var widget = await _db.AskpearlsPortalUserWebsiteWidgetLists
            .FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
                                      x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                                      x.IsActive == true && x.IsVisible == true);
        if (widget is null)
            return NotFound(new { success = false, message = "Widget was not found for the demo owner." });

        var drafts = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetStatusCode == "draft" &&
                        x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        if (drafts.Count != 1)
            return Conflict(new
            {
                success = false,
                message = drafts.Count == 0
                ? "Save a draft before publishing." : "More than one active draft exists."
            });

        var draft = drafts[0];
        if (!IsValidStoredDesign(draft.WidgetConfigurationJson))
            return Conflict(new { success = false, message = "Draft configuration is missing or invalid." });

        await using var transaction = await _db.Database.BeginTransactionAsync();
        var published = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetStatusCode == "published" &&
                        x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        foreach (var previous in published)
        {
            previous.WidgetStatusCode = "archived";
            previous.WidgetStatusText = "archived";
            previous.WidgetStatus = "archived";
            previous.UpdatedAtUtc = DateTime.UtcNow;
        }

        draft.WidgetStatusCode = "published";
        draft.WidgetStatusText = "published";
        draft.WidgetStatus = "published";
        draft.WidgetPublishedVersion = draft.WidgetVersionNumber;
        draft.IsNew = false;
        draft.UpdatedAtUtc = DateTime.UtcNow;
        widget.WidgetPublishedVersion = draft.WidgetVersionNumber;
        widget.WidgetStatusCode = "published";
        widget.WidgetStatusText = "published";
        widget.WidgetStatus = "published";

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return Ok(new { success = true, status = "published", versionNumber = draft.WidgetVersionNumber });
    }

    [HttpGet("{publicKey}/draft/configuration")]
    public async Task<IActionResult> GetDesignDraft(string publicKey)
    {
        if (!TryGetDemoOwner(out var error)) return error!;

        var widget = await _db.AskpearlsPortalUserWebsiteWidgetLists
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
                                      x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                                      x.IsActive == true && x.IsVisible == true);
        if (widget is null)
            return NotFound(new { success = false, message = "Widget was not found for the demo owner." });

        var draft = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetStatusCode == "draft" &&
                        x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UwebsiteWidgetVersionId)
            .FirstOrDefaultAsync();

        var version = draft ?? await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.WidgetTokenId == widget.WidgetTokenId &&
                                      x.WidgetPublicKey == publicKey &&
                                      x.WidgetVersionNumber == widget.WidgetPublishedVersion &&
                                      x.WidgetStatusCode == "published" &&
                                      x.IsActive == true && x.IsVisible == true);

        if (string.IsNullOrWhiteSpace(version?.WidgetConfigurationJson))
            return Ok(new
            {
                success = true,
                status = draft is null ? "published" : "draft",
                versionNumber = version?.WidgetVersionNumber,
                configuration = (object?)null
            });

        try
        {
            using var document = JsonDocument.Parse(version.WidgetConfigurationJson);
            return Ok(new
            {
                success = true,
                status = version.WidgetStatusCode,
                versionNumber = version.WidgetVersionNumber,
                configuration = document.RootElement.Clone()
            });
        }
        catch (JsonException)
        {
            return Conflict(new { success = false, message = "The saved widget design JSON is invalid." });
        }
    }

    [HttpGet("{publicKey}/draft/questions")]
    public async Task<IActionResult> GetQuestionDraft(string publicKey)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        var widget = await FindDemoWidget(publicKey);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for the demo owner." });

        var draftVersion = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey &&
                        x.WidgetStatusCode == "draft" && x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UpdatedAtUtc ?? x.CreatedAtUtc)
            .FirstOrDefaultAsync();

        if (draftVersion is null || string.IsNullOrWhiteSpace(draftVersion.WidgetVersionTokenId))
        {
            var publishedQuestions = await _questionService.GetPublishedQuestionsAsync(publicKey);
            return Ok(new
            {
                success = true,
                status = "published",
                versionNumber = widget.WidgetPublishedVersion,
                questions = publishedQuestions.Select((q, i) => new
                {
                    id = q.QuestionTokenId,
                    text = q.Text,
                    key = q.Key,
                    helper = q.Description ?? string.Empty,
                    required = true,
                    options = q.Answers.Select(a => a.Text).ToArray(),
                    sortOrder = i + 1
                })
            });
        }

        var rows = await _db.AskpearlsPortalUserWebsiteWidgetQuestionLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetVersionTokenId == draftVersion.WidgetVersionTokenId &&
                        x.IsActive == true && x.IsVisible == true)
            .OrderBy(x => x.WidgetQuestionSortOrder).ToListAsync();
        var ids = rows.Select(x => x.WidgetQuestionTokenId).Where(x => x != null).ToArray();
        var answers = await _db.AskpearlsPortalUserWebsiteWidgetQuestionAnswerLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.WidgetPublicKey == publicKey &&
                        x.WidgetVersionTokenId == draftVersion.WidgetVersionTokenId && ids.Contains(x.WidgetQuestionTokenId) &&
                        x.IsActive == true && x.IsVisible == true)
            .OrderBy(x => x.WidgetAnswerSortOrder).ToListAsync();

        return Ok(new
        {
            success = true,
            status = "draft",
            versionNumber = draftVersion.WidgetVersionNumber,
            questions = rows.Select((q, i) => new
            {
                id = q.WidgetQuestionTokenId,
                text = q.WidgetQuestionText ?? q.WidgetQuestionTitle ?? string.Empty,
                key = q.WidgetQuestionKey ?? string.Empty,
                helper = q.WidgetQuestionDescription ?? string.Empty,
                required = ReadRequired(q.WidgetConfigurationJson),
                options = answers.Where(a => a.WidgetQuestionTokenId == q.WidgetQuestionTokenId)
                                 .Select(a => a.WidgetAnswerText ?? a.WidgetAnswerTitle ?? string.Empty).ToArray(),
                sortOrder = q.WidgetQuestionSortOrder ?? i + 1
            })
        });
    }

    [HttpPut("{publicKey}/draft/questions")]
    public async Task<IActionResult> SaveQuestionDraft(string publicKey, [FromBody] WidgetQuestionsDraftRequest? request)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        var widget = await FindDemoWidget(publicKey);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for the demo owner." });
        if (!IsValidQuestionDraft(request))
            return BadRequest(new { success = false, message = "Questions are invalid. Each question needs a unique key, text, and at least two distinct options." });

        var versions = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey && x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        var drafts = versions.Where(x => x.WidgetStatusCode == "draft").ToList();
        if (drafts.Count > 1) return Conflict(new { success = false, message = "More than one active draft exists for this widget." });
        var published = versions.Where(x => x.WidgetStatusCode == "published" && x.WidgetVersionNumber == widget.WidgetPublishedVersion).ToList();
        if (published.Count > 1 || (published.Count == 0 && drafts.Count == 0))
            return Conflict(new { success = false, message = "A valid widget draft or published version is required before saving questions." });

        var draft = drafts.SingleOrDefault();
        var now = DateTime.UtcNow;
        if (draft is null)
        {
            var maxNumber = versions.Select(x => int.TryParse(x.WidgetVersionNumber, out var n) ? n : 0).DefaultIfEmpty(0).Max();
            draft = new AskpearlsPortalUserWebsiteWidgetVersionList
            {
                AccountType = widget.AccountType,
                OwnerUserTokenId = widget.OwnerUserTokenId,
                OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
                OrganizationTokenId = widget.OrganizationTokenId,
                CompanyTokenId = widget.CompanyTokenId,
                CompanyName = widget.CompanyName,
                WidgetTokenId = widget.WidgetTokenId,
                WebsiteTokenId = widget.WebsiteTokenId,
                WebsiteName = widget.WebsiteName,
                WidgetPublicKey = widget.WidgetPublicKey,
                WidgetVersionNumber = (maxNumber + 1).ToString(),
                WidgetStatusCode = "draft",
                WidgetStatusText = "draft",
                WidgetStatus = "draft",
                WidgetPublishedVersion = widget.WidgetPublishedVersion,
                ActiveStatus = "pending",
                IsNew = true,
                IsActive = true,
                IsVisible = true,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
            draft.WidgetVersionTokenId = published.Count == 1 && !string.IsNullOrWhiteSpace(published[0].WidgetVersionTokenId)
                ? published[0].WidgetVersionTokenId
                : Guid.NewGuid().ToString("N");
            _db.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(draft);
        }
        // Never share the published content token with editable question rows.
        // Also detach a design-only draft token, if one was created earlier.
        var publishedTokenId = published.SingleOrDefault()?.WidgetVersionTokenId;
        if (!string.IsNullOrWhiteSpace(draft.WidgetVersionTokenId) &&
            draft.WidgetVersionTokenId != publishedTokenId)
        {
            var oldQuestions = await _db.AskpearlsPortalUserWebsiteWidgetQuestionLists
                .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.WidgetPublicKey == publicKey &&
                            x.WidgetVersionTokenId == draft.WidgetVersionTokenId && x.IsActive == true)
                .ToListAsync();
            var oldAnswers = await _db.AskpearlsPortalUserWebsiteWidgetQuestionAnswerLists
                .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.WidgetPublicKey == publicKey &&
                            x.WidgetVersionTokenId == draft.WidgetVersionTokenId && x.IsActive == true)
                .ToListAsync();
            foreach (var row in oldQuestions) { row.IsActive = false; row.IsVisible = false; row.UpdatedAtUtc = now; }
            foreach (var row in oldAnswers) { row.IsActive = false; row.IsVisible = false; row.UpdatedAtUtc = now; }
        }
        draft.WidgetVersionTokenId = Guid.NewGuid().ToString("N");
        draft.UpdatedAtUtc = now;

        var questionRows = request!.Questions.Select((q, index) => new AskpearlsPortalUserWebsiteWidgetQuestionList
        {
            RowViewTokenId = Guid.NewGuid(),
            RowDataTokenId = Guid.NewGuid().ToString("N"),
            AccountType = widget.AccountType,
            OwnerUserTokenId = widget.OwnerUserTokenId,
            OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
            OrganizationTokenId = widget.OrganizationTokenId,
            CompanyTokenId = widget.CompanyTokenId,
            CompanyName = widget.CompanyName,
            WidgetTokenId = widget.WidgetTokenId,
            WebsiteTokenId = widget.WebsiteTokenId,
            WidgetQuestionTokenId = Guid.NewGuid().ToString("N"),
            WidgetQuestionCode = q.Key,
            WidgetQuestionType = "choice",
            WidgetQuestionKey = q.Key,
            WidgetQuestionTitle = q.Text,
            WidgetQuestionName = q.Key,
            WidgetQuestionText = q.Text,
            WidgetQuestionDescription = q.Helper,
            WidgetQuestionSortOrder = index + 1,
            WebsiteName = widget.WebsiteName,
            WidgetPublicKey = publicKey,
            WidgetVersionTokenId = draft.WidgetVersionTokenId,
            WidgetVersionNumber = draft.WidgetVersionNumber,
            WidgetConfigurationJson = JsonSerializer.Serialize(new { required = q.Required }),
            WidgetStatusCode = "draft",
            WidgetStatusText = "draft",
            WidgetStatus = "draft",
            WidgetPublishedVersion = widget.WidgetPublishedVersion,
            IsNew = true,
            ActiveStatus = "pending",
            IsActive = true,
            IsVisible = true,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        }).ToList();
        _db.AskpearlsPortalUserWebsiteWidgetQuestionLists.AddRange(questionRows);

        var answerRows = questionRows.SelectMany((question, index) => request.Questions[index].Options.Select((option, optionIndex) =>
            new AskpearlsPortalUserWebsiteWidgetQuestionAnswerList
            {
                RowViewTokenId = Guid.NewGuid(),
                RowDataTokenId = Guid.NewGuid().ToString("N"),
                AccountType = widget.AccountType,
                OwnerUserTokenId = widget.OwnerUserTokenId,
                OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
                OrganizationTokenId = widget.OrganizationTokenId,
                CompanyTokenId = widget.CompanyTokenId,
                CompanyName = widget.CompanyName,
                WidgetTokenId = widget.WidgetTokenId,
                WidgetQuestionTokenId = question.WidgetQuestionTokenId,
                WidgetQuestionCode = question.WidgetQuestionCode,
                WidgetQuestionType = question.WidgetQuestionType,
                WidgetQuestionKey = question.WidgetQuestionKey,
                WidgetAnswerTokenId = Guid.NewGuid().ToString("N"),
                WidgetAnswerCode = "option_" + (optionIndex + 1),
                WidgetAnswerType = "option",
                WidgetAnswerKey = "option_" + (optionIndex + 1),
                WidgetAnswerTitle = option,
                WidgetAnswerName = option,
                WidgetAnswerText = option,
                WidgetAnswerSortOrder = optionIndex + 1,
                WebsiteTokenId = widget.WebsiteTokenId,
                WebsiteName = widget.WebsiteName,
                WidgetPublicKey = publicKey,
                WidgetVersionTokenId = draft.WidgetVersionTokenId,
                WidgetVersionNumber = draft.WidgetVersionNumber,
                WidgetStatus = "draft",
                WidgetPublishedVersion = widget.WidgetPublishedVersion,
                IsNew = true,
                ActiveStatus = "pending",
                IsActive = true,
                IsVisible = true,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            })).ToList();
        _db.AskpearlsPortalUserWebsiteWidgetQuestionAnswerLists.AddRange(answerRows);
        await _db.SaveChangesAsync();
        return Ok(new { success = true, status = "draft", versionNumber = draft.WidgetVersionNumber, questionCount = questionRows.Count });
    }

    [HttpGet("{publicKey}/draft/recommendations")]
    public async Task<IActionResult> GetRecommendationDraft(string publicKey)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        var widget = await FindDemoWidget(publicKey);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for the demo owner." });

        var versions = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey &&
                        x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        var version = versions.Where(x => x.WidgetStatusCode == "draft")
            .OrderByDescending(x => x.UpdatedAtUtc ?? x.CreatedAtUtc).FirstOrDefault()
            ?? versions.FirstOrDefault(x => x.WidgetStatusCode == "published" && x.WidgetVersionNumber == widget.WidgetPublishedVersion);
        if (version is null) return Conflict(new { success = false, message = "A published or draft widget version is required." });

        // Resolve recommendations using both version number and token. Existing
        // portal records are not all populated consistently across these fields.
        var widgetRows = await _db.AskpearlsPortalUserWebsiteWidgetRecommendationLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey && x.IsVisible == true)
            .ToListAsync();
        bool BelongsToVersion(AskpearlsPortalUserWebsiteWidgetRecommendationList row, AskpearlsPortalUserWebsiteWidgetVersionList target) =>
            (!string.IsNullOrWhiteSpace(target.WidgetVersionTokenId) && row.WidgetVersionTokenId == target.WidgetVersionTokenId) ||
            (!string.IsNullOrWhiteSpace(target.WidgetVersionNumber) && row.WidgetVersionNumber == target.WidgetVersionNumber);
        var sourceVersion = version;
        var rows = widgetRows.Where(row => BelongsToVersion(row, version)).ToList();
        if (rows.Count == 0 && string.Equals(version.WidgetStatusCode, "draft", StringComparison.OrdinalIgnoreCase))
        {
            var liveVersion = versions.FirstOrDefault(x => x.WidgetStatusCode == "published" && x.WidgetVersionNumber == widget.WidgetPublishedVersion);
            if (liveVersion is not null)
            {
                var liveRows = widgetRows.Where(row => BelongsToVersion(row, liveVersion)).ToList();
                if (liveRows.Count > 0) { rows = liveRows; sourceVersion = liveVersion; }
            }
        }
        if (rows.Count == 0)
            rows = widgetRows.Where(row => string.IsNullOrWhiteSpace(row.WidgetVersionTokenId) &&
                string.IsNullOrWhiteSpace(row.WidgetVersionNumber)).ToList();
        rows = rows.OrderBy(x => x.RecommendationSortOrder).ThenBy(x => x.RecommendationName).ToList();

        var usedRecommendationKeys = await _db.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey &&
                        x.IsVisible == true && x.RecommendationKey != null)
            .Select(x => x.RecommendationKey!).Distinct().ToListAsync();
        var usedRecommendationKeySet = usedRecommendationKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var servedCount = await _db.AskpearlsPortalUserWebsiteWidgetEventLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.IsVisible == true && x.IsActive == true &&
                        (x.RecommendationKey != null || x.RecommendationTokenId != null))
            .CountAsync();
        return Ok(new
        {
            success = true,
            status = sourceVersion.WidgetStatusCode,
            versionNumber = sourceVersion.WidgetVersionNumber,
            selectedVersionNumber = version.WidgetVersionNumber,
            usedLiveFallback = sourceVersion != version,
            usedInRulesCount = rows.Count(item => usedRecommendationKeySet.Contains(item.RecommendationKey ?? item.RecommendationCode ?? "")),
            recommendationsServedCount = servedCount,
            items = rows.Select((x, i) => new {
                id = x.RecommendationTokenId ?? x.RowViewTokenId.ToString("N"),
                name = x.RecommendationName ?? x.RecommendationTitle ?? "",
                key = x.RecommendationKey ?? x.RecommendationCode ?? "",
                description = x.RecommendationDescription ?? x.RecommendationShortDescription ?? "",
                imageUrl = x.RecommendationImageUrl ?? x.ImageServerUrl,
                url = x.RecommendationUrl,
                ctaText = x.RecommendationCtatext ?? x.RecommendationCta,
                active = x.IsActive ?? !string.Equals(x.ActiveStatus, "inactive", StringComparison.OrdinalIgnoreCase),
                sortOrder = x.RecommendationSortOrder ?? i + 1
            })
        });
    }

    [HttpPut("{publicKey}/draft/recommendations")]
    public async Task<IActionResult> SaveRecommendationDraft(string publicKey, [FromBody] RecommendationDraftRequest? request)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        var widget = await FindDemoWidget(publicKey);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for the demo owner." });
        if (request?.Items is null || request.Items.Count > 500 || request.Items.Any(x =>
                string.IsNullOrWhiteSpace(x.Name) || x.Name.Trim().Length > 150 ||
                string.IsNullOrWhiteSpace(x.Key) || !Regex.IsMatch(x.Key, "^[a-zA-Z][a-zA-Z0-9_-]{0,79}$") ||
                x.Description?.Length > 1000 || x.ImageUrl?.Length > 2000 || x.Url?.Length > 2000 || x.CtaText?.Length > 100) ||
            request.Items.Select(x => x.Key.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != request.Items.Count)
            return BadRequest(new { success = false, message = "Recommendation fields are invalid. Names and unique identifiers are required." });

        var versions = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey && x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        var drafts = versions.Where(x => x.WidgetStatusCode == "draft").ToList();
        if (drafts.Count > 1) return Conflict(new { success = false, message = "More than one active draft exists for this widget." });
        var draft = drafts.SingleOrDefault();
        var published = versions.Where(x => x.WidgetStatusCode == "published" && x.WidgetVersionNumber == widget.WidgetPublishedVersion).ToList();
        var now = DateTime.UtcNow;
        if (draft is null)
        {
            if (published.Count != 1) return Conflict(new { success = false, message = "A published widget version is required before saving recommendations." });
            var maxNumber = versions.Select(x => int.TryParse(x.WidgetVersionNumber, out var n) ? n : 0).DefaultIfEmpty(0).Max();
            draft = new AskpearlsPortalUserWebsiteWidgetVersionList
            {
                AccountType = widget.AccountType,
                OwnerUserTokenId = widget.OwnerUserTokenId,
                OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
                OrganizationTokenId = widget.OrganizationTokenId,
                CompanyTokenId = widget.CompanyTokenId,
                CompanyName = widget.CompanyName,
                WidgetTokenId = widget.WidgetTokenId,
                WebsiteTokenId = widget.WebsiteTokenId,
                WebsiteName = widget.WebsiteName,
                WidgetPublicKey = publicKey,
                WidgetVersionNumber = (maxNumber + 1).ToString(),
                WidgetVersionTokenId = Guid.NewGuid().ToString("N"),
                WidgetConfigurationJson = published[0].WidgetConfigurationJson,
                WidgetStatusCode = "draft",
                WidgetStatusText = "draft",
                WidgetStatus = "draft",
                WidgetPublishedVersion = widget.WidgetPublishedVersion,
                ActiveStatus = "pending",
                IsNew = true,
                IsActive = true,
                IsVisible = true,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
            _db.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(draft);
        }

        var oldDraftRows = await _db.AskpearlsPortalUserWebsiteWidgetRecommendationLists
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetVersionNumber == draft.WidgetVersionNumber && x.IsActive == true)
            .ToListAsync();
        foreach (var row in oldDraftRows) { row.IsActive = false; row.IsVisible = false; row.UpdatedAtUtc = now; }
        var newRows = request!.Items.Select((item, index) => new AskpearlsPortalUserWebsiteWidgetRecommendationList
        {
            RowViewTokenId = Guid.NewGuid(),
            RowDataTokenId = Guid.NewGuid().ToString("N"),
            AccountType = widget.AccountType,
            OwnerUserTokenId = widget.OwnerUserTokenId,
            OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
            OrganizationTokenId = widget.OrganizationTokenId,
            CompanyTokenId = widget.CompanyTokenId,
            CompanyName = widget.CompanyName,
            WebsiteTokenId = widget.WebsiteTokenId,
            WidgetTokenId = widget.WidgetTokenId,
            WidgetPublicKey = publicKey,
            WidgetVersionTokenId = draft.WidgetVersionTokenId,
            WidgetVersionNumber = draft.WidgetVersionNumber,
            RecommendationTokenId = Guid.NewGuid().ToString("N"),
            RecommendationKey = item.Key.Trim(),
            RecommendationCode = item.Key.Trim(),
            RecommendationType = "outcome",
            RecommendationName = item.Name.Trim(),
            RecommendationTitle = item.Name.Trim(),
            RecommendationDescription = item.Description?.Trim(),
            RecommendationShortDescription = item.Description?.Trim(),
            RecommendationImageUrl = item.ImageUrl?.Trim(),
            RecommendationUrl = item.Url?.Trim(),
            RecommendationCtatext = item.CtaText?.Trim(),
            RecommendationSortOrder = index + 1,
            IsNew = true,
            ActiveStatus = item.Active ? "active" : "inactive",
            IsActive = item.Active,
            IsVisible = true,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        }).ToList();
        _db.AskpearlsPortalUserWebsiteWidgetRecommendationLists.AddRange(newRows);
        draft.UpdatedAtUtc = now;
        await _db.SaveChangesAsync();
        var ruleKeys = await _db.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey &&
                        x.IsVisible == true && x.RecommendationKey != null)
            .Select(x => x.RecommendationKey!).Distinct().ToListAsync();
        var eventCount = await _db.AskpearlsPortalUserWebsiteWidgetEventLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.IsVisible == true && x.IsActive == true &&
                        (x.RecommendationKey != null || x.RecommendationTokenId != null))
            .CountAsync();
        var ruleKeySet = ruleKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return Ok(new
        {
            success = true,
            status = "draft",
            versionNumber = draft.WidgetVersionNumber,
            itemCount = newRows.Count,
            usedInRulesCount = request.Items.Count(item => ruleKeySet.Contains(item.Key.Trim())),
            recommendationsServedCount = eventCount
        });
    }

    [HttpGet("{publicKey}/draft/rules")]
    public async Task<IActionResult> GetDecisionRuleDraft(string publicKey)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        var widget = await FindDemoWidget(publicKey);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for the demo owner." });
        var versions = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey && x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        var version = versions.Where(x => x.WidgetStatusCode == "draft")
            .OrderByDescending(x => x.UpdatedAtUtc ?? x.CreatedAtUtc).FirstOrDefault()
            ?? versions.FirstOrDefault(x => x.WidgetStatusCode == "published" && x.WidgetVersionNumber == widget.WidgetPublishedVersion);
        if (version is null) return Conflict(new { success = false, message = "A published or draft widget version is required." });

        // Older rule records may be linked by version token (or one of the
        // legacy token columns) without a populated version number. Load this
        // widget's rows once, then resolve the selected draft/live version.
        var widgetRows = await _db.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == widget.OwnerUserTokenId && x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.IsVisible == true)
            .ToListAsync();
        bool BelongsToVersion(AskpearlsPortalUserWebsiteWidgetDecisionRuleList row, AskpearlsPortalUserWebsiteWidgetVersionList target) =>
            (!string.IsNullOrWhiteSpace(target.WidgetVersionTokenId) &&
                (row.WidgetVersionTokenId == target.WidgetVersionTokenId || row.WidgetVersionTokenId1 == target.WidgetVersionTokenId ||
                 row.WidgetVersionTokenId2 == target.WidgetVersionTokenId || row.WidgetVersionTokenId3 == target.WidgetVersionTokenId ||
                 row.WidgetVersionTokenId4 == target.WidgetVersionTokenId)) ||
            (!string.IsNullOrWhiteSpace(target.WidgetVersionNumber) && row.WidgetVersionNumber == target.WidgetVersionNumber);

        var sourceVersion = version;
        var rows = widgetRows.Where(row => BelongsToVersion(row, version)).ToList();
        if (rows.Count == 0 && string.Equals(version.WidgetStatusCode, "draft", StringComparison.OrdinalIgnoreCase))
        {
            var liveVersion = versions.FirstOrDefault(x => x.WidgetStatusCode == "published" && x.WidgetVersionNumber == widget.WidgetPublishedVersion);
            if (liveVersion is not null)
            {
                var liveRows = widgetRows.Where(row => BelongsToVersion(row, liveVersion)).ToList();
                if (liveRows.Count > 0) { rows = liveRows; sourceVersion = liveVersion; }
            }
        }
        // Very old rows may predate version tracking. They are safe to show
        // only when they have no version identifiers at all.
        if (rows.Count == 0)
            rows = widgetRows.Where(row => string.IsNullOrWhiteSpace(row.WidgetVersionTokenId) &&
                string.IsNullOrWhiteSpace(row.WidgetVersionTokenId1) && string.IsNullOrWhiteSpace(row.WidgetVersionTokenId2) &&
                string.IsNullOrWhiteSpace(row.WidgetVersionTokenId3) && string.IsNullOrWhiteSpace(row.WidgetVersionTokenId4) &&
                string.IsNullOrWhiteSpace(row.WidgetVersionNumber)).ToList();

        var rules = rows.GroupBy(x => x.DecisionRuleTokenId ?? x.DecisionRuleCode ?? Guid.NewGuid().ToString("N"))
            .Select(group =>
            {
                var first = group.First();
                var priority = 1;
                try
                {
                    if (!string.IsNullOrWhiteSpace(first.DecisionRuleValue))
                    {
                        using var metadata = JsonDocument.Parse(first.DecisionRuleValue);
                        // Legacy rule rows can store a scalar number in this
                        // column. Only inspect priority when the JSON is an object.
                        if (metadata.RootElement.ValueKind == JsonValueKind.Object &&
                            metadata.RootElement.TryGetProperty("priority", out var value) &&
                            value.TryGetInt32(out var parsed)) priority = parsed;
                    }
                }
                catch (JsonException) { }
                return new
                {
                    id = first.DecisionRuleTokenId,
                    key = first.DecisionRuleCode ?? string.Empty,
                    name = first.DecisionRuleKey ?? first.DecisionRuleCode ?? "Untitled rule",
                    description = first.DecisionRuleReason ?? string.Empty,
                    recommendationKey = first.RecommendationKey ?? string.Empty,
                    recommendationName = first.RecommendationValue ?? first.RecommendationKey ?? string.Empty,
                    conditionLogic = string.Equals(first.DecisionRuleType, "any", StringComparison.OrdinalIgnoreCase) ? "any" : "all",
                    priority,
                    status = string.Equals(first.ActiveStatus, "draft", StringComparison.OrdinalIgnoreCase) ? "draft" : (first.IsActive == true ? "active" : "inactive"),
                    conditions = group.Select(x => new {
                        questionKey = x.WidgetQuestionKey ?? string.Empty,
                        questionText = x.WidgetQuestionValue ?? x.WidgetQuestionKey ?? string.Empty,
                        answerKey = x.WidgetAnswerKey ?? x.AnswerValue ?? string.Empty,
                        answerText = x.WidgetAnswerValue ?? x.AnswerValue ?? x.WidgetAnswerKey ?? string.Empty
                    }).ToArray()
                };
            }).OrderBy(x => x.priority).ToArray();
        return Ok(new
        {
            success = true,
            status = sourceVersion.WidgetStatusCode,
            versionNumber = sourceVersion.WidgetVersionNumber,
            selectedVersionNumber = version.WidgetVersionNumber,
            usedLiveFallback = sourceVersion != version,
            rules
        });
    }

    [HttpPut("{publicKey}/draft/rules")]
    public async Task<IActionResult> SaveDecisionRuleDraft(string publicKey, [FromBody] DecisionRulesDraftRequest? request)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        var widget = await FindDemoWidget(publicKey);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for the demo owner." });
        var valid = request?.Rules is not null && request.Rules.Count <= 300 &&
            request.Rules.All(rule => !string.IsNullOrWhiteSpace(rule.Key) && Regex.IsMatch(rule.Key, "^[a-z][a-z0-9_]{0,79}$") &&
                !string.IsNullOrWhiteSpace(rule.Name) && rule.Name.Trim().Length <= 100 && rule.Description?.Length <= 500 &&
                !string.IsNullOrWhiteSpace(rule.RecommendationKey) && rule.RecommendationKey.Length <= 100 &&
                rule.RecommendationName?.Length <= 150 && rule.ConditionLogic is "all" or "any" &&
                rule.Priority is >= 1 and <= 999 && rule.Status is "active" or "draft" or "inactive" &&
                rule.Conditions is { Count: > 0 and <= 50 } && rule.Conditions.All(c =>
                    !string.IsNullOrWhiteSpace(c.QuestionKey) && c.QuestionKey.Length <= 100 && c.QuestionText?.Length <= 500 &&
                    !string.IsNullOrWhiteSpace(c.AnswerKey) && c.AnswerKey.Length <= 150 && c.AnswerText?.Length <= 300)) &&
            request.Rules.Select(x => x.Key.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() == request.Rules.Count;
        if (!valid) return BadRequest(new { success = false, message = "Decision rule data is invalid. Provide unique rule identifiers and at least one complete question/answer condition for each rule." });

        var versions = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey && x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        var drafts = versions.Where(x => x.WidgetStatusCode == "draft").ToList();
        if (drafts.Count > 1) return Conflict(new { success = false, message = "More than one active draft exists for this widget." });
        var draft = drafts.SingleOrDefault();
        var published = versions.Where(x => x.WidgetStatusCode == "published" && x.WidgetVersionNumber == widget.WidgetPublishedVersion).ToList();
        var now = DateTime.UtcNow;
        if (draft is null)
        {
            if (published.Count != 1) return Conflict(new { success = false, message = "A published widget version is required before saving decision rules." });
            var maxNumber = versions.Select(x => int.TryParse(x.WidgetVersionNumber, out var n) ? n : 0).DefaultIfEmpty(0).Max();
            draft = new AskpearlsPortalUserWebsiteWidgetVersionList
            {
                AccountType = widget.AccountType,
                OwnerUserTokenId = widget.OwnerUserTokenId,
                OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
                OrganizationTokenId = widget.OrganizationTokenId,
                CompanyTokenId = widget.CompanyTokenId,
                CompanyName = widget.CompanyName,
                WidgetTokenId = widget.WidgetTokenId,
                WebsiteTokenId = widget.WebsiteTokenId,
                WebsiteName = widget.WebsiteName,
                WidgetPublicKey = publicKey,
                WidgetVersionNumber = (maxNumber + 1).ToString(),
                WidgetVersionTokenId = Guid.NewGuid().ToString("N"),
                WidgetConfigurationJson = published[0].WidgetConfigurationJson,
                WidgetStatusCode = "draft",
                WidgetStatusText = "draft",
                WidgetStatus = "draft",
                WidgetPublishedVersion = widget.WidgetPublishedVersion,
                ActiveStatus = "pending",
                IsNew = true,
                IsActive = true,
                IsVisible = true,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
            _db.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(draft);
        }
        var oldRows = await _db.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetVersionNumber == draft.WidgetVersionNumber && x.IsVisible == true)
            .ToListAsync();
        foreach (var row in oldRows) { row.IsActive = false; row.IsVisible = false; row.UpdatedAtUtc = now; }
        var rowsToSave = request!.Rules.SelectMany(rule => rule.Conditions.Select(condition => new AskpearlsPortalUserWebsiteWidgetDecisionRuleList
        {
            RowViewTokenId = Guid.NewGuid(),
            RowDataTokenId = Guid.NewGuid().ToString("N"),
            AccountType = widget.AccountType,
            OwnerUserTokenId = widget.OwnerUserTokenId,
            OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
            OrganizationTokenId = widget.OrganizationTokenId,
            CompanyTokenId = widget.CompanyTokenId,
            CompanyName = widget.CompanyName,
            WebsiteTokenId = widget.WebsiteTokenId,
            WebsiteName = widget.WebsiteName,
            WidgetTokenId = widget.WidgetTokenId,
            WidgetPublicKey = publicKey,
            WidgetVersionTokenId = draft.WidgetVersionTokenId,
            WidgetVersionNumber = draft.WidgetVersionNumber,
            DecisionRuleTokenId = Guid.NewGuid().ToString("N"),
            DecisionRuleCode = rule.Key.Trim(),
            DecisionRuleKey = rule.Name.Trim(),
            DecisionRuleType = rule.ConditionLogic,
            DecisionRuleValue = JsonSerializer.Serialize(new { priority = rule.Priority }),
            DecisionRuleReason = rule.Description?.Trim(),
            WidgetQuestionKey = condition.QuestionKey.Trim(),
            WidgetQuestionValue = condition.QuestionText?.Trim(),
            WidgetAnswerKey = condition.AnswerKey.Trim(),
            WidgetAnswerValue = condition.AnswerText?.Trim(),
            AnswerValue = condition.AnswerKey.Trim(),
            RecommendationKey = rule.RecommendationKey.Trim(),
            RecommendationValue = rule.RecommendationName?.Trim(),
            DecisionRuleScore = 1,
            ActiveStatus = rule.Status,
            IsActive = rule.Status == "active",
            IsVisible = true,
            IsNew = true,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        })).ToList();
        // Give every condition in a rule the same grouping token.
        var index = 0;
        foreach (var rule in request.Rules)
        {
            var ruleToken = Guid.NewGuid().ToString("N");
            foreach (var _ in rule.Conditions) rowsToSave[index++].DecisionRuleTokenId = ruleToken;
        }
        _db.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists.AddRange(rowsToSave);
        draft.UpdatedAtUtc = now;
        await _db.SaveChangesAsync();
        return Ok(new { success = true, status = "draft", versionNumber = draft.WidgetVersionNumber, ruleCount = request.Rules.Count });
    }

    private async Task<AskpearlsPortalUserWebsiteWidgetList?> FindDemoWidget(string publicKey) =>
        await _db.AskpearlsPortalUserWebsiteWidgetLists.FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
            x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true && x.IsVisible == true);

    private static bool ReadRequired(string? json)
    {
        try { return JsonDocument.Parse(json ?? "{}").RootElement.TryGetProperty("required", out var value) && value.GetBoolean(); }
        catch (JsonException) { return true; }
    }

    private static bool IsValidQuestionDraft(WidgetQuestionsDraftRequest? request)
    {
        if (request?.Questions is null || request.Questions.Count > 50) return false;
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var q in request.Questions)
        {
            if (string.IsNullOrWhiteSpace(q.Text) || q.Text.Length > 500 || string.IsNullOrWhiteSpace(q.Key) ||
                !Regex.IsMatch(q.Key ?? "", "^[a-zA-Z][a-zA-Z0-9_]{0,79}$") || !keys.Add(q.Key!) ||
                q.Helper?.Length > 500 || q.Options is null || q.Options.Count is < 2 or > 30 ||
                q.Options.Any(x => string.IsNullOrWhiteSpace(x) || x.Length > 150) ||
                q.Options.Distinct(StringComparer.OrdinalIgnoreCase).Count() != q.Options.Count) return false;
        }
        return true;
    }

    private bool TryGetDemoOwner(out IActionResult? error)
    {
        error = null;
        if (!_environment.IsDevelopment())
        {
            error = NotFound();
            return false;
        }
        if (Request.Headers["X-Demo-Owner-User-Token"].ToString() != DemoOwnerUserTokenId)
        {
            error = Unauthorized(new { success = false, message = "Demo owner login is required." });
            return false;
        }
        return true;
    }

    private static bool IsValidDesign(WidgetDesignDraftRequest? value)
    {
        if (value is null || !IsHex(value.Primary) || !IsHex(value.Background) || !IsHex(value.Text) ||
            value.FontSize is < 12 or > 18 || value.Radius is < 0 or > 28 || value.Width is < 300 or > 520 ||
            value.Position is not ("bottom-right" or "bottom-left" or "center") ||
            value.ButtonShape is not ("rounded" or "pill" or "square") ||
            value.Theme is not ("custom" or "light" or "dark") ||
            value.Heading is null || value.Description is null || value.ButtonText is null || value.Footer is null)
            return false;

        var fonts = new[] { "Inter, Arial, sans-serif", "Arial, sans-serif", "Georgia, serif", "Verdana, sans-serif" };
        return fonts.Contains(value.Font, StringComparer.Ordinal) && value.Heading.Length <= 100 &&
               value.Description.Length <= 250 && value.ButtonText.Length <= 40 && value.Footer.Length <= 80;
    }

    private static bool IsValidStoredDesign(string? json)
    {
        try
        {
            return IsValidDesign(JsonSerializer.Deserialize<WidgetDesignDraftRequest>(
                json ?? "", new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }
        catch (JsonException) { return false; }
    }

    private static bool IsHex(string? value) =>
        Regex.IsMatch(value ?? "", "^#[0-9a-fA-F]{6}$");



    [HttpGet("{publicKey}")]
    public async Task<IActionResult> GetPublished(string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
        {
            return BadRequest(new
            {
                message = "Widget public key is required."
            });
        }

        var widget = await _widgetService
            .GetPublishedWidgetAsync(publicKey);

        if (widget == null)
        {
            return NotFound(new
            {
                message = "Published widget was not found."
            });
        }

        return Ok(widget);
    }




    [HttpGet("{publicKey}/version")]
    public async Task<IActionResult> GetPublishedVersion(
    string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
        {
            return BadRequest(new
            {
                message = "Widget public key is required."
            });
        }

        var version = await _widgetVersionService
            .GetPublishedVersionAsync(publicKey);

        if (version == null)
        {
            return NotFound(new
            {
                message = "Published widget version was not found."
            });
        }

        return Ok(new
        {
            version.VersionTokenId,
            version.VersionNumber,
            version.Status,
            version.ConfigurationJson
        });
    }





    [HttpGet("{publicKey}/questions")]
    public async Task<IActionResult> GetPublishedQuestions(string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
        {
            return BadRequest(new
            {
                message = "Widget public key is required."
            });
        }

        var questions =
            await _questionService.GetPublishedQuestionsAsync(publicKey);

        if (questions.Count == 0)
        {
            return NotFound(new
            {
                message = "No published questions were found for this widget."
            });
        }

        return Ok(new
        {
            widgetPublicKey = publicKey,
            questions
        });
    }







    //[HttpGet("{publicKey}")]
    [NonAction]
    public async Task<IActionResult> GetPublished_X3(string publicKey)
    {
        if (publicKey != "pk_demo_sofa_finder")
            return NotFound();




        //////////        var questions = new List<DecisionQuestion>
        //////////{
        //////////    new()
        //////////    {
        //////////        Key = "users"
        //////////    },

        //////////    new()
        //////////    {
        //////////        Key = "priority"
        //////////    }
        //////////};

        //////////        var recommendations = new List<DecisionRecommendation>
        //////////{
        //////////    new()
        //////////    {
        //////////        Key = "oslo",
        //////////        Name = "Oslo Modular Sofa",

        //////////        Rules =
        //////////        [
        //////////            new()
        //////////            {
        //////////                QuestionKey = "users",
        //////////                AnswerValue = "small",
        //////////                Score = 15,
        //////////                Reason = "Works well for smaller households"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "users",
        //////////                AnswerValue = "medium",
        //////////                Score = 25,
        //////////                Reason = "Comfortable for families and small groups"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "users",
        //////////                AnswerValue = "large",
        //////////                Score = 40,
        //////////                Reason = "Great for larger groups"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "priority",
        //////////                AnswerValue = "comfort",
        //////////                Score = 35,
        //////////                Reason = "Comfort-focused design"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "priority",
        //////////                AnswerValue = "style",
        //////////                Score = 10,
        //////////                Reason = "Modern and elegant appearance"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "priority",
        //////////                AnswerValue = "space",
        //////////                Score = 15,
        //////////                Reason = "Flexible modular configuration"
        //////////            }
        //////////        ]
        //////////    },

        //////////    new()
        //////////    {
        //////////        Key = "luna",
        //////////        Name = "Luna Designer Sofa",

        //////////        Rules =
        //////////        [
        //////////            new()
        //////////            {
        //////////                QuestionKey = "users",
        //////////                AnswerValue = "small",
        //////////                Score = 35,
        //////////                Reason = "Ideal for smaller spaces"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "users",
        //////////                AnswerValue = "medium",
        //////////                Score = 25,
        //////////                Reason = "Works well for everyday living"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "users",
        //////////                AnswerValue = "large",
        //////////                Score = 10,
        //////////                Reason = "Can accommodate larger households"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "priority",
        //////////                AnswerValue = "comfort",
        //////////                Score = 15,
        //////////                Reason = "Comfortable premium seating"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "priority",
        //////////                AnswerValue = "style",
        //////////                Score = 40,
        //////////                Reason = "Elegant contemporary design"
        //////////            },

        //////////            new()
        //////////            {
        //////////                QuestionKey = "priority",
        //////////                AnswerValue = "space",
        //////////                Score = 35,
        //////////                Reason = "Space-efficient footprint"
        //////////            }
        //////////        ]
        //////////    }
        //////////};


        //////////        var answers = new Dictionary<string, string>
        //////////        {
        //////////            ["users"] = "large",
        //////////            ["priority"] = "comfort"
        //////////        };

        //////////        var engine = new DecisionEngine();

        //////////        var result = engine.Evaluate(
        //////////            questions,
        //////////            recommendations,
        //////////            answers);





        /////////////////////////


        var configuration = new
        {
            /*
             * =========================================================
             * WIDGET
             * =========================================================
             */

            widget = new
            {
                key = "pk_demo_sofa_finder",

                name = "Sofa Finder",

                title = "Find Your Perfect Sofa",

                subtitle =
                    "Answer a few questions and we'll find the sofa that fits you best."
            },


            /*
             * =========================================================
             * QUESTIONS
             * =========================================================
             */

            questions = new[]
            {
            /*
             * =====================================================
             * QUESTION 1
             * =====================================================
             */

            new
            {
                key = "users",

                text =
                    "How many people will use the sofa?",

                options = new[]
                {
                    new
                    {
                        text = "1–2 people",

                        value = "small"
                    },

                    new
                    {
                        text = "3–4 people",

                        value = "medium"
                    },

                    new
                    {
                        text = "5+ people",

                        value = "large"
                    }
                }
            },


            /*
             * =====================================================
             * QUESTION 2
             * =====================================================
             */

            new
            {
                key = "priority",

                text =
                    "What matters most to you?",

                options = new[]
                {
                    new
                    {
                        text = "Comfort",

                        value = "comfort"
                    },

                    new
                    {
                        text = "Style",

                        value = "style"
                    },

                    new
                    {
                        text = "Space saving",

                        value = "space"
                    }
                }
            }
        },


            /*
             * =========================================================
             * RECOMMENDATION ITEMS
             *
             * These are the possible final results.
             *
             * The scores are NOT stored here directly.
             *
             * Scores are defined by the Decision Rules below.
             * =========================================================
             */

            recommendations = new[]
            {
            /*
             * =====================================================
             * RECOMMENDATION 1
             * OSLO
             * =====================================================
             */

            new
            {
                key = "oslo",

                name =
                    "Oslo Modular Sofa",

                imageUrl =
                    "https://images.unsplash.com/photo-1555041469-a586c61ea9bc",

                url =
                    "https://example.com/products/oslo-modular-sofa",

                shortDescription =
                    "A spacious, comfortable modular sofa designed for modern family living.",

                description =
                    "The Oslo Modular Sofa gives you flexible seating with a strong focus on comfort and everyday practicality.",

                ctaText =
                    "View Oslo Sofa",


                /*
                 * -------------------------------------------------
                 * PRODUCT HIGHLIGHTS
                 * -------------------------------------------------
                 */

                highlights = new[]
                {
                    "Seats up to 6 people",

                    "Modular configuration",

                    "Deep comfortable seating",

                    "Modern neutral design"
                },


                /*
                 * -------------------------------------------------
                 * DECISION RULES
                 *
                 * IMPORTANT:
                 *
                 * For EVERY possible answer:
                 *
                 * Oslo Score + Luna Score = 50
                 *
                 * Example:
                 *
                 * 5+ people
                 *
                 * Oslo = 40
                 * Luna = 10
                 *
                 * Total = 50
                 * -------------------------------------------------
                 */

                rules = new[]
                {
                    /*
                     * =================================================
                     * QUESTION 1
                     * How many people will use the sofa?
                     * =================================================
                     */


                    /*
                     * 1–2 people
                     *
                     * Oslo = 15
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "small",

                        score =
                            15,

                        reason =
                            "Works well for smaller households"
                    },


                    /*
                     * 3–4 people
                     *
                     * Oslo = 25
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "medium",

                        score =
                            25,

                        reason =
                            "Comfortable for families and small groups"
                    },


                    /*
                     * 5+ people
                     *
                     * Oslo = 40
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "large",

                        score =
                            40,

                        reason =
                            "Great for larger groups"
                    },


                    /*
                     * =================================================
                     * QUESTION 2
                     * What matters most to you?
                     * =================================================
                     */


                    /*
                     * Comfort
                     *
                     * Oslo = 35
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "comfort",

                        score =
                            35,

                        reason =
                            "Comfort-focused design"
                    },


                    /*
                     * Style
                     *
                     * Oslo = 10
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "style",

                        score =
                            10,

                        reason =
                            "Modern and elegant appearance"
                    },


                    /*
                     * Space saving
                     *
                     * Oslo = 15
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "space",

                        score =
                            15,

                        reason =
                            "Flexible modular configuration"
                    }
                }
            },


            /*
             * =====================================================
             * RECOMMENDATION 2
             * LUNA
             * =====================================================
             */

            new
            {
                key = "luna",

                name =
                    "Luna Designer Sofa",

                imageUrl =
                    "https://images.unsplash.com/photo-1550226891-ef816aed4a98",

                url =
                    "https://example.com/products/luna-designer-sofa",

                shortDescription =
                    "A stylish designer sofa for customers who want modern aesthetics without sacrificing comfort.",

                description =
                    "The Luna Designer Sofa combines contemporary styling with a compact footprint, making it a strong choice for design-conscious spaces.",

                ctaText =
                    "View Luna Sofa",


                /*
                 * -------------------------------------------------
                 * PRODUCT HIGHLIGHTS
                 * -------------------------------------------------
                 */

                highlights = new[]
                {
                    "Contemporary design",

                    "Space-efficient",

                    "Premium upholstery",

                    "Multiple color options"
                },


                /*
                 * -------------------------------------------------
                 * DECISION RULES
                 * -------------------------------------------------
                 */

                rules = new[]
                {
                    /*
                     * =================================================
                     * QUESTION 1
                     * How many people will use the sofa?
                     * =================================================
                     */


                    /*
                     * 1–2 people
                     *
                     * Oslo = 15
                     * Luna = 35
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "small",

                        score =
                            35,

                        reason =
                            "Ideal for smaller spaces"
                    },


                    /*
                     * 3–4 people
                     *
                     * Oslo = 25
                     * Luna = 25
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "medium",

                        score =
                            25,

                        reason =
                            "Works well for everyday living"
                    },


                    /*
                     * 5+ people
                     *
                     * Oslo = 40
                     * Luna = 10
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "large",

                        score =
                            10,

                        reason =
                            "Can accommodate larger households"
                    },


                    /*
                     * =================================================
                     * QUESTION 2
                     * What matters most to you?
                     * =================================================
                     */


                    /*
                     * Comfort
                     *
                     * Oslo = 35
                     * Luna = 15
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "comfort",

                        score =
                            15,

                        reason =
                            "Comfortable premium seating"
                    },


                    /*
                     * Style
                     *
                     * Oslo = 10
                     * Luna = 40
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "style",

                        score =
                            40,

                        reason =
                            "Elegant contemporary design"
                    },


                    /*
                     * Space saving
                     *
                     * Oslo = 15
                     * Luna = 35
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "space",

                        score =
                            35,

                        reason =
                            "Space-efficient footprint"
                    }
                }
            }
        }
        };


        return Ok(configuration);

    }



    //    [HttpGet("{publicKey}")]
    [NonAction]
    public async Task<IActionResult> GetPublished_X2(string publicKey)
    {
        if (publicKey != "pk_demo_sofa_finder")
            return NotFound();


        var configuration = new
        {
            /*
             * =========================================================
             * WIDGET
             * =========================================================
             */

            widget = new
            {
                key = "pk_demo_sofa_finder",

                name = "Sofa Finder",

                title = "Find Your Perfect Sofa",

                subtitle =
                    "Answer a few questions and we'll find the sofa that fits you best."
            },


            /*
             * =========================================================
             * QUESTIONS
             * =========================================================
             */

            questions = new[]
            {
            /*
             * =====================================================
             * QUESTION 1
             * =====================================================
             */

            new
            {
                key = "users",

                text =
                    "How many people will use the sofa?",

                options = new[]
                {
                    new
                    {
                        text = "1–2 people",

                        value = "small"
                    },

                    new
                    {
                        text = "3–4 people",

                        value = "medium"
                    },

                    new
                    {
                        text = "5+ people",

                        value = "large"
                    }
                }
            },


            /*
             * =====================================================
             * QUESTION 2
             * =====================================================
             */

            new
            {
                key = "priority",

                text =
                    "What matters most to you?",

                options = new[]
                {
                    new
                    {
                        text = "Comfort",

                        value = "comfort"
                    },

                    new
                    {
                        text = "Style",

                        value = "style"
                    },

                    new
                    {
                        text = "Space saving",

                        value = "space"
                    }
                }
            }
        },


            /*
             * =========================================================
             * RECOMMENDATION ITEMS
             *
             * These are the possible final results.
             *
             * The scores are NOT stored here directly.
             *
             * Scores are defined by the Decision Rules below.
             * =========================================================
             */

            recommendations = new[]
            {
            /*
             * =====================================================
             * RECOMMENDATION 1
             * OSLO
             * =====================================================
             */

            new
            {
                key = "oslo",

                name =
                    "Oslo Modular Sofa",

                imageUrl =
                    "https://images.unsplash.com/photo-1555041469-a586c61ea9bc",

                url =
                    "https://example.com/products/oslo-modular-sofa",

                shortDescription =
                    "A spacious, comfortable modular sofa designed for modern family living.",

                description =
                    "The Oslo Modular Sofa gives you flexible seating with a strong focus on comfort and everyday practicality.",

                ctaText =
                    "View Oslo Sofa",


                /*
                 * -------------------------------------------------
                 * PRODUCT HIGHLIGHTS
                 * -------------------------------------------------
                 */

                highlights = new[]
                {
                    "Seats up to 6 people",

                    "Modular configuration",

                    "Deep comfortable seating",

                    "Modern neutral design"
                },


                /*
                 * -------------------------------------------------
                 * DECISION RULES
                 *
                 * IMPORTANT:
                 *
                 * For EVERY possible answer:
                 *
                 * Oslo Score + Luna Score = 50
                 *
                 * Example:
                 *
                 * 5+ people
                 *
                 * Oslo = 40
                 * Luna = 10
                 *
                 * Total = 50
                 * -------------------------------------------------
                 */

                rules = new[]
                {
                    /*
                     * =================================================
                     * QUESTION 1
                     * How many people will use the sofa?
                     * =================================================
                     */


                    /*
                     * 1–2 people
                     *
                     * Oslo = 15
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "small",

                        score =
                            15,

                        reason =
                            "Works well for smaller households"
                    },


                    /*
                     * 3–4 people
                     *
                     * Oslo = 25
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "medium",

                        score =
                            25,

                        reason =
                            "Comfortable for families and small groups"
                    },


                    /*
                     * 5+ people
                     *
                     * Oslo = 40
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "large",

                        score =
                            40,

                        reason =
                            "Great for larger groups"
                    },


                    /*
                     * =================================================
                     * QUESTION 2
                     * What matters most to you?
                     * =================================================
                     */


                    /*
                     * Comfort
                     *
                     * Oslo = 35
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "comfort",

                        score =
                            35,

                        reason =
                            "Comfort-focused design"
                    },


                    /*
                     * Style
                     *
                     * Oslo = 10
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "style",

                        score =
                            10,

                        reason =
                            "Modern and elegant appearance"
                    },


                    /*
                     * Space saving
                     *
                     * Oslo = 15
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "space",

                        score =
                            15,

                        reason =
                            "Flexible modular configuration"
                    }
                }
            },


            /*
             * =====================================================
             * RECOMMENDATION 2
             * LUNA
             * =====================================================
             */

            new
            {
                key = "luna",

                name =
                    "Luna Designer Sofa",

                imageUrl =
                    "https://images.unsplash.com/photo-1550226891-ef816aed4a98",

                url =
                    "https://example.com/products/luna-designer-sofa",

                shortDescription =
                    "A stylish designer sofa for customers who want modern aesthetics without sacrificing comfort.",

                description =
                    "The Luna Designer Sofa combines contemporary styling with a compact footprint, making it a strong choice for design-conscious spaces.",

                ctaText =
                    "View Luna Sofa",


                /*
                 * -------------------------------------------------
                 * PRODUCT HIGHLIGHTS
                 * -------------------------------------------------
                 */

                highlights = new[]
                {
                    "Contemporary design",

                    "Space-efficient",

                    "Premium upholstery",

                    "Multiple color options"
                },


                /*
                 * -------------------------------------------------
                 * DECISION RULES
                 * -------------------------------------------------
                 */

                rules = new[]
                {
                    /*
                     * =================================================
                     * QUESTION 1
                     * How many people will use the sofa?
                     * =================================================
                     */


                    /*
                     * 1–2 people
                     *
                     * Oslo = 15
                     * Luna = 35
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "small",

                        score =
                            35,

                        reason =
                            "Ideal for smaller spaces"
                    },


                    /*
                     * 3–4 people
                     *
                     * Oslo = 25
                     * Luna = 25
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "medium",

                        score =
                            25,

                        reason =
                            "Works well for everyday living"
                    },


                    /*
                     * 5+ people
                     *
                     * Oslo = 40
                     * Luna = 10
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "large",

                        score =
                            10,

                        reason =
                            "Can accommodate larger households"
                    },


                    /*
                     * =================================================
                     * QUESTION 2
                     * What matters most to you?
                     * =================================================
                     */


                    /*
                     * Comfort
                     *
                     * Oslo = 35
                     * Luna = 15
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "comfort",

                        score =
                            15,

                        reason =
                            "Comfortable premium seating"
                    },


                    /*
                     * Style
                     *
                     * Oslo = 10
                     * Luna = 40
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "style",

                        score =
                            40,

                        reason =
                            "Elegant contemporary design"
                    },


                    /*
                     * Space saving
                     *
                     * Oslo = 15
                     * Luna = 35
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "space",

                        score =
                            35,

                        reason =
                            "Space-efficient footprint"
                    }
                }
            }
        }
        };


        return Ok(configuration);

    }

    ////[HttpGet("{publicKey}")]
    [NonAction]
    public async Task<IActionResult> GetPublished_X1(string publicKey)
    {

        if (publicKey != "pk_demo_sofa_finder")
            return NotFound();

        var configuration = new
        {
            title = "Find Your Perfect Sofa",

            subtitle = "Answer a few questions and we'll find the sofa that fits you best.",

            questions = new[]
            {
                new
                {
                    key = "users",

                    text = "How many people will use the sofa?",

                    options = new[]
                    {
                        new
                        {
                            text = "1–2 people",
                            value = "small"
                        },

                        new
                        {
                            text = "3–4 people",
                            value = "medium"
                        },

                        new
                        {
                            text = "5+ people",
                            value = "large"
                        }
                    }
                },

                new
                {
                    key = "priority",

                    text = "What matters most to you?",

                    options = new[]
                    {
                        new
                        {
                            text = "Comfort",
                            value = "comfort"
                        },

                        new
                        {
                            text = "Style",
                            value = "style"
                        },

                        new
                        {
                            text = "Space saving",
                            value = "space"
                        }
                    }
                }
            },

            recommendations = new[]
{
    new
    {
        key = "oslo",

        name = "Oslo Modular Sofa",

        imageUrl =
            "https://images.unsplash.com/photo-1555041469-a586c61ea9bc",

        url =
            "https://example.com/products/oslo-modular-sofa",

        shortDescription =
            "A spacious, comfortable modular sofa designed for modern family living.",

        description =
            "The Oslo Modular Sofa gives you flexible seating with a strong focus on comfort and everyday practicality.",

        ctaText = "View Oslo Sofa",

        /*
         * DecisionOS scoring rules
         */
        rules = new[]
        {
            new
            {
                questionKey = "users",
                answerValue = "large",
                score = 40,
                reason = "Great for larger groups"
            },

            new
            {
                questionKey = "users",
                answerValue = "medium",
                score = 25,
                reason = "Comfortable for families and small groups"
            },

            new
            {
                questionKey = "users",
                answerValue = "small",
                score = 10,
                reason = "Works well for smaller households"
            },

            new
            {
                questionKey = "priority",
                answerValue = "comfort",
                score = 45,
                reason = "Comfort-focused design"
            },

            new
            {
                questionKey = "priority",
                answerValue = "style",
                score = 10,
                reason = "Modern and elegant appearance"
            },

            new
            {
                questionKey = "priority",
                answerValue = "space",
                score = 15,
                reason = "Flexible modular configuration"
            }
        },

        highlights = new[]
        {
            "Seats up to 6 people",
            "Modular configuration",
            "Deep comfortable seating",
            "Modern neutral design"
        }
    },


    new
    {
        key = "luna",

        name = "Luna Designer Sofa",

        imageUrl =
            "https://images.unsplash.com/photo-1550226891-ef816aed4a98",

        url =
            "https://example.com/products/luna-designer-sofa",

        shortDescription =
            "A stylish designer sofa for customers who want modern aesthetics without sacrificing comfort.",

        description =
            "The Luna Designer Sofa combines contemporary styling with a compact footprint, making it a strong choice for design-conscious spaces.",

        ctaText = "View Luna Sofa",

        /*
         * DecisionOS scoring rules
         */
        rules = new[]
        {
            new
            {
                questionKey = "users",
                answerValue = "small",
                score = 25,
                reason = "Ideal for smaller spaces"
            },

            new
            {
                questionKey = "users",
                answerValue = "medium",
                score = 20,
                reason = "Works well for everyday living"
            },

            new
            {
                questionKey = "users",
                answerValue = "large",
                score = 10,
                reason = "Can accommodate larger households"
            },

            new
            {
                questionKey = "priority",
                answerValue = "style",
                score = 50,
                reason = "Elegant contemporary design"
            },

            new
            {
                questionKey = "priority",
                answerValue = "space",
                score = 40,
                reason = "Space-efficient footprint"
            },

            new
            {
                questionKey = "priority",
                answerValue = "comfort",
                score = 25,
                reason = "Comfortable premium seating"
            }
        },

        highlights = new[]
        {
            "Contemporary design",
            "Space-efficient",
            "Premium upholstery",
            "Multiple color options"
        }
    }
}
            //////////recommendations = new[]
            //////////{
            //////////    new
            //////////    {
            //////////        key = "oslo",

            //////////        name = "Oslo Modular Sofa",

            //////////        matchScore = 92,

            //////////        imageUrl =
            //////////            "https://images.unsplash.com/photo-1555041469-a586c61ea9bc",

            //////////        url =
            //////////            "https://example.com/products/oslo-modular-sofa",

            //////////        shortDescription =
            //////////            "A spacious, comfortable modular sofa designed for modern family living.",

            //////////        description =
            //////////            "The Oslo Modular Sofa gives you flexible seating with a strong focus on comfort and everyday practicality.",

            //////////        reasons = new[]
            //////////        {
            //////////            "Great for larger groups",

            //////////            "Comfort-focused design",

            //////////            "Flexible modular configuration",

            //////////            "Ideal for everyday family use"
            //////////        },

            //////////        highlights = new[]
            //////////        {
            //////////            "Seats up to 6 people",

            //////////            "Modular configuration",

            //////////            "Deep comfortable seating",

            //////////            "Modern neutral design"
            //////////        },

            //////////        ctaText = "View Oslo Sofa"
            //////////    },

            //////////    new
            //////////    {
            //////////        key = "luna",

            //////////        name = "Luna Designer Sofa",

            //////////        matchScore = 94,

            //////////        imageUrl =
            //////////            "https://images.unsplash.com/photo-1550226891-ef816aed4a98",

            //////////        url =
            //////////            "https://example.com/products/luna-designer-sofa",

            //////////        shortDescription =
            //////////            "A stylish designer sofa for customers who want modern aesthetics without sacrificing comfort.",

            //////////        description =
            //////////            "The Luna Designer Sofa combines contemporary styling with a compact footprint, making it a strong choice for design-conscious spaces.",

            //////////        reasons = new[]
            //////////        {
            //////////            "Elegant contemporary design",

            //////////            "Space-efficient footprint",

            //////////            "Ideal for style-focused buyers",

            //////////            "Perfect for modern interiors"
            //////////        },

            //////////        highlights = new[]
            //////////        {
            //////////            "Contemporary design",

            //////////            "Space-efficient",

            //////////            "Premium upholstery",

            //////////            "Multiple color options"
            //////////        },

            //////////        ctaText = "View Luna Sofa"
            //////////    }
            //////////}
        };

        return Ok(configuration);


        ///////////////////////////////////////////////
        ///
        /// 
        //////////if (publicKey == "pk_demo_sofa_finder")
        //////////{
        //////////    return Ok(new
        //////////    {
        //////////        title = "Find Your Perfect Sofa",

        //////////        questions = new[]
        //////////        {
        //////////        new
        //////////        {
        //////////            key = "users",
        //////////            text = "How many people will use the sofa?",
        //////////            options = new[]
        //////////            {
        //////////                new { text = "1–2 people", value = "small" },
        //////////                new { text = "3–4 people", value = "medium" },
        //////////                new { text = "5+ people", value = "large" }
        //////////            }
        //////////        },
        //////////        new
        //////////        {
        //////////            key = "priority",
        //////////            text = "What matters most?",
        //////////            options = new[]
        //////////            {
        //////////                new { text = "Comfort", value = "comfort" },
        //////////                new { text = "Style", value = "style" },
        //////////                new { text = "Space saving", value = "space" }
        //////////            }
        //////////        }
        //////////    },

        //////////        recommendations = new[]
        //////////        {
        //////////        new { key = "oslo", name = "Oslo Modular Sofa" },
        //////////        new { key = "luna", name = "Luna Designer Sofa" }
        //////////    }
        //////////    });
        //////////}

        //////////return NotFound();

        ///////////////////////////////////////////////////////////////////////

        //////////if (publicKey != "pk_demo_sofa_finder")
        //////////    return NotFound();

        //////////var configuration = new
        //////////{
        //////////    title = "Find Your Perfect Sofa",

        //////////    questions = new[]
        //////////    {
        //////////        new
        //////////        {
        //////////            key = "users",
        //////////            text = "How many people will use the sofa?",
        //////////            options = new[]
        //////////            {
        //////////                new { text = "1–2 people", value = "small" },
        //////////                new { text = "3–4 people", value = "medium" },
        //////////                new { text = "5+ people", value = "large" }
        //////////            }
        //////////        },

        //////////        new
        //////////        {
        //////////            key = "priority",
        //////////            text = "What matters most to you?",
        //////////            options = new[]
        //////////            {
        //////////                new { text = "Comfort", value = "comfort" },
        //////////                new { text = "Style", value = "style" },
        //////////                new { text = "Space saving", value = "space" }
        //////////            }
        //////////        }
        //////////    },

        //////////    recommendations = new[]
        //////////    {
        //////////        new
        //////////        {
        //////////            key = "oslo",
        //////////            name = "Oslo Modular Sofa"
        //////////        },

        //////////        new
        //////////        {
        //////////            key = "luna",
        //////////            name = "Luna Designer Sofa"
        //////////        }
        //////////    }
        //////////};

        //////////return Ok(configuration);

        //////////////////////////////////////////////

        ////var widget = await db.Widgets.AsNoTracking()
        ////    .FirstOrDefaultAsync(x => x.PublicKey == publicKey && x.Status == "Published");

        ////if (widget is null) return NotFound();

        ////var version = await db.WidgetVersions.AsNoTracking()
        ////    .FirstOrDefaultAsync(x => x.WidgetId == widget.Id &&
        ////                              x.VersionNumber == widget.PublishedVersion &&
        ////                              x.Status == "Published");

        ////if (version is null) return NotFound();

        ////return Content(version.ConfigurationJson, "application/json");

        ////return Ok();

    }






}

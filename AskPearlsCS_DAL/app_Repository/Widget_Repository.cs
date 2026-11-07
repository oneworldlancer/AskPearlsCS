using AskPearls.Domain;

using AskPearlsCS_DAL.app_Mapper;
using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DOMAIN.app_ContractsManager;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using System.Text.RegularExpressions;

namespace AskPearlsCS_DAL.app_Repository;

public class Widget_Repository : IWidget_Repository
{
    private readonly DBContextModel _dbContext;
    private readonly IWidgetBrandingEntitlement? _brandingEntitlement;
    public Widget_Repository(DBContextModel dbContext, IWidgetBrandingEntitlement? brandingEntitlement = null)
    {
        _dbContext = dbContext;
        _brandingEntitlement = brandingEntitlement;
    }

    public async Task<Widget?> GetPublishedWidgetAsync(string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey)) return null;
        var entity = await _dbContext.AskpearlsPortalUserWebsiteWidgetLists
            .AsNoTracking().FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
            x.WidgetStatusCode == "published" && x.IsActive == true && x.IsVisible == true);
        return Widget_Mapper.ToDomain(entity);
    }

    public async Task<List<Widget>> GetByCompanyTokenIdAsync(string companyTokenId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(companyTokenId) || companyTokenId == "0") return new List<Widget>();
        var entities = await _dbContext.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking()
            .Where(x => x.CompanyTokenId == companyTokenId && x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UploadDate).ToListAsync(cancellationToken);
        return entities.Select(Widget_Mapper.ToDomain).Where(x => x != null).Select(x => x!).ToList();
    }

    public async Task<List<Widget>> GetByOwnerUserTokenIdAsync(string ownerUserTokenId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ownerUserTokenId) || ownerUserTokenId == "0") return new List<Widget>();
        var entities = await _dbContext.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == ownerUserTokenId && x.IsActive == true && x.IsVisible == true)
            .OrderBy(x => x.WidgetName).ToListAsync(cancellationToken);
        return entities.Select(Widget_Mapper.ToDomain).Where(x => x != null).Select(x => x!).ToList();
    }

    // HTTP-neutral response factories: DAL never references ASP.NET MVC or ControllerBase.
    private static WidgetOperationResult Ok(object? body) => new(WidgetOperationOutcome.Ok, body);
    private static WidgetOperationResult Created(string location, object? body) => new(WidgetOperationOutcome.Created, body, location);
    private static WidgetOperationResult BadRequest(object? body) => new(WidgetOperationOutcome.Invalid, body);
    private static WidgetOperationResult NotFound(object? body) => new(WidgetOperationOutcome.NotFound, body);
    private static WidgetOperationResult Conflict(object? body) => new(WidgetOperationOutcome.Conflict, body);

    public async Task<WidgetOperationResult> GetMyWidgetsAsync(string ownerUserTokenId)
    {
        var widgets = await _dbContext.AskpearlsPortalUserWebsiteWidgetLists
            .AsNoTracking()
            .Where(x =>
            x.OwnerUserTokenId == ownerUserTokenId &&
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
            : await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
                .AsNoTracking()
                .Where(x => x.OwnerUserTokenId == ownerUserTokenId &&
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

        var questions = await _dbContext.AskpearlsPortalUserWebsiteWidgetQuestionLists
            .AsNoTracking()
            .Where(x => x.OwnerUserTokenId == ownerUserTokenId &&
                        widgetKeys.Contains(x.WidgetPublicKey!) &&
                        x.IsActive == true && x.IsVisible == true)
            .Select(x => new { x.WidgetPublicKey, x.WidgetVersionNumber })
            .ToListAsync();

        var recommendations = await _dbContext.AskpearlsPortalUserWebsiteWidgetRecommendationLists
            .AsNoTracking()
            .Where(x => x.OwnerUserTokenId == ownerUserTokenId &&
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

            var status = (widget.WidgetStatusCode ?? "draft").Trim().ToLowerInvariant();
            // Parent status is authoritative; show the corresponding version first.
            var currentVersion = status == "published" ? published ?? draft : draft ?? published;
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

                // Required to populate Target Website in Edit Mode.
                websiteTokenId = widget.WebsiteTokenId,
                websiteAddress = widget.WebsiteAddress,

                status,
                versionNumber = currentVersionNumber,
                draftVersionNumber = draft?.WidgetVersionNumber,
                publishedVersionNumber = published?.WidgetVersionNumber ?? widget.WidgetPublishedVersion,
                hasDraft = draft is not null,
                hasPublishedVersion = published is not null ||
                    !string.IsNullOrWhiteSpace(widget.WidgetPublishedVersion),
                updatedAt = currentVersion?.UpdatedAtUtc ?? currentVersion?.CreatedAtUtc,
                questionCount,
                recommendationCount
            };


        });

        return Ok(new { success = true, widgets = result });
    }


    public async Task<WidgetOperationResult> GetMyWebsitesAsync(string ownerUserTokenId)
    {
        var websites = await _dbContext.AskpearlsPortalUserWebsiteLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == ownerUserTokenId && x.IsActive == true && x.IsVisible == true)
            .OrderBy(x => x.WebsiteName)
            .Select(x => new { websiteTokenId = x.WebsiteTokenId, name = x.WebsiteName, address = x.WebsiteAddress })
            .ToListAsync();
        return Ok(new { success = true, websites });
    }


    public async Task<WidgetOperationResult> CreateWidgetAsync(string ownerUserTokenId, CreateWidgetRequest? request)
    {
        if (request is null ||
    string.IsNullOrWhiteSpace(request.Name) ||
    request.Name.Trim().Length > 100 ||
    request.Type != "decision" ||
    string.IsNullOrWhiteSpace(request.Title) ||
    request.Title.Trim().Length > 120 ||
    request.Description?.Length > 500 ||
    (string.IsNullOrWhiteSpace(request.WebsiteTokenId) &&
     string.IsNullOrWhiteSpace(request.WebsiteAddress)))
        {
            return BadRequest(new
            {
                success = false,
                message = "Enter a valid widget name, supported type, target website, title, and description of up to 500 characters."
            });
        }

        AskpearlsPortalUserWebsiteList? website = null;
        if (!string.IsNullOrWhiteSpace(request.WebsiteTokenId))
        {
            website = await _dbContext.AskpearlsPortalUserWebsiteLists.FirstOrDefaultAsync(x =>
                x.WebsiteTokenId == request.WebsiteTokenId && x.OwnerUserTokenId == ownerUserTokenId &&
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
            website = await _dbContext.AskpearlsPortalUserWebsiteLists.FirstOrDefaultAsync(x =>
                x.OwnerUserTokenId == ownerUserTokenId && x.WebsiteAddress == request.WebsiteAddress &&
                x.IsActive == true && x.IsVisible == true);
            if (website is null)
            {
                website = new AskpearlsPortalUserWebsiteList
                {
                    RowViewTokenId = Guid.NewGuid(),
                    RowDataTokenId = Guid.NewGuid().ToString("N"),
                    AccountType = "portal",
                    OwnerUserTokenId = ownerUserTokenId,
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
                _dbContext.AskpearlsPortalUserWebsiteLists.Add(website);
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
            OwnerUserTokenId = ownerUserTokenId,
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
            WidgetPublishedVersion = "0",
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
        _dbContext.AskpearlsPortalUserWebsiteWidgetLists.Add(widget);
        _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(version);
        await _dbContext.SaveChangesAsync();
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


    public async Task<WidgetOperationResult> UpdateWidgetBasicsAsync(
        string ownerUserTokenId,
        string publicKey,
        UpdateWidgetBasicsRequest? request)
    {
        // ---------------------------------------------------------
        // 1. Validate request
        // ---------------------------------------------------------

        if (string.IsNullOrWhiteSpace(ownerUserTokenId) ||
            string.IsNullOrWhiteSpace(publicKey) ||
            request is null ||
            string.IsNullOrWhiteSpace(request.Name) ||
            request.Name.Trim().Length > 100 ||
            string.IsNullOrWhiteSpace(request.Title) ||
            request.Title.Trim().Length > 120 ||
            request.Description?.Length > 500)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid widget basic information."
            });
        }

        // ---------------------------------------------------------
        // 2. Validate target website
        // ---------------------------------------------------------

        var address = request.WebsiteAddress?.Trim();

        Uri? uri = null;

        if (!string.IsNullOrWhiteSpace(address) &&
            (address.Length > 250 ||
             !Uri.TryCreate(address, UriKind.Absolute, out uri) ||
             uri.Scheme is not ("http" or "https") ||
             string.IsNullOrWhiteSpace(uri.Host)))
        {
            return BadRequest(new
            {
                success = false,
                message = "Enter a valid HTTP/HTTPS Target Website URL."
            });
        }

        if (string.IsNullOrWhiteSpace(address) &&
            string.IsNullOrWhiteSpace(request.WebsiteTokenId))
        {
            return BadRequest(new
            {
                success = false,
                message = "Target Website is required."
            });
        }

        // ---------------------------------------------------------
        // 3. Begin transaction
        // ---------------------------------------------------------

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable);

        // ---------------------------------------------------------
        // 4. Find widget belonging to current user
        // ---------------------------------------------------------

        var widget = await _dbContext
            .AskpearlsPortalUserWebsiteWidgetLists
            .FirstOrDefaultAsync(x =>
                x.WidgetPublicKey == publicKey &&
                x.OwnerUserTokenId == ownerUserTokenId &&
                x.IsActive == true &&
                x.IsVisible == true);

        if (widget is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Widget was not found for this user."
            });
        }

        // ---------------------------------------------------------
        // 5. Find or create target website
        // ---------------------------------------------------------

        AskpearlsPortalUserWebsiteList? website = null;

        if (!string.IsNullOrWhiteSpace(address))
        {
            website = await _dbContext
                .AskpearlsPortalUserWebsiteLists
                .FirstOrDefaultAsync(x =>
                    x.OwnerUserTokenId == ownerUserTokenId &&
                    x.WebsiteAddress == address &&
                    x.IsActive == true &&
                    x.IsVisible == true);

            if (website is null)
            {
                var now = DateTime.UtcNow;

                website = new AskpearlsPortalUserWebsiteList
                {
                    RowViewTokenId = Guid.NewGuid(),
                    RowDataTokenId = Guid.NewGuid().ToString("N"),

                    AccountType = "portal",

                    OwnerUserTokenId = ownerUserTokenId,

                    WebsiteTokenId = Guid.NewGuid().ToString("N"),

                    WebsiteName =
                        string.IsNullOrWhiteSpace(request.WebsiteName)
                            ? uri!.Host
                            : request.WebsiteName.Trim(),

                    WebsiteAddress = address,

                    ActiveStatus = "active",

                    IsNew = true,
                    IsActive = true,
                    IsVisible = true,

                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };

                _dbContext
                    .AskpearlsPortalUserWebsiteLists
                    .Add(website);
            }
        }
        else
        {
            website = await _dbContext
                .AskpearlsPortalUserWebsiteLists
                .FirstOrDefaultAsync(x =>
                    x.OwnerUserTokenId == ownerUserTokenId &&
                    x.WebsiteTokenId == request.WebsiteTokenId &&
                    x.IsActive == true &&
                    x.IsVisible == true);
        }

        if (website is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Target Website was not found for this user."
            });
        }

        // ---------------------------------------------------------
        // 6. Find active draft and validate uniqueness
        // ---------------------------------------------------------

        var activeDrafts = await _dbContext
            .AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x =>
                x.OwnerUserTokenId == ownerUserTokenId &&
                x.WidgetTokenId == widget.WidgetTokenId &&
                x.WidgetPublicKey == publicKey &&
                x.WidgetStatusCode == "draft" &&
                x.IsActive == true &&
                x.IsVisible == true)
            .ToListAsync();

        if (activeDrafts.Count > 1)
        {
            return Conflict(new
            {
                success = false,
                message = "More than one active draft exists for this widget."
            });
        }

        // ---------------------------------------------------------
        // 7. Update widget basic information
        // ---------------------------------------------------------

        widget.WidgetName = request.Name.Trim();
        widget.WidgetTitle = request.Title.Trim();

        widget.WidgetDescription =
            (request.Description ?? string.Empty).Trim();

        widget.WebsiteTokenId = website.WebsiteTokenId;
        widget.WebsiteName = website.WebsiteName;
        widget.WebsiteAddress = website.WebsiteAddress;

        // ---------------------------------------------------------
        // 8. Synchronize active draft website metadata
        // ---------------------------------------------------------

        foreach (var draft in activeDrafts)
        {
            draft.WebsiteTokenId = website.WebsiteTokenId;
            draft.WebsiteName = website.WebsiteName;
            draft.UpdatedAtUtc = DateTime.UtcNow;
        }

        // ---------------------------------------------------------
        // 9. Save and commit
        // ---------------------------------------------------------

        await _dbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        // ---------------------------------------------------------
        // 10. Return updated widget
        // ---------------------------------------------------------

        return Ok(new
        {
            success = true,

            publicKey = widget.WidgetPublicKey,

            name = widget.WidgetName,
            title = widget.WidgetTitle,
            description = widget.WidgetDescription,

            websiteTokenId = website.WebsiteTokenId,
            websiteName = website.WebsiteName,
            websiteAddress = website.WebsiteAddress,

            status = widget.WidgetStatusCode
        });
    }



    // Increment the editable draft revision on every successful Save Draft.
    // This keeps a single active draft and updates its dependent version references.
    // It does NOT create immutable historical snapshots of each draft revision.
    public async Task<WidgetOperationResult> SaveDesignDraftAsync(
        string ownerUserTokenId,
        string publicKey,
        WidgetDesignDraftRequest? request)
    {
        if (string.IsNullOrWhiteSpace(publicKey) || !IsValidDesign(request))
            return BadRequest(new { success = false, message = "Widget key or design settings are invalid." });

        // Fail closed: premium branding must be authorized by the subscription service.
        if (HasPremiumBranding(request!) &&
            (_brandingEntitlement is null ||
             !await _brandingEntitlement.CanUseWhiteLabelAsync(ownerUserTokenId)))
            return BadRequest(new
            {
                success = false,
                message = "White-label branding requires an eligible paid subscription."
            });

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable);

        var widget = await _dbContext.AskpearlsPortalUserWebsiteWidgetLists
            .FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
                                      x.OwnerUserTokenId == ownerUserTokenId &&
                                      x.IsActive == true && x.IsVisible == true);
        if (widget is null)
            return NotFound(new { success = false, message = "Widget was not found for this user." });

        var versions = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey &&
                        x.OwnerUserTokenId == ownerUserTokenId)
            .ToListAsync();

        var drafts = versions.Where(x => x.WidgetStatusCode == "draft" &&
                                         x.IsActive == true && x.IsVisible == true).ToList();
        if (drafts.Count > 1)
            return Conflict(new { success = false, message = "More than one active draft exists for this widget." });

        var publishedVersions = versions.Where(x => x.WidgetStatusCode == "published" &&
                                                     x.IsActive == true && x.IsVisible == true).ToList();
        if (publishedVersions.Count > 1)
            return Conflict(new { success = false, message = "Multiple active published versions exist." });

        var maxNumber = versions
            .Select(x => int.TryParse(x.WidgetVersionNumber,
                System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture,
                out var n) ? n : 0)
            .DefaultIfEmpty(0).Max();
        var nextVersionNumber = checked(maxNumber + 1).ToString(
            System.Globalization.CultureInfo.InvariantCulture);
        var now = DateTime.UtcNow;
        var json = JsonSerializer.Serialize(request);
        var draft = drafts.SingleOrDefault();

        if (draft is null)
        {
            draft = new AskpearlsPortalUserWebsiteWidgetVersionList
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
                WebsiteName = widget.WebsiteName,
                WidgetPublicKey = widget.WidgetPublicKey,
                WidgetVersionTokenId = Guid.NewGuid().ToString("N"),
                WidgetVersionNumber = nextVersionNumber,
                WidgetConfigurationJson = json,
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
            _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(draft);
        }
        else
        {
            var previousNumber = draft.WidgetVersionNumber;
            var draftToken = draft.WidgetVersionTokenId;

            // Keep existing draft content attached to the same draft revision.
            // Never touch rows belonging to the published version.
            var questions = await _dbContext.AskpearlsPortalUserWebsiteWidgetQuestionLists
                .Where(x => x.OwnerUserTokenId == ownerUserTokenId &&
                            x.WidgetPublicKey == publicKey &&
                            x.WidgetTokenId == widget.WidgetTokenId &&
                            x.WidgetVersionNumber == previousNumber &&
                            x.WidgetVersionTokenId == draftToken)
                .ToListAsync();
            foreach (var row in questions) row.WidgetVersionNumber = nextVersionNumber;

            var answers = await _dbContext.AskpearlsPortalUserWebsiteWidgetQuestionAnswerLists
                .Where(x => x.OwnerUserTokenId == ownerUserTokenId &&
                            x.WidgetPublicKey == publicKey &&
                            x.WidgetTokenId == widget.WidgetTokenId &&
                            x.WidgetVersionNumber == previousNumber &&
                            x.WidgetVersionTokenId == draftToken)
                .ToListAsync();
            foreach (var row in answers) row.WidgetVersionNumber = nextVersionNumber;

            var recommendations = await _dbContext.AskpearlsPortalUserWebsiteWidgetRecommendationLists
                .Where(x => x.OwnerUserTokenId == ownerUserTokenId &&
                            x.WidgetPublicKey == publicKey &&
                            x.WidgetTokenId == widget.WidgetTokenId &&
                            x.WidgetVersionNumber == previousNumber &&
                            x.WidgetVersionTokenId == draftToken)
                .ToListAsync();
            foreach (var row in recommendations) row.WidgetVersionNumber = nextVersionNumber;

            var rules = await _dbContext.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists
                .Where(x => x.OwnerUserTokenId == ownerUserTokenId &&
                            x.WidgetPublicKey == publicKey &&
                            x.WidgetTokenId == widget.WidgetTokenId &&
                            x.WidgetVersionNumber == previousNumber &&
                            x.WidgetVersionTokenId == draftToken)
                .ToListAsync();
            foreach (var row in rules) row.WidgetVersionNumber = nextVersionNumber;

            draft.WidgetVersionNumber = nextVersionNumber;
            draft.WidgetConfigurationJson = json;
            draft.UpdatedAtUtc = now;
        }

        // Parent status remains authoritative. Published version is untouched.
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new
        {
            success = true,
            status = "draft",
            versionNumber = draft.WidgetVersionNumber,
            publishedVersionNumber = widget.WidgetPublishedVersion
        });
    }


    public async Task<WidgetOperationResult> PublishDesignDraftAsync(string ownerUserTokenId, string publicKey)
    {
        var widget = await _dbContext.AskpearlsPortalUserWebsiteWidgetLists
            .FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
                                      x.OwnerUserTokenId == ownerUserTokenId &&
                                      x.IsActive == true && x.IsVisible == true);
        if (widget is null)
            return NotFound(new { success = false, message = "Widget was not found for this user." });

        var drafts = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
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

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        var published = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
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

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return Ok(new { success = true, status = "published", versionNumber = draft.WidgetVersionNumber });
    }


    public async Task<WidgetOperationResult> GetDesignDraftAsync(string ownerUserTokenId, string publicKey)
    {
        var widget = await _dbContext.AskpearlsPortalUserWebsiteWidgetLists
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
                                      x.OwnerUserTokenId == ownerUserTokenId &&
                                      x.IsActive == true && x.IsVisible == true);
        if (widget is null)
            return NotFound(new { success = false, message = "Widget was not found for this user." });

        var draft = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
            .AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetStatusCode == "draft" &&
                        x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UwebsiteWidgetVersionId)
            .FirstOrDefaultAsync();

        var version = draft ?? await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
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
                status = version?.WidgetStatusCode ?? widget.WidgetStatusCode ?? "draft",
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


    public async Task<WidgetOperationResult> GetQuestionDraftAsync(string ownerUserTokenId, string publicKey, IReadOnlyList<Question> publishedQuestions)
    {
        var widget = await FindOwnedWidget(publicKey, ownerUserTokenId);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for this user." });

        var draftVersion = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
            .AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey &&
                        x.WidgetStatusCode == "draft" && x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UpdatedAtUtc ?? x.CreatedAtUtc)
            .FirstOrDefaultAsync();

        if (draftVersion is null || string.IsNullOrWhiteSpace(draftVersion.WidgetVersionTokenId))
        {
            // Published question fallback supplied by the BLL via IQuestion_Service.
            return Ok(new
            {
                success = true,
                status = widget.WidgetStatusCode ?? "draft",
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

        var rows = await _dbContext.AskpearlsPortalUserWebsiteWidgetQuestionLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == ownerUserTokenId && x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetVersionTokenId == draftVersion.WidgetVersionTokenId &&
                        x.IsActive == true && x.IsVisible == true)
            .OrderBy(x => x.WidgetQuestionSortOrder).ToListAsync();
        var ids = rows.Select(x => x.WidgetQuestionTokenId).Where(x => x != null).ToArray();
        var answers = await _dbContext.AskpearlsPortalUserWebsiteWidgetQuestionAnswerLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == ownerUserTokenId && x.WidgetPublicKey == publicKey &&
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


    public async Task<WidgetOperationResult> SaveQuestionDraftAsync(string ownerUserTokenId, string publicKey, WidgetQuestionsDraftRequest? request)
    {
        var widget = await FindOwnedWidget(publicKey, ownerUserTokenId);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for this user." });
        if (!IsValidQuestionDraft(request))
            return BadRequest(new { success = false, message = "Questions are invalid. Each question needs a unique key, text, and at least two distinct options." });

        var versions = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
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
            _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(draft);
        }
        // Never share the published content token with editable question rows.
        // Also detach a design-only draft token, if one was created earlier.
        var publishedTokenId = published.SingleOrDefault()?.WidgetVersionTokenId;
        if (!string.IsNullOrWhiteSpace(draft.WidgetVersionTokenId) &&
            draft.WidgetVersionTokenId != publishedTokenId)
        {
            var oldQuestions = await _dbContext.AskpearlsPortalUserWebsiteWidgetQuestionLists
                .Where(x => x.OwnerUserTokenId == ownerUserTokenId && x.WidgetPublicKey == publicKey &&
                            x.WidgetVersionTokenId == draft.WidgetVersionTokenId && x.IsActive == true)
                .ToListAsync();
            var oldAnswers = await _dbContext.AskpearlsPortalUserWebsiteWidgetQuestionAnswerLists
                .Where(x => x.OwnerUserTokenId == ownerUserTokenId && x.WidgetPublicKey == publicKey &&
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
        _dbContext.AskpearlsPortalUserWebsiteWidgetQuestionLists.AddRange(questionRows);

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
        _dbContext.AskpearlsPortalUserWebsiteWidgetQuestionAnswerLists.AddRange(answerRows);
        await _dbContext.SaveChangesAsync();
        return Ok(new { success = true, status = "draft", versionNumber = draft.WidgetVersionNumber, questionCount = questionRows.Count });
    }


    public async Task<WidgetOperationResult> GetRecommendationDraftAsync(string ownerUserTokenId, string publicKey)
    {
        var widget = await FindOwnedWidget(publicKey, ownerUserTokenId);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for this user." });

        var versions = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey &&
                        x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        var version = versions.Where(x => x.WidgetStatusCode == "draft")
            .OrderByDescending(x => x.UpdatedAtUtc ?? x.CreatedAtUtc).FirstOrDefault()
            ?? versions.FirstOrDefault(x => x.WidgetStatusCode == "published" && x.WidgetVersionNumber == widget.WidgetPublishedVersion);
        if (version is null) return Conflict(new { success = false, message = "A published or draft widget version is required." });

        // Resolve recommendations using both version number and token. Existing
        // portal records are not all populated consistently across these fields.
        var widgetRows = await _dbContext.AskpearlsPortalUserWebsiteWidgetRecommendationLists.AsNoTracking()
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
        // Legacy saves may have inserted several visible rows with the same identifier.
        // Display the most recently updated row for each identifier. The next save
        // consolidates duplicate draft rows in the database as well.
        rows = rows
            .GroupBy(x => (x.RecommendationKey ?? x.RecommendationCode ?? "").Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderByDescending(x => x.UpdatedAtUtc ?? x.CreatedAtUtc)
                          .ThenByDescending(x => x.CreatedAtUtc).First())
            .OrderBy(x => x.RecommendationSortOrder).ThenBy(x => x.RecommendationName).ToList();

        var usedRecommendationKeys = await _dbContext.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey &&
                        x.IsVisible == true && x.RecommendationKey != null)
            .Select(x => x.RecommendationKey!).Distinct().ToListAsync();
        var usedRecommendationKeySet = usedRecommendationKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var servedCount = await _dbContext.AskpearlsPortalUserWebsiteWidgetEventLists.AsNoTracking()
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
            items = rows.Select((x, i) => new
            {
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


    public async Task<WidgetOperationResult> SaveRecommendationDraftAsync(string ownerUserTokenId, string publicKey, RecommendationDraftRequest? request)
    {
        var widget = await FindOwnedWidget(publicKey, ownerUserTokenId);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for this user." });
        if (request?.Items is null || request.Items.Count > 500 || request.Items.Any(x =>
                string.IsNullOrWhiteSpace(x.Name) || x.Name.Trim().Length > 150 ||
                string.IsNullOrWhiteSpace(x.Key) || !Regex.IsMatch(x.Key, "^[a-zA-Z][a-zA-Z0-9_-]{0,79}$") ||
                x.Description?.Length > 1000 || x.ImageUrl?.Length > 2000 || x.Url?.Length > 2000 || x.CtaText?.Length > 100) ||
            request.Items.Select(x => x.Key.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != request.Items.Count)
            return BadRequest(new { success = false, message = "Recommendation fields are invalid. Names and unique identifiers are required." });

        var versions = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
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
            _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(draft);
        }

        // Match draft rows by the version NUMBER or its token. Older saves can
        // carry a stale token after the draft version was updated elsewhere.
        // Never match published rows solely through a reused token.
        var oldDraftRows = await _dbContext.AskpearlsPortalUserWebsiteWidgetRecommendationLists
            .Where(x => x.OwnerUserTokenId == ownerUserTokenId &&
                        x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey &&
                        x.IsVisible == true &&
                        (x.WidgetVersionNumber == draft.WidgetVersionNumber ||
                         (x.WidgetVersionTokenId == draft.WidgetVersionTokenId &&
                          (x.WidgetVersionNumber == null || x.WidgetVersionNumber == ""))))
            .ToListAsync();

        // Existing identifiers are immutable. Keep the newest physical row,
        // hide legacy duplicates, and UPDATE that row instead of INSERTING.
        var existingByKey = new Dictionary<string, AskpearlsPortalUserWebsiteWidgetRecommendationList>(StringComparer.OrdinalIgnoreCase);
        foreach (var group in oldDraftRows
            .GroupBy(x => (x.RecommendationKey ?? x.RecommendationCode ?? "").Trim(), StringComparer.OrdinalIgnoreCase))
        {
            var ordered = group.OrderByDescending(x => x.UpdatedAtUtc ?? x.CreatedAtUtc)
                               .ThenByDescending(x => x.CreatedAtUtc).ToList();
            if (group.Key.Length > 0) existingByKey[group.Key] = ordered[0];
            foreach (var duplicate in ordered.Skip(group.Key.Length > 0 ? 1 : 0))
            {
                duplicate.IsVisible = false;
                duplicate.UpdatedAtUtc = now;
            }
        }

        var incomingKeys = request!.Items.Select(x => x.Key.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in existingByKey)
        {
            if (!incomingKeys.Contains(pair.Key))
            {
                pair.Value.IsVisible = false;
                pair.Value.UpdatedAtUtc = now;
            }
        }

        var savedRows = new List<AskpearlsPortalUserWebsiteWidgetRecommendationList>();
        for (var index = 0; index < request.Items.Count; index++)
        {
            var item = request.Items[index];
            var key = item.Key.Trim();
            if (existingByKey.TryGetValue(key, out var row))
            {
                // Preserve RowViewTokenId, RowDataTokenId, RecommendationTokenId,
                // CreatedAtUtc, and the immutable identifier.
                // Reconcile legacy version links while retaining row identity.
                row.WidgetVersionTokenId = draft.WidgetVersionTokenId;
                row.WidgetVersionNumber = draft.WidgetVersionNumber;
                row.RecommendationKey = key;
                row.RecommendationCode = key;
                row.RecommendationName = item.Name.Trim();
                row.RecommendationTitle = item.Name.Trim();
                row.RecommendationDescription = item.Description?.Trim();
                row.RecommendationShortDescription = item.Description?.Trim();
                row.RecommendationImageUrl = item.ImageUrl?.Trim();
                row.RecommendationUrl = item.Url?.Trim();
                row.RecommendationCtatext = item.CtaText?.Trim();
                row.RecommendationSortOrder = index + 1;
                row.ActiveStatus = item.Active ? "active" : "inactive";
                row.IsActive = item.Active;
                row.IsVisible = true;
                row.UpdatedAtUtc = now;
            }
            else
            {
                row = new AskpearlsPortalUserWebsiteWidgetRecommendationList
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
                    RecommendationKey = key,
                    RecommendationCode = key,
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
                };
                _dbContext.AskpearlsPortalUserWebsiteWidgetRecommendationLists.Add(row);
            }
            savedRows.Add(row);
        }
        draft.UpdatedAtUtc = now;
        await _dbContext.SaveChangesAsync();
        var ruleKeys = await _dbContext.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey &&
                        x.IsVisible == true && x.RecommendationKey != null)
            .Select(x => x.RecommendationKey!).Distinct().ToListAsync();
        var eventCount = await _dbContext.AskpearlsPortalUserWebsiteWidgetEventLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.IsVisible == true && x.IsActive == true &&
                        (x.RecommendationKey != null || x.RecommendationTokenId != null))
            .CountAsync();
        var ruleKeySet = ruleKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return Ok(new
        {
            success = true,
            status = "draft",
            versionNumber = draft.WidgetVersionNumber,
            itemCount = savedRows.Count,
            usedInRulesCount = request.Items.Count(item => ruleKeySet.Contains(item.Key.Trim())),
            recommendationsServedCount = eventCount
        });
    }


    public async Task<WidgetOperationResult> GetDecisionRuleDraftAsync(string ownerUserTokenId, string publicKey)
    {
        var widget = await FindOwnedWidget(publicKey, ownerUserTokenId);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for this user." });
        var versions = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists.AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey && x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        var version = versions.Where(x => x.WidgetStatusCode == "draft")
            .OrderByDescending(x => x.UpdatedAtUtc ?? x.CreatedAtUtc).FirstOrDefault()
            ?? versions.FirstOrDefault(x => x.WidgetStatusCode == "published" && x.WidgetVersionNumber == widget.WidgetPublishedVersion);
        if (version is null) return Conflict(new { success = false, message = "A published or draft widget version is required." });

        // Older rule records may be linked by version token (or one of the
        // legacy token columns) without a populated version number. Load this
        // widget's rows once, then resolve the selected draft/live version.
        var widgetRows = await _dbContext.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists.AsNoTracking()
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
                    conditions = group.Select(x => new
                    {
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


    public async Task<WidgetOperationResult> SaveDecisionRuleDraftAsync(string ownerUserTokenId, string publicKey, DecisionRulesDraftRequest? request)
    {
        var widget = await FindOwnedWidget(publicKey, ownerUserTokenId);
        if (widget is null) return NotFound(new { success = false, message = "Widget was not found for this user." });
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

        var versions = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
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
            _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(draft);
        }
        var oldRows = await _dbContext.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists
            .Where(x => x.OwnerUserTokenId == ownerUserTokenId && x.WidgetTokenId == widget.WidgetTokenId &&
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
        _dbContext.AskpearlsPortalUserWebsiteWidgetDecisionRuleLists.AddRange(rowsToSave);
        draft.UpdatedAtUtc = now;
        await _dbContext.SaveChangesAsync();
        return Ok(new { success = true, status = "draft", versionNumber = draft.WidgetVersionNumber, ruleCount = request.Rules.Count });
    }


    private async Task<AskpearlsPortalUserWebsiteWidgetList?> FindOwnedWidget(string publicKey, string ownerUserTokenId) =>
        await _dbContext.AskpearlsPortalUserWebsiteWidgetLists.FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
            x.OwnerUserTokenId == ownerUserTokenId && x.IsActive == true && x.IsVisible == true);

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




    private static bool HasPremiumBranding(WidgetDesignDraftRequest v) =>
        !string.IsNullOrWhiteSpace(v.BrandName) ||
        v.HeaderTitle != "AskPearls Assistant" ||
        !string.IsNullOrWhiteSpace(v.LogoDataUrl) ||
        !string.IsNullOrWhiteSpace(v.LogoUrl) ||
        v.BrandIcon != "stars" || v.HideAskPearlsBranding;

    private static bool IsValidLogoUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        if (url.Length > 2048 || !Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
        return uri.Scheme == Uri.UriSchemeHttps &&
               !string.IsNullOrWhiteSpace(uri.Host) &&
               uri.UserInfo.Length == 0 &&
               uri.HostNameType == UriHostNameType.Dns &&
               !uri.IsLoopback &&
               !uri.Host.EndsWith(".local", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidLogo(string? data) =>
        string.IsNullOrEmpty(data) ||
        (data.Length <= 110000 &&
         (data.StartsWith("data:image/png;base64,", StringComparison.Ordinal) ||
          data.StartsWith("data:image/jpeg;base64,", StringComparison.Ordinal) ||
          data.StartsWith("data:image/webp;base64,", StringComparison.Ordinal)) &&
         IsValidBase64Image(data));

    private static bool IsValidBase64Image(string data)
    {
        try
        {
            var bytes = Convert.FromBase64String(data[(data.IndexOf(',') + 1)..]);
            if (bytes.Length > 80000 || bytes.Length < 12) return false;
            if (data.StartsWith("data:image/png;")) return bytes.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
            if (data.StartsWith("data:image/jpeg;")) return bytes[0] == 255 && bytes[1] == 216;
            if (data.StartsWith("data:image/webp;")) return System.Text.Encoding.ASCII.GetString(bytes, 0, 4) == "RIFF" && System.Text.Encoding.ASCII.GetString(bytes, 8, 4) == "WEBP";
            return false;
        }
        catch (FormatException) { return false; }
    }

    private static bool IsValidDesign(WidgetDesignDraftRequest? value)
    {
        if (value is null)
            return false;

        if (!IsHex(value.Primary) ||
            !IsHex(value.Background) ||
            !IsHex(value.Text))
            return false;

        if (value.FontSize is < 12 or > 18 ||
            value.Radius is < 0 or > 28 ||
            value.Width is < 300 or > 520)
            return false;

        if (value.Position is not (
            "bottom-right" or
            "bottom-left" or
            "center"))
            return false;

        if (value.ButtonShape is not (
            "rounded" or
            "pill" or
            "square"))
            return false;

        if (value.Theme is not (
            "custom" or
            "light" or
            "dark"))
            return false;

        if (value.Heading is null ||
            value.Description is null ||
            value.ButtonText is null ||
            value.Footer is null)
            return false;

        string[] allowedFonts =
        {
        "system",
        "Inter, Arial, sans-serif",
        "Arial, sans-serif",
        "Georgia, serif",
        "Verdana, sans-serif"
    };

        if (!allowedFonts.Contains(
            value.Font,
            StringComparer.Ordinal))
            return false;

        if (value.BrandName is null || value.HeaderTitle is null ||
            value.LogoDataUrl is null || value.LogoUrl is null || value.BrandIcon is null ||
            value.BrandName.Length > 100 || value.HeaderTitle.Length > 80 ||
            value.BrandIcon is not ("stars" or "building" or "chat" or "robot" or "custom") ||
            !IsValidLogoUrl(value.LogoUrl) ||
            !IsValidLogo(value.LogoDataUrl)) return false;

        return value.Heading.Length <= 100 &&
               value.Description.Length <= 250 &&
               value.ButtonText.Length <= 40 &&
               value.Footer.Length <= 80;
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



}

using AskPearls.Domain;

using AskPearlsCS_BLL.app_Service;

using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Globalization;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "ApiJwt")]
[Route("api/widgets")]
public class WidgetController : ControllerBase
{
    private readonly IWidget_Service _widgetService;
    private readonly IWidgetVersion_Service _widgetVersionService;
    private readonly IQuestion_Service _questionService;

    public WidgetController(
        IWidget_Service widgetService,
        IWidgetVersion_Service widgetVersionService,
        IQuestion_Service questionService)
    {
        _widgetService = widgetService;
        _widgetVersionService = widgetVersionService;
        _questionService = questionService;
    }

    private IActionResult FromOperation(WidgetOperationResult result)
    {
        return result.Outcome switch
        {
            WidgetOperationOutcome.Ok => Ok(result.Body),
            WidgetOperationOutcome.Created => Created(result.Location ?? string.Empty, result.Body),
            WidgetOperationOutcome.Invalid => BadRequest(result.Body),
            WidgetOperationOutcome.NotFound => NotFound(result.Body),
            WidgetOperationOutcome.Conflict => Conflict(result.Body),
            _ => StatusCode(500, new { success = false, message = "Unexpected widget operation outcome." })
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWidgets()
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.GetMyWidgetsAsync(ownerUserTokenId);
        return FromOperation(result);
    }

    [HttpGet("websites")]
    public async Task<IActionResult> GetMyWebsites()
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.GetMyWebsitesAsync(ownerUserTokenId);
        return FromOperation(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWidget([FromBody] CreateWidgetRequest? request)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.CreateWidgetAsync(ownerUserTokenId, request);
        return FromOperation(result);
    }

    [HttpPut("{publicKey}/basics")]
    public async Task<IActionResult> UpdateWidgetBasics(string publicKey, [FromBody] UpdateWidgetBasicsRequest? request)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error)) return error!;
        return FromOperation(await _widgetService.UpdateWidgetBasicsAsync(ownerUserTokenId, publicKey, request));
    }

    [HttpPut("{publicKey}/draft/configuration")]
    public async Task<IActionResult> SaveDesignDraft(string publicKey, [FromBody] WidgetDesignDraftRequest? request)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.SaveDesignDraftAsync(ownerUserTokenId, publicKey, request);
        return FromOperation(result);
    }

    [HttpPost("{publicKey}/publish")]
    public async Task<IActionResult> PublishDesignDraft(string publicKey)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.PublishDesignDraftAsync(ownerUserTokenId, publicKey);
        return FromOperation(result);
    }

    [HttpGet("{publicKey}/draft/configuration")]
    public async Task<IActionResult> GetDesignDraft(string publicKey)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.GetDesignDraftAsync(ownerUserTokenId, publicKey);
        return FromOperation(result);
    }

    [HttpGet("{publicKey}/draft/questions")]
    public async Task<IActionResult> GetQuestionDraft(string publicKey)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        // Question_Service depends on IWidget_Service, so keep this call in the
        // controller to avoid a circular DI dependency between services.
        var publishedQuestions = await _questionService.GetPublishedQuestionsAsync(publicKey);
        var result = await _widgetService.GetQuestionDraftAsync(ownerUserTokenId, publicKey, publishedQuestions);
        return FromOperation(result);
    }

    [HttpPut("{publicKey}/draft/questions")]
    public async Task<IActionResult> SaveQuestionDraft(string publicKey, [FromBody] WidgetQuestionsDraftRequest? request)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.SaveQuestionDraftAsync(ownerUserTokenId, publicKey, request);
        return FromOperation(result);
    }

    [HttpGet("{publicKey}/draft/recommendations")]
    public async Task<IActionResult> GetRecommendationDraft(string publicKey)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.GetRecommendationDraftAsync(ownerUserTokenId, publicKey);
        return FromOperation(result);
    }

    [HttpPut("{publicKey}/draft/recommendations")]
    public async Task<IActionResult> SaveRecommendationDraft(string publicKey, [FromBody] RecommendationDraftRequest? request)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.SaveRecommendationDraftAsync(ownerUserTokenId, publicKey, request);
        return FromOperation(result);
    }

    [HttpGet("{publicKey}/draft/rules")]
    public async Task<IActionResult> GetDecisionRuleDraft(string publicKey)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.GetDecisionRuleDraftAsync(ownerUserTokenId, publicKey);
        return FromOperation(result);
    }

    [HttpPut("{publicKey}/draft/rules")]
    public async Task<IActionResult> SaveDecisionRuleDraft(string publicKey, [FromBody] DecisionRulesDraftRequest? request)
    {
        if (!TryGetAuthenticatedOwner(out var ownerUserTokenId, out var error))
            return error!;

        var result = await _widgetService.SaveDecisionRuleDraftAsync(ownerUserTokenId, publicKey, request);
        return FromOperation(result);
    }

    private bool TryGetAuthenticatedOwner(out string ownerUserTokenId, out IActionResult? error)
    {
        ownerUserTokenId = string.Empty;
        error = null;

        // This claim is the AskPearls database user ID issued by the authenticated Portal.
        var claimValue = User.FindFirst(AskPearlsClaimTypes.ASKPEARLS_USER_ID)?.Value;
        if (long.TryParse(
                claimValue,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var userId) &&
            userId > 0)
        {
            ownerUserTokenId = userId.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        error = Unauthorized(new
        {
            success = false,
            message = "The access token does not contain a valid AskPearlsUserId claim."
        });
        return false;
    }

    [AllowAnonymous]
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




    [AllowAnonymous]
    [HttpGet("{publicKey}/design")]
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





    [AllowAnonymous]
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







}

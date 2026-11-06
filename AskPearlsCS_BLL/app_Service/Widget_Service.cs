using AskPearls.Domain;

using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_BLL.app_Service;

public class Widget_Service : IWidget_Service
{
    private readonly IWidget_Repository _widgetRepository;

    public Widget_Service(IWidget_Repository widgetRepository)
    {
        _widgetRepository = widgetRepository;
    }

    public async Task<Widget?> GetPublishedWidgetAsync(string publicKey)
        => string.IsNullOrWhiteSpace(publicKey) ? null : await _widgetRepository.GetPublishedWidgetAsync(publicKey);

    public Task<List<Widget>> GetByCompanyTokenIdAsync(string companyTokenId, CancellationToken cancellationToken = default)
        => _widgetRepository.GetByCompanyTokenIdAsync(companyTokenId, cancellationToken);

    public Task<List<Widget>> GetByOwnerUserTokenIdAsync(string ownerUserTokenId, CancellationToken cancellationToken = default)
        => _widgetRepository.GetByOwnerUserTokenIdAsync(ownerUserTokenId, cancellationToken);

    public Task<WidgetOperationResult> GetMyWidgetsAsync(string ownerUserTokenId)
        => _widgetRepository.GetMyWidgetsAsync(ownerUserTokenId);

    public Task<WidgetOperationResult> GetMyWebsitesAsync(string ownerUserTokenId)
        => _widgetRepository.GetMyWebsitesAsync(ownerUserTokenId);

    public Task<WidgetOperationResult> CreateWidgetAsync(string ownerUserTokenId, CreateWidgetRequest? request)
        => _widgetRepository.CreateWidgetAsync(ownerUserTokenId, request);

    public Task<WidgetOperationResult> SaveDesignDraftAsync(string ownerUserTokenId, string publicKey, WidgetDesignDraftRequest? request)
        => _widgetRepository.SaveDesignDraftAsync(ownerUserTokenId, publicKey, request);

    public Task<WidgetOperationResult> PublishDesignDraftAsync(string ownerUserTokenId, string publicKey)
        => _widgetRepository.PublishDesignDraftAsync(ownerUserTokenId, publicKey);

    public Task<WidgetOperationResult> GetDesignDraftAsync(string ownerUserTokenId, string publicKey)
        => _widgetRepository.GetDesignDraftAsync(ownerUserTokenId, publicKey);

    public Task<WidgetOperationResult> GetQuestionDraftAsync(string ownerUserTokenId, string publicKey, IReadOnlyList<Question> publishedQuestions)
        => _widgetRepository.GetQuestionDraftAsync(ownerUserTokenId, publicKey, publishedQuestions);

    public Task<WidgetOperationResult> SaveQuestionDraftAsync(string ownerUserTokenId, string publicKey, WidgetQuestionsDraftRequest? request)
        => _widgetRepository.SaveQuestionDraftAsync(ownerUserTokenId, publicKey, request);

    public Task<WidgetOperationResult> GetRecommendationDraftAsync(string ownerUserTokenId, string publicKey)
        => _widgetRepository.GetRecommendationDraftAsync(ownerUserTokenId, publicKey);

    public Task<WidgetOperationResult> SaveRecommendationDraftAsync(string ownerUserTokenId, string publicKey, RecommendationDraftRequest? request)
        => _widgetRepository.SaveRecommendationDraftAsync(ownerUserTokenId, publicKey, request);

    public Task<WidgetOperationResult> GetDecisionRuleDraftAsync(string ownerUserTokenId, string publicKey)
        => _widgetRepository.GetDecisionRuleDraftAsync(ownerUserTokenId, publicKey);

    public Task<WidgetOperationResult> SaveDecisionRuleDraftAsync(string ownerUserTokenId, string publicKey, DecisionRulesDraftRequest? request)
        => _widgetRepository.SaveDecisionRuleDraftAsync(ownerUserTokenId, publicKey, request);

}

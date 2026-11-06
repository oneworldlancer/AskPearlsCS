

using AskPearls.Domain;

using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_BLL.app_Service;

public interface IWidget_Service
{
    Task<Widget?> GetPublishedWidgetAsync(string publicKey);
    Task<List<Widget>> GetByCompanyTokenIdAsync(string companyTokenId, CancellationToken cancellationToken = default);
    Task<List<Widget>> GetByOwnerUserTokenIdAsync(string ownerUserTokenId, CancellationToken cancellationToken = default);

    Task<WidgetOperationResult> GetMyWidgetsAsync(string ownerUserTokenId);
    Task<WidgetOperationResult> GetMyWebsitesAsync(string ownerUserTokenId);
    Task<WidgetOperationResult> CreateWidgetAsync(string ownerUserTokenId, CreateWidgetRequest? request);
    Task<WidgetOperationResult> SaveDesignDraftAsync(string ownerUserTokenId, string publicKey, WidgetDesignDraftRequest? request);
    Task<WidgetOperationResult> PublishDesignDraftAsync(string ownerUserTokenId, string publicKey);
    Task<WidgetOperationResult> GetDesignDraftAsync(string ownerUserTokenId, string publicKey);
    Task<WidgetOperationResult> GetQuestionDraftAsync(string ownerUserTokenId, string publicKey, IReadOnlyList<Question> publishedQuestions);
    Task<WidgetOperationResult> SaveQuestionDraftAsync(string ownerUserTokenId, string publicKey, WidgetQuestionsDraftRequest? request);
    Task<WidgetOperationResult> GetRecommendationDraftAsync(string ownerUserTokenId, string publicKey);
    Task<WidgetOperationResult> SaveRecommendationDraftAsync(string ownerUserTokenId, string publicKey, RecommendationDraftRequest? request);
    Task<WidgetOperationResult> GetDecisionRuleDraftAsync(string ownerUserTokenId, string publicKey);
    Task<WidgetOperationResult> SaveDecisionRuleDraftAsync(string ownerUserTokenId, string publicKey, DecisionRulesDraftRequest? request);
}

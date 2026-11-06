using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AskPearlsCS_DOMAIN.app_ContractsManager;

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
    [property: JsonPropertyName("theme")] string Theme = "custom",

    [property: JsonPropertyName("showTitle")] bool ShowTitle = true,
    [property: JsonPropertyName("showDescription")] bool ShowDescription = true,
    [property: JsonPropertyName("showIntroduction")] bool ShowIntroduction = true
);

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


/// <summary>HTTP-independent result for widget application operations.</summary>
public enum WidgetOperationOutcome
{
    Ok,
    Created,
    Invalid,
    NotFound,
    Conflict
}

public sealed record WidgetOperationResult(
    WidgetOperationOutcome Outcome,
    object? Body,
    string? Location = null);

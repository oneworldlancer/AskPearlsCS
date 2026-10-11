using AskPearls.Domain;

using AskPearlsCS_DAL.DBModels;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AskPearlsCS_DAL.app_Mapper
{
    public static class RecommendationItem_Mapper
    {
        public static RecommendationItem ToDomain(
            AskpearlsPortalUserWebsiteWidgetRecommendationList source)
        {
            return new RecommendationItem
            {
                Id = source.RowViewTokenId,

                RecommendationTokenId =
                    source.RecommendationTokenId ?? string.Empty,

                Key = source.RecommendationKey ?? string.Empty,
                Code = source.RecommendationCode ?? string.Empty,
                Type = source.RecommendationType ?? string.Empty,

                Name = source.RecommendationName ?? string.Empty,
                Title = source.RecommendationTitle ?? string.Empty,

                Text = source.RecommendationText,
                ShortDescription = source.RecommendationShortDescription,
                Description = source.RecommendationDescription,

                ImageTokenId = source.RecommendationImageTokenId
                               ?? source.ImageTokenId,

                ImageUrl = source.RecommendationImageUrl
                           ?? source.ImageServerUrl,

                Url = source.RecommendationUrl,

                CtaText = source.RecommendationCtatext
                          ?? source.RecommendationCta
                          ?? source.RecommendationCtacode,

                HighlightsJson = source.RecommendationHighlightsJson,
                Highlights = ParseHighlights(
                    source.RecommendationHighlightsJson),

                WidgetTokenId = source.WidgetTokenId,
                WidgetVersionTokenId = source.WidgetVersionTokenId,

                SortOrder = source.RecommendationSortOrder ?? 0,

                IsActive = source.IsActive == true,
                IsVisible = source.IsVisible == true,

                CreatedAtUtc = source.CreatedAtUtc ?? DateTime.UtcNow,
                UpdatedAtUtc = source.UpdatedAtUtc ?? DateTime.UtcNow
            };
        }

        private static List<string> ParseHighlights(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json)
                       ?? new List<string>();
            }
            catch (JsonException)
            {
                return new List<string>();
            }
        }
    }
}

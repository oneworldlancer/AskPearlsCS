using AskPearls.Domain;

using AskPearlsCS_DAL.DBModels;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Mapper
{

    public static class WidgetAnswer_Mapper
    {
        public static Answer? ToDomain(
            AskpearlsPortalUserWebsiteWidgetQuestionAnswerList? entity)
        {
            if (entity == null)
                return null;

            return new Answer
            {
                // Internal row identity
                Id = entity.RowViewTokenId,

                // Public/application token identifiers remain strings
                AnswerTokenId = entity.WidgetAnswerTokenId ?? string.Empty,
                WidgetTokenId = entity.WidgetTokenId ?? string.Empty,
                QuestionTokenId = entity.WidgetQuestionTokenId ?? string.Empty,
                WidgetVersionTokenId = entity.WidgetVersionTokenId ?? string.Empty,

                Code = entity.WidgetAnswerCode ?? string.Empty,
                Key = entity.WidgetAnswerKey ?? string.Empty,
                Type = entity.WidgetAnswerType ?? string.Empty,
                Title = entity.WidgetAnswerTitle ?? string.Empty,
                Name = entity.WidgetAnswerName ?? string.Empty,
                Text = entity.WidgetAnswerText ?? string.Empty,
                Description = entity.WidgetAnswerDescription ?? string.Empty,

                SortOrder = entity.WidgetAnswerSortOrder ?? 0,

                ImageTokenId = entity.ImageTokenId,
                ImageServerUrl = entity.ImageServerUrl,

                IsActive = entity.IsActive == true,
                IsVisible = entity.IsVisible == true,

                CreatedAtUtc = entity.CreatedAtUtc ?? DateTime.UtcNow,
                UpdatedAtUtc = entity.UpdatedAtUtc ?? DateTime.UtcNow
            };
        }

        public static IReadOnlyList<Answer> ToDomainList(
            IEnumerable<AskpearlsPortalUserWebsiteWidgetQuestionAnswerList> entities)
        {
            return entities
                .Select(ToDomain)
                .Where(answer => answer != null)
                .Select(answer => answer!)
                .ToList();
        }
    }

}

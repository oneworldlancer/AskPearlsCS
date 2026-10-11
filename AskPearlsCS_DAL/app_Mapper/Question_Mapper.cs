using AskPearls.Domain;

using AskPearlsCS_DAL.DBModels;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Mapper
{
    public static class Question_Mapper
    {
        public static Question? ToDomain(
            AskpearlsPortalUserWebsiteWidgetQuestionList? entity)
        {
            if (entity == null)
                return null;

            return new Question
            {
                Id = entity.RowViewTokenId,

                QuestionTokenId =
                    entity.WidgetQuestionTokenId ?? string.Empty,

                WidgetTokenId =
                    entity.WidgetTokenId ?? string.Empty,

                WidgetVersionTokenId =
                    entity.WidgetVersionTokenId ?? string.Empty,

                Code = entity.WidgetQuestionCode ?? string.Empty,
                Key = entity.WidgetQuestionKey ?? string.Empty,
                Type = entity.WidgetQuestionType ?? string.Empty,
                Title = entity.WidgetQuestionTitle ?? string.Empty,
                Name = entity.WidgetQuestionName ?? string.Empty,
                Text = entity.WidgetQuestionText ?? string.Empty,

                Description =
                    entity.WidgetQuestionDescription ?? string.Empty,

                SortOrder = entity.WidgetQuestionSortOrder ?? 0,

                IsActive = entity.IsActive == true,
                IsVisible = entity.IsVisible == true,

                CreatedAtUtc =
                    entity.CreatedAtUtc ?? DateTime.UtcNow,

                UpdatedAtUtc =
                    entity.UpdatedAtUtc ?? DateTime.UtcNow,

                Answers = new List<Answer>()
            };
        }
    }
}

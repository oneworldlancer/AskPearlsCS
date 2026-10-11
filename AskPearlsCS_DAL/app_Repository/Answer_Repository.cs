using AskPearls.Domain;

using AskPearlsCS_DAL.app_Mapper;
using AskPearlsCS_DAL.DBContext;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{

    public class Answer_Repository : IAnswer_Repository
    {
        private readonly DBContextModel _dbContext;

        public Answer_Repository(DBContextModel dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Answer>> GetActiveByQuestionAsync(
            string publicKey,
            string questionTokenId,
            string widgetVersionTokenId)
        {
            if (string.IsNullOrWhiteSpace(publicKey) ||
                string.IsNullOrWhiteSpace(questionTokenId) ||
                string.IsNullOrWhiteSpace(widgetVersionTokenId))
            {
                return Array.Empty<Answer>();
            }

            var entities = await _dbContext
                .AskpearlsPortalUserWebsiteWidgetQuestionAnswerLists
                .AsNoTracking()
                .Where(x =>
                    x.WidgetPublicKey == publicKey &&
                    x.WidgetQuestionTokenId == questionTokenId &&
                    x.WidgetVersionTokenId == widgetVersionTokenId &&
                    x.IsActive == true &&
                    x.IsVisible == true)
                .OrderBy(x => x.WidgetAnswerSortOrder)
                .ToListAsync();

            return WidgetAnswer_Mapper.ToDomainList(entities);
        }
    }

}

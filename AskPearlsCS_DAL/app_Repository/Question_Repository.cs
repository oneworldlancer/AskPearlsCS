using AskPearls.Domain;

using AskPearlsCS_DAL.app_Mapper;
using AskPearlsCS_DAL.DBContext;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{

    public class Question_Repository : IQuestion_Repository
    {
        private readonly DBContextModel _dbContext;

        public Question_Repository(DBContextModel dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Question>> GetActiveByVersionAsync(
            string publicKey,
            string widgetTokenId,
            string widgetVersionTokenId)
        {
            if (string.IsNullOrWhiteSpace(publicKey) ||
                string.IsNullOrWhiteSpace(widgetTokenId) ||
                string.IsNullOrWhiteSpace(widgetVersionTokenId))
            {
                return Array.Empty<Question>();
            }

            var entities = await _dbContext
                .AskpearlsPortalUserWebsiteWidgetQuestionLists
                .AsNoTracking()
                .Where(x =>
                    x.WidgetPublicKey == publicKey &&
                    x.WidgetTokenId == widgetTokenId &&
                    x.WidgetVersionTokenId == widgetVersionTokenId &&
                    x.IsActive == true &&
                    x.IsVisible == true)
                .OrderBy(x => x.WidgetQuestionSortOrder)
                .ToListAsync();

            return entities
                .Select(Question_Mapper.ToDomain)
                .Where(question => question != null)
                .Select(question => question!)
                .ToList();
        }
    }
}

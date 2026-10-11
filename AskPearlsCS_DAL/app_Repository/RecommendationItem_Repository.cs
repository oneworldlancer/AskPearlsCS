using AskPearls.Domain;

using AskPearlsCS_DAL.app_Mapper;
using AskPearlsCS_DAL.DBContext;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AskPearlsCS_DAL.app_Repository
{

    public class RecommendationItem_Repository : IRecommendationItem_Repository
    {
        private readonly DBContextModel _context;

        public RecommendationItem_Repository(DBContextModel context)
        {
            _context = context;
        }

        public async Task<List<RecommendationItem>>
            GetActiveByWidgetVersionAsync(
                string widgetPublicKey,
                string widgetTokenId,
                string widgetVersionTokenId,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(widgetPublicKey) ||
                string.IsNullOrWhiteSpace(widgetTokenId) ||
                string.IsNullOrWhiteSpace(widgetVersionTokenId))
            {
                return new List<RecommendationItem>();
            }

            var rows = await _context
                .AskpearlsPortalUserWebsiteWidgetRecommendationLists
                .AsNoTracking()
                .Where(x =>
                    x.WidgetPublicKey == widgetPublicKey &&
                    x.WidgetTokenId == widgetTokenId &&
                    x.WidgetVersionTokenId == widgetVersionTokenId &&
                    x.IsActive == true &&
                    x.IsVisible == true)
                .OrderBy(x => x.RecommendationSortOrder)
                .ThenBy(x => x.RecommendationName)
                .ToListAsync(cancellationToken);

            return rows
                .Select(RecommendationItem_Mapper.ToDomain)
                .ToList();
        }
    }
}

using AskPearls.Domain;

using AskPearlsCS_DAL.app_Mapper;
using AskPearlsCS_DAL.DBContext;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{
    public class WidgetVersion_Repository : IWidgetVersion_Repository
    {
        private readonly DBContextModel _dbContext;

        public WidgetVersion_Repository(DBContextModel dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WidgetVersion?> GetPublishedVersionAsync(
            Guid widgetId,
            string widgetTokenId,
            string publicKey,
            int publishedVersion)
        {
            if (widgetId == Guid.Empty ||
                string.IsNullOrWhiteSpace(widgetTokenId) ||
                string.IsNullOrWhiteSpace(publicKey) ||
                publishedVersion <= 0)
            {
                return null;
            }

            var entities = await _dbContext
                .AskpearlsPortalUserWebsiteWidgetVersionLists
                .AsNoTracking()
                .Where(x =>
                    x.WidgetTokenId == widgetTokenId &&
                    x.WidgetPublicKey == publicKey &&
                    x.WidgetVersionNumber == publishedVersion.ToString() &&
                    x.WidgetStatusCode == "published" &&
                    x.IsActive == true &&
                    x.IsVisible == true)
                .Take(2)
                .ToListAsync();

            // Do not select an arbitrary version if duplicate rows exist.
            if (entities.Count != 1)
                return null;

            return WidgetVersion_Mapper.ToDomain(
                entities[0],
                widgetId);
        }
    }

}

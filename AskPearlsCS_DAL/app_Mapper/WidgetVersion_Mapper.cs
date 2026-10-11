using AskPearls.Domain;

using AskPearlsCS_DAL.DBModels;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Mapper
{


    public static class WidgetVersion_Mapper
    {
        public static WidgetVersion? ToDomain(
            AskpearlsPortalUserWebsiteWidgetVersionList? entity,
            Guid widgetId)
        {
            if (entity == null)
                return null;

            if (!int.TryParse(entity.WidgetVersionNumber, out var versionNumber))
                return null;

            return new WidgetVersion
            {
                Id = entity.RowViewTokenId,
                WidgetId = widgetId,
                VersionTokenId = entity.WidgetVersionTokenId ?? string.Empty,
                VersionNumber = versionNumber,
                Status = entity.WidgetStatusCode ?? string.Empty,
                ConfigurationJson = entity.WidgetConfigurationJson ?? "{}",
                PublishedAtUtc = null,
                CreatedAtUtc = entity.CreatedAtUtc ?? DateTime.UtcNow,
                UpdatedAtUtc = entity.UpdatedAtUtc ?? DateTime.UtcNow
            };
        }
    }

}

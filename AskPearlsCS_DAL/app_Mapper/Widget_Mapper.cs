using AskPearls.Domain;

using AskPearlsCS_DAL.DBModels;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Mapper
{
    public static class Widget_Mapper
    {
        public static Widget? ToDomain(
            AskpearlsPortalUserWebsiteWidgetList? entity)
        {
            if (entity == null)
                return null;

            int.TryParse(
                entity.WidgetPublishedVersion,
                out var publishedVersion);

            return new Widget
            {
                // Internal entity/database identity
                Id = entity.RowViewTokenId,

                // AskPearls business identity
                WidgetTokenId =
                    entity.WidgetTokenId ?? string.Empty,

                // Organization / company
                // Keep these as Guid only if their token values
                // are actually Guid strings.
                OrganizationId = ParseGuid(entity.CompanyTokenId),

                // Website business identity
                WebsiteId = ParseGuid(entity.WebsiteTokenId),

                WidgetCode =
                    entity.WidgetCode ?? string.Empty,

                WidgetType =
                    entity.WidgetType ?? string.Empty,

                WidgetName =
                    entity.WidgetName ?? string.Empty,

                WidgetTitle =
                    entity.WidgetTitle ?? string.Empty,

                WidgetDescription =
                    entity.WidgetDescription ?? string.Empty,

                WidgetPublicKey =
                    entity.WidgetPublicKey ?? string.Empty,

                WidgetStatusCode =
                    entity.WidgetStatusCode ?? string.Empty,

            
                WidgetStatusText =
                    entity.WidgetStatusText ?? string.Empty,

                WidgetPublishedVersion =
                    publishedVersion
            };
        }

        private static Guid ParseGuid(string? value)
        {
            return Guid.TryParse(value, out var result)
                ? result
                : Guid.Empty;
        }
    }
}

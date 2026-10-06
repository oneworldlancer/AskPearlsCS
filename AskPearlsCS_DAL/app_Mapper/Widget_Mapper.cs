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
                Id = entity.RowViewTokenId,

                OrganizationId = Guid.TryParse(
                    entity.CompanyTokenId,
                    out var organizationId)
                        ? organizationId
                        : Guid.Empty,

                WebsiteId = Guid.TryParse(
                    entity.WebsiteTokenId,
                    out var websiteId)
                        ? websiteId
                        : Guid.Empty,

                WidgetTokenId =
                    entity.WidgetTokenId ?? string.Empty,

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

                WidgetStatus =
                    entity.WidgetStatus ?? string.Empty,

                WidgetPublishedVersion =
                    publishedVersion
            };
        }
    }
}

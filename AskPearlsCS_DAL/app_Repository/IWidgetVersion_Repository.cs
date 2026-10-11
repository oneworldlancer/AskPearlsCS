using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{
    public interface IWidgetVersion_Repository
    {
        Task<WidgetVersion?> GetPublishedVersionAsync(
            Guid widgetId,
            string widgetTokenId,
            string publicKey,
            int publishedVersion);
    }
}

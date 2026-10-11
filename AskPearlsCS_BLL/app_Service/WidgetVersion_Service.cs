using AskPearls.Domain;

using AskPearlsCS_DAL.app_Repository;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{
    public class WidgetVersion_Service : IWidgetVersion_Service
    {
        private readonly IWidget_Service _widgetService;
        private readonly IWidgetVersion_Repository _widgetVersionRepository;

        public WidgetVersion_Service(
            IWidget_Service widgetService,
            IWidgetVersion_Repository widgetVersionRepository)
        {
            _widgetService = widgetService;
            _widgetVersionRepository = widgetVersionRepository;
        }

        public async Task<WidgetVersion?> GetPublishedVersionAsync(
            string publicKey)
        {
            if (string.IsNullOrWhiteSpace(publicKey))
                return null;

            // Resolve the published widget first.
            var widget = await _widgetService
                .GetPublishedWidgetAsync(publicKey);

            if (widget == null || widget.WidgetPublishedVersion <= 0)
                return null;

            // Resolve the corresponding published version.
            return await _widgetVersionRepository.GetPublishedVersionAsync(
                widget.Id,
                widget.WidgetTokenId,
                widget.WidgetPublicKey,
                widget.WidgetPublishedVersion);
        }
    }
}

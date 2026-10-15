using AskPearls.Domain;

using AskPearlsCS_DAL.app_Repository;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{

    public class Widget_Service : IWidget_Service
    {
        private readonly IWidget_Repository _widgetRepository;

        public Widget_Service(
            IWidget_Repository widgetRepository)
        {
            _widgetRepository = widgetRepository;
        }

        public async Task<Widget?> GetPublishedWidgetAsync(
            string publicKey)
        {
            if (string.IsNullOrWhiteSpace(publicKey))
                return null;

            return await _widgetRepository
                .GetPublishedWidgetAsync(publicKey);
        }



        public async Task<List<Widget>> GetByCompanyTokenIdAsync(
            string companyTokenId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(companyTokenId) ||
                companyTokenId == "0")
            {
                return new List<Widget>();
            }

            return await _widgetRepository.GetByCompanyTokenIdAsync(
                companyTokenId,
                cancellationToken);
        }

    }
}

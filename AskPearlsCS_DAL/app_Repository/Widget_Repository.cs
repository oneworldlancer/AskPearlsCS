using AskPearls.Domain;

using AskPearlsCS_DAL.app_Mapper;
using AskPearlsCS_DAL.DBContext;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{
    public class Widget_Repository : IWidget_Repository
    {
        private readonly DBContextModel _dbContext;

        public Widget_Repository(DBContextModel dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Widget?> GetPublishedWidgetAsync(
            string publicKey)
        {
            var entity = await _dbContext
                .AskpearlsPortalUserWebsiteWidgetLists
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.WidgetPublicKey == publicKey &&
                    x.WidgetStatusCode == "published" &&
                    x.IsActive == true &&
                    x.IsVisible == true);

            return Widget_Mapper.ToDomain(entity);
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

            var entities = await _dbContext
                .AskpearlsPortalUserWebsiteWidgetLists
                .AsNoTracking()
                .Where(x =>
                    x.CompanyTokenId == companyTokenId &&
                    x.IsActive == true &&
                    x.IsVisible == true)
                .OrderByDescending(x => x.UploadDate)
                .ToListAsync(cancellationToken);

            return entities
                .Select(Widget_Mapper.ToDomain)
                .Where(x => x != null)
                .Select(x => x!)
                .ToList();
        }



    }

}

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
                    x.WidgetStatus == "Published");

            return Widget_Mapper.ToDomain(entity);
        }
    }

}

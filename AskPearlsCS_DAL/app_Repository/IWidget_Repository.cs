using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{
    public interface IWidget_Repository
    {
        Task<Widget?> GetPublishedWidgetAsync(
            string publicKey);
    }
}

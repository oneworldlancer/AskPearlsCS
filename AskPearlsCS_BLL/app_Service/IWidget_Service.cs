using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{
    public interface IWidget_Service
    {
        Task<Widget?> GetPublishedWidgetAsync(
            string publicKey);
    }
}

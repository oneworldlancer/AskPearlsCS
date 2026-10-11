using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{

    public interface IWidgetVersion_Service
    {
        Task<WidgetVersion?> GetPublishedVersionAsync(string publicKey);
    }
}

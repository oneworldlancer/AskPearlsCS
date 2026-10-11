using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{
    public interface IQuestion_Repository
    {
        Task<IReadOnlyList<Question>> GetActiveByVersionAsync(
            string publicKey,
            string widgetTokenId,
            string widgetVersionTokenId);
    }
}

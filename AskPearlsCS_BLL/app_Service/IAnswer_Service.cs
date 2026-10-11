using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{
    public interface IAnswer_Service
    {
        Task<IReadOnlyList<Answer>> GetActiveByQuestionAsync(
            string publicKey,
            string questionTokenId,
            string widgetVersionTokenId);
    }
}

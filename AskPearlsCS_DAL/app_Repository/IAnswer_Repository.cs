using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{
    public interface IAnswer_Repository
    {
        Task<IReadOnlyList<Answer>> GetActiveByQuestionAsync(
            string publicKey,
            string questionTokenId,
            string widgetVersionTokenId);
    }
}

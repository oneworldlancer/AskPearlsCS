using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{
    public interface IQuestion_Service
    {
        Task<IReadOnlyList<Question>> GetPublishedQuestionsAsync(
            string publicKey);
    }
}

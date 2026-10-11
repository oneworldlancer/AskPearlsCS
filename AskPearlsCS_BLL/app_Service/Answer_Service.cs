using AskPearls.Domain;

using AskPearlsCS_DAL.app_Repository;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{
    public class Answer_Service : IAnswer_Service
    {
        private readonly IAnswer_Repository _answerRepository;

        public Answer_Service(IAnswer_Repository answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public async Task<IReadOnlyList<Answer>> GetActiveByQuestionAsync(
            string publicKey,
            string questionTokenId,
            string widgetVersionTokenId)
        {
            if (string.IsNullOrWhiteSpace(publicKey) ||
                string.IsNullOrWhiteSpace(questionTokenId) ||
                string.IsNullOrWhiteSpace(widgetVersionTokenId))
            {
                return Array.Empty<Answer>();
            }

            return await _answerRepository.GetActiveByQuestionAsync(
                publicKey,
                questionTokenId,
                widgetVersionTokenId);
        }
    }

}

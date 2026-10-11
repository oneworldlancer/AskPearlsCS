using AskPearls.Domain;

using AskPearlsCS_DAL.app_Repository;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{

    public class Question_Service : IQuestion_Service
    {
        private readonly IWidget_Service _widgetService;
        private readonly IWidgetVersion_Service _widgetVersionService;
        private readonly IQuestion_Repository _questionRepository;
        private readonly IAnswer_Service _answerService;

        public Question_Service(
            IWidget_Service widgetService,
            IWidgetVersion_Service widgetVersionService,
            IQuestion_Repository questionRepository,
            IAnswer_Service answerService)
        {
            _widgetService = widgetService;
            _widgetVersionService = widgetVersionService;
            _questionRepository = questionRepository;
            _answerService = answerService;
        }

        public async Task<IReadOnlyList<Question>> GetPublishedQuestionsAsync(
            string publicKey)
        {
            if (string.IsNullOrWhiteSpace(publicKey))
                return Array.Empty<Question>();

            var widget =
                await _widgetService.GetPublishedWidgetAsync(publicKey);

            if (widget == null ||
                string.IsNullOrWhiteSpace(widget.WidgetTokenId))
            {
                return Array.Empty<Question>();
            }

            var version =
                await _widgetVersionService.GetPublishedVersionAsync(publicKey);

            if (version == null ||
                string.IsNullOrWhiteSpace(version.VersionTokenId))
            {
                return Array.Empty<Question>();
            }

            var questions =
                await _questionRepository.GetActiveByVersionAsync(
                    widget.WidgetPublicKey,
                    widget.WidgetTokenId,
                    version.VersionTokenId);

            foreach (var question in questions)
            {
                question.Answers =
                    (await _answerService.GetActiveByQuestionAsync(
                        widget.WidgetPublicKey,
                        question.QuestionTokenId,
                        version.VersionTokenId))
                    .ToList();
            }

            return questions;
        }
    }
}

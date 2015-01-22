using System.Collections.Generic;
using Domain.Entities;
using HtmlAgilityPack;

namespace LFedorov.Moodle.QuestionParsers
{
    public abstract class QuestionParser : IQuestionParser
    {
        protected readonly List<Question> _questions;

        protected QuestionParser()
        {
            _questions = new List<Question>();
        }

        public List<Question> ParseQuestions(IEnumerable<HtmlNode> questionNodes)
        {
            foreach (var questionNode in questionNodes)
            {
                var question = GetQuestionFromNode(questionNode);
                _questions.Add(question);
            }

            return _questions;
        }

        public abstract Question GetQuestionFromNode(HtmlNode questionNode);
    }
}
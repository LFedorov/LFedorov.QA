using System.Collections.Generic;
using Domain.Entities;
using HtmlAgilityPack;

namespace LFedorov.Moodle.QuestionParsers
{
    public interface IQuestionParser
    {
        List<Question> ParseQuestions(IEnumerable<HtmlNode> questionNodes);
    }
}
using System.Collections.Generic;
using Domain.Entities;
using HtmlAgilityPack;

namespace LFedorov.Moodle.QuestionParsers
{
    public class BooleanQuestionParser : QuestionParser
    {
        public override Question GetQuestionFromNode(HtmlNode questionNode)
        {
            var questionText = "";
            var questionAnswers = new Dictionary<Answer, bool>();

            var questionContentNode = questionNode.SelectSingleNode("./div[@class='content']");
            if (questionContentNode != null)
            {
                var questionTextNode = questionContentNode.SelectSingleNode("./div[@class='qtext']");
                if (questionTextNode != null)
                {
                    questionText = GetQuestionText(questionTextNode);
                }

                var answersNode = questionContentNode.SelectSingleNode("./div[@class='ablock clearfix']/div[@class='answer']");
                if (answersNode != null)
                {
                    questionAnswers = GetQuestionAnswers(answersNode);
                }
            }

            var question = new Question(questionText);

            foreach (var questionAnswer in questionAnswers)
            {
                question.AddAnswer(questionAnswer.Key, questionAnswer.Value);
            }

            return question;
        }

        private string GetQuestionText(HtmlNode questionTextNode)
        {
            var questionText = questionTextNode.InnerHtml.Trim();
            return questionText;
        }

        private Dictionary<Answer, bool> GetQuestionAnswers(HtmlNode answersNode)
        {
            var answers = new Dictionary<Answer, bool>();

            var answerNodes = answersNode.SelectNodes(".//span");
            if (answerNodes != null)
            {
                foreach (var answerNode in answerNodes)
                {
                    var answerTextNode = answerNode.SelectSingleNode("./label");
                    var answerText = answerTextNode != null ? answerTextNode.InnerText.Trim() : "";

                    var answerImageNode = answerNode.SelectSingleNode("./img[not(@class)]");
                    var answerImage = answerImageNode != null ? answerImageNode.Attributes["src"].Value : "";

                    var answerCorrectnessNode = answerNode.SelectSingleNode("./img[@class='icon']");

                    if (answerCorrectnessNode != null)
                    {
                        var answerIsCorrect = answerCorrectnessNode.Attributes["alt"].Value == "Верно";

                        var answer = new Answer(answerText);
                        answers.Add(answer, answerIsCorrect);
                    }
                }
            }

            return answers;
        }
    }
}
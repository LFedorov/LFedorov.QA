using System;
using Domain.Entities;
using HtmlAgilityPack;

namespace LFedorov.Moodle.QuestionParsers
{
    public class NumericalQuestionParser : QuestionParser
    {
        public override Question GetQuestionFromNode(HtmlNode questionNode)
        {
            var questionText = "";
            Tuple<Answer, bool> questionAnswer = null;

            var questionContentNode = questionNode.SelectSingleNode("./div[@class='content']");
            if (questionContentNode != null)
            {
                var questionTextNode = questionContentNode.SelectSingleNode("./div[@class='qtext']");
                if (questionTextNode != null)
                {
                    questionText = GetQuestionText(questionTextNode);

                    GetQuestionImage(questionTextNode);
                }

                var answersNode = questionContentNode.SelectSingleNode("./div[@class='ablock clearfix']/div[@class='answer']");
                if (answersNode != null)
                {
                    questionAnswer = GetQuestionAnswer(answersNode);
                }
            }

            var question = new Question(questionText);
            if (questionAnswer != null && questionAnswer.Item1 != null)
            {
                question.AddAnswer(questionAnswer.Item1, questionAnswer.Item2);
            }

            return question;
        }

        private string GetQuestionText(HtmlNode questionTextNode)
        {
            if (questionTextNode.SelectNodes(".//a/img") != null)
            {
                foreach (var imgNode in questionTextNode.SelectNodes(".//a/img"))
                {
                    if (imgNode.Attributes["src"] == null) continue;

                    var newNode = HtmlNode.CreateNode("<img />");
                    //newNode.SetAttributeValue("src", ConvertImageURLToBase64(imgNode.Attributes["src"].Value));
                    newNode.SetAttributeValue("src", imgNode.Attributes["src"].Value);
                    imgNode.ParentNode.ReplaceChild(newNode, imgNode);
                }
            }

            return questionTextNode.InnerHtml.Trim();
        }

        private string GetQuestionImage(HtmlNode questionTextNode)
        {
            var questionImageNode = questionTextNode != null ? questionTextNode.SelectSingleNode("./a/img") : null;
            var questionImage = questionImageNode != null ? questionImageNode.Attributes["src"].Value : "";
            return questionImage;
        }

        private Tuple<Answer, bool> GetQuestionAnswer(HtmlNode answerBlockNode)
        {
            var answerNode = answerBlockNode.SelectSingleNode("./input");
            if (answerNode == null || answerNode.Attributes["class"] == null)
                return new Tuple<Answer, bool>(null, false);

            var isCorrect = answerNode.Attributes["class"].Value == "correct";
            var answerText = answerNode.Attributes["value"].Value;

            return new Tuple<Answer, bool>(new Answer(answerText), isCorrect);
        }
    }
}
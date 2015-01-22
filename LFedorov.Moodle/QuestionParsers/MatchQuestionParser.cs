using System;
using Domain.Entities;
using HtmlAgilityPack;

namespace LFedorov.Moodle.QuestionParsers
{
    public class MatchQuestionParser : QuestionParser
    {
        public override Question GetQuestionFromNode(HtmlNode questionNode)
        {
            var questionText = "";
            Tuple<Answer, bool> questionAnswer = null;

            //Получаем блок содержимого вопроса
            var questionContentNode = questionNode.SelectSingleNode("./div[@class='content']");
            if (questionContentNode != null)
            {
                //Получаем блок текста вопроса
                var questionTextNode = questionContentNode.SelectSingleNode("./div[@class='qtext']");
                if (questionTextNode != null)
                {
                    //Получаем текст вопроса
                    questionText = GetQuestionText(questionTextNode);
                }

                //Получаем блок ответов вопроса
                var answersNode = questionContentNode.SelectSingleNode("./div[@class='ablock clearfix']/table[@class='answer']");
                //Получаем блок оценки ответов вопроса
                var gradingNode = questionContentNode.SelectSingleNode("./div[@class='grading']");
                if (answersNode != null && gradingNode != null)
                {
                    questionAnswer = GetQuestionAnswers(answersNode, gradingNode);
                }
            }

            //var question = new Question(questionText, questionImage);
            var question = new Question(questionText);

            if (questionAnswer != null && questionAnswer.Item1 != null)
            {
                question.AddAnswer(questionAnswer.Item1, questionAnswer.Item2);
            }

            return question;
        }

        protected string GetQuestionText(HtmlNode questionTextNode)
        {
            var imgNodes = questionTextNode.SelectNodes(".//a/img");
            if (imgNodes != null)
            {
                foreach (var imgNode in imgNodes)
                {
                    var newNode = imgNode.Clone();
                    imgNode.ParentNode.ParentNode.ReplaceChild(newNode, imgNode.ParentNode);
                }
            }

            var tableNodes = questionTextNode.SelectNodes(".//table");
            if (tableNodes != null)
            {
                foreach (var tableNode in tableNodes)
                {
                    tableNode.SetAttributeValue("class", "table");
                }
            }

            var strongNodes = questionTextNode.SelectNodes(".//strong");
            if (strongNodes != null)
            {
                foreach (var strongNode in strongNodes)
                {
                    var newNode = HtmlNode.CreateNode(strongNode.InnerText);
                    strongNode.ParentNode.ReplaceChild(newNode, strongNode);
                }
            }

            var pNodes = questionTextNode.SelectNodes(".//p");
            if (pNodes != null)
            {
                foreach (var pNode in pNodes)
                {
                    var newNode = HtmlNode.CreateNode(pNode.InnerText);
                    pNode.ParentNode.ReplaceChild(newNode, pNode);
                }
            }

            var h3Nodes = questionTextNode.SelectNodes(".//h3");
            if (h3Nodes != null)
            {
                foreach (var h3Node in h3Nodes)
                {
                    var newNode = HtmlNode.CreateNode(h3Node.InnerText);
                    h3Node.ParentNode.ReplaceChild(newNode, h3Node);
                }
            }

            return questionTextNode.InnerHtml
                .Trim()
                .Replace("–", "-")
                .Replace("—", "-");
        }

        private Tuple<Answer, bool> GetQuestionAnswers(HtmlNode answersNode, HtmlNode gradingNode)
        {
            var answerNodes = answersNode.SelectNodes(".//tr");
            if (answerNodes == null)
            {
                return null;
            }

            var finalText = "<ul>";

            var correctnessNode = gradingNode != null ? gradingNode.SelectSingleNode("./div[@class='correctness  correct']") : null;

            var partiallyCorrectNode = gradingNode != null ? gradingNode.SelectSingleNode("./div[@class='correctness  partiallycorrect']") : null;
            if (partiallyCorrectNode != null)
            {
                var score = gradingNode.SelectSingleNode("./div[@class='gradingdetails']").InnerText;
                finalText += " (" + score + ")";
            }

            foreach (var answerNode in answerNodes)
            {
                var answerTextNode = answerNode.SelectSingleNode("./td[@class='c0 text']");
                var answerText = answerTextNode != null ? answerTextNode.InnerText.Trim() : "";


                if (answerTextNode != null && answerTextNode.SelectSingleNode("./img") != null)
                {
                    answerText += "<img src=\"" + answerTextNode.SelectSingleNode("./img").Attributes["src"].Value + "\" />";
                }

                var answerControlNode = answerNode.SelectSingleNode("./td[@class='c1 control  ']");
                var answerSelectedNode = answerControlNode != null ? answerControlNode.SelectSingleNode("./select/option[@selected='selected']") : null;
                var answerSelected = answerSelectedNode != null ? answerSelectedNode.NextSibling.InnerText.Trim() : "Не выбран";

                finalText += "<li>" + answerText + " <b>[" + answerSelected + "]</b> </li>";
            }

            finalText += "</ul>";

            var answer = new Answer(finalText);
            return new Tuple<Answer, bool>(answer, correctnessNode != null);
        }
    }
}
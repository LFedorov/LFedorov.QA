using System.Collections.Generic;
using Domain.Entities;
using HtmlAgilityPack;

namespace LFedorov.Moodle.QuestionParsers
{
    public class MultichoiceQuestionParser : QuestionParser
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

                var answersNode = questionContentNode.SelectSingleNode("./div[@class='ablock clearfix']/table[@class='answer']");
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
            if (questionTextNode.SelectNodes(".//img") != null)
            {
                foreach (var imgNode in questionTextNode.SelectNodes(".//img"))
                {
                    if (imgNode.Attributes["src"] == null) continue;

                    var newNode = HtmlNode.CreateNode("<img />");
                    //newNode.SetAttributeValue("src", ConvertImageURLToBase64(imgNode.Attributes["src"].Value));
                    newNode.SetAttributeValue("src", imgNode.Attributes["src"].Value);
                    imgNode.ParentNode.ReplaceChild(newNode, imgNode);
                }
            }

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
                    var newNode = HtmlNode.CreateNode(pNode.InnerText + " ");
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

            return questionTextNode.InnerHtml.Trim();
        }

        private Dictionary<Answer, bool> GetQuestionAnswers(HtmlNode answersNode)
        {
            var answers = new Dictionary<Answer, bool>();

            var answerNodes = answersNode.SelectNodes(".//tr");
            if (answerNodes != null)
            {
                foreach (var answerNode in answerNodes)
                {
                    var answerTextNode = answerNode.SelectSingleNode("./td[@class='c1 text ']");

                    var answerLettersNode = answerTextNode != null ? answerTextNode.SelectSingleNode("./label/span[@class='anun']") : null;
                    if (answerLettersNode != null)
                    {
                        answerLettersNode.Remove();
                    }

                    var answerCorrectnessNode = answerTextNode != null ? answerTextNode.SelectSingleNode("./label/img[@class='icon']") : null;
                    if (answerCorrectnessNode != null)
                    {
                        var answerIsCorrect = answerCorrectnessNode.Attributes["alt"].Value == "Верно";
                        answerCorrectnessNode.Remove();

                        if (answerTextNode.SelectNodes(".//label") != null)
                        {
                            var imgNodes = answerTextNode.SelectNodes(".//a/img");
                            if (imgNodes != null)
                            {
                                foreach (var imgNode in imgNodes)
                                {
                                    if (imgNode.Attributes["src"] == null) continue;

                                    var newNode = HtmlNode.CreateNode("<img />");
                                    newNode.SetAttributeValue("src", imgNode.Attributes["src"].Value);
                                    imgNode.ParentNode.ReplaceChild(newNode, imgNode);
                                }
                            }

                            var oldNodes = answerTextNode.SelectNodes(".//label");
                            foreach (var oldNode in oldNodes)
                            {
                                var newNode = HtmlNode.CreateNode(oldNode.InnerHtml.Trim());
                                oldNode.ParentNode.ReplaceChild(newNode, oldNode);
                            }
                        }

                        var answerText = answerTextNode.InnerHtml.Trim();
                        var answer = new Answer(answerText);
                        answers.Add(answer, answerIsCorrect);
                    }
                }
            }

            return answers;
        }
    }
}
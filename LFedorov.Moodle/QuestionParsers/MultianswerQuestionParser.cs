using System;
using System.Linq;
using Domain.Entities;
using HtmlAgilityPack;

namespace LFedorov.Moodle.QuestionParsers
{
    public class MultianswerQuestionParser : QuestionParser
    {
        public override Question GetQuestionFromNode(HtmlNode questionNode)
        {
            var questionText = "";
            Tuple<Answer, bool> questionAnswer = null;
            //var questionAnswers = new Dictionary<Answer, bool>();

            //Получаем блок содержимого вопроса
            var questionContentNode = questionNode.SelectSingleNode("./div[@class='content']");
            if (questionContentNode != null)
            {
                //Получаем текст вопроса
                questionText = GetQuestionText(questionContentNode);

                //Получаем ответы на вопрос
                questionAnswer = GetQuestionAnswers(questionContentNode);
            }

            //var question = new Question(questionText), questionImage);
            var question = new Question(questionText);
            if (questionAnswer != null && questionAnswer.Item1 != null)
            {
                question.AddAnswer(questionAnswer.Item1, questionAnswer.Item2);
            }

            return question;
        }

        private string GetQuestionText(HtmlNode questionContentNode)
        {
            var answersBlockNode = questionContentNode.SelectSingleNode("./div[@class='ablock clearfix']");
            if (answersBlockNode != null)
            {
                if (answersBlockNode.SelectSingleNode(".//label/font/strong") != null)
                {
                    var questionTextNode = answersBlockNode.SelectSingleNode(".//label/font/strong");
                    var questionText = questionTextNode.InnerHtml.Trim();
                    questionTextNode.Remove();
                    return questionText;
                }

                if (answersBlockNode.SelectSingleNode(".//label/strong/font") != null)
                {
                    var questionTextNode = answersBlockNode.SelectSingleNode(".//label/strong/font");
                    var questionText = questionTextNode.InnerHtml.Trim();
                    questionTextNode.Remove();
                    return questionText;
                }

                if (answersBlockNode.SelectSingleNode(".//label/p/i/strong") != null)
                {
                    var questionTextNode = answersBlockNode.SelectSingleNode(".//label/p/i/strong");
                    var questionText = questionTextNode.InnerHtml.Trim();
                    questionTextNode.Remove();
                    return questionText;
                }

                var questionTextNode2 = answersBlockNode.SelectSingleNode("./label[1]/p[1]/b[1]");
                if (questionTextNode2 != null)
                {
                    var questionText = questionTextNode2.InnerText.Trim();
                    questionTextNode2.ParentNode.RemoveChild(questionTextNode2);
                    return questionText;
                }

                var questionTextNode3 = answersBlockNode.SelectSingleNode("./label[1]/span[1]");
                if (questionTextNode3 != null)
                {
                    var questionText = questionTextNode3.InnerText.Trim();
                    questionTextNode3.ParentNode.RemoveChild(questionTextNode3);
                    return questionText;
                }

                var questionTextWithInputNodes = answersBlockNode.SelectNodes(".//input");
                if (questionTextWithInputNodes.Any())
                {
                    var answerBlockNodeCopy = answersBlockNode.Clone();
                    foreach (var node in answerBlockNodeCopy.SelectNodes(".//input"))
                    {
                        node.ParentNode.ReplaceChild(HtmlNode.CreateNode("<b>[...]</b>"), node);
                        //answerBlockNodeCopy.ReplaceChild(HtmlNode.CreateNode("<b>[...]</b>"), node);
                    }

                    foreach (var questionTextWithInputNode in questionTextWithInputNodes)
                    {
                        bool isCorrect = false;

                        if (questionTextWithInputNode.Attributes["class"] != null)
                            isCorrect = questionTextWithInputNode.Attributes["class"].Value == "correct";

                        var value = questionTextWithInputNode.Attributes["value"].Value;

                        if (string.IsNullOrEmpty(value))
                            value = "[...]";

                        var newText = isCorrect
                            ? HtmlNode.CreateNode("<b>" + value + "</b>")
                            : HtmlNode.CreateNode("<del>" + value + "</del>");

                        questionTextWithInputNode.ParentNode.ReplaceChild(newText, questionTextWithInputNode);

                        //answersBlockNode.ReplaceChild(
                        //    isCorrect
                        //        ? HtmlNode.CreateNode("<b>" + value + "</b>")
                        //        : HtmlNode.CreateNode("<del>" + value + "</del>"), questionTextWithInputNode);
                    }

                    return answerBlockNodeCopy.InnerText.Trim();
                }
            }

            return "Undefined";
        }

        private Tuple<Answer, bool> GetQuestionAnswers(HtmlNode questionContentNode)
        {
            var answersBlockNode = questionContentNode.SelectSingleNode("./div[@class='ablock clearfix']");
            if (answersBlockNode != null)
            {
                var correctnessNodes = answersBlockNode.SelectNodes(".//span/select");
                if (correctnessNodes != null)
                {
                    foreach (var correctnessNode in correctnessNodes)
                    {
                        var selectedValueNode = correctnessNode.SelectSingleNode("./option[@selected='selected']");
                        var selectedValue = selectedValueNode != null ? selectedValueNode.NextSibling.InnerText.Trim() : "......";

                        if (correctnessNode.Attributes["class"] != null && correctnessNode.Attributes["class"].Value == "correct")
                            correctnessNode.ParentNode.ParentNode.ReplaceChild(HtmlNode.CreateNode("<b>[" + selectedValue + "]</b>"), correctnessNode.ParentNode);
                        else if (correctnessNode.Attributes["class"] != null && correctnessNode.Attributes["class"].Value == "incorrect")
                            correctnessNode.ParentNode.ParentNode.ReplaceChild(HtmlNode.CreateNode("<del>[" + selectedValue + "]</del>"), correctnessNode.ParentNode);
                        else
                            correctnessNode.ParentNode.ParentNode.ReplaceChild(HtmlNode.CreateNode("<del>[" + selectedValue + "]</del>"), correctnessNode.ParentNode);
                    }
                }

                var imageNodes = answersBlockNode.SelectNodes(".//img");
                if (imageNodes != null)
                {
                    foreach (var imageNode in imageNodes)
                    {
                        imageNode.Remove();
                    }
                }

                var centerNodes = answersBlockNode.SelectNodes(".//center");
                if (centerNodes != null)
                {
                    foreach (var centerNode in centerNodes)
                    {
                        centerNode.Remove();
                    }
                }

                var divNodes = answersBlockNode.SelectNodes(".//div");
                if (divNodes != null)
                {
                    foreach (var divNode in divNodes)
                    {
                        divNode.Remove();
                    }
                }

                var answerText = answersBlockNode.InnerHtml
                    .Replace("<i>", "")
                    .Replace("</i>", "")
                    .Replace("<em>", "")
                    .Replace("</em>", "")
                    .Replace("<p>", "")
                    .Replace("</p>", "")
                    .Replace("<label>", "")
                    .Replace("</label>", "");

                var isCorrect = false;
                var gradingNode = questionContentNode.SelectSingleNode("./div[@class='grading']");
                if (gradingNode != null)
                {
                    var correctnessNode = gradingNode.SelectSingleNode("./div[@class='correctness  correct']");
                    isCorrect = correctnessNode != null;

                    var partiallyCorrectNode = gradingNode.SelectSingleNode("./div[@class='correctness  partiallycorrect']");
                    if (partiallyCorrectNode != null)
                    {
                        var score = gradingNode.SelectSingleNode("./div[@class='gradingdetails']").InnerText;
                        answerText += " (" + score + ")";
                    }

                    var incorrectCorrectNode = gradingNode.SelectSingleNode("./div[@class='correctness  incorrect']");
                    if (incorrectCorrectNode != null)
                    {
                        var score = gradingNode.SelectSingleNode("./div[@class='gradingdetails']").InnerText;
                        answerText += " (" + score + ")";
                    }
                }

                return new Tuple<Answer, bool>(new Answer(answerText), isCorrect);
            }

            return new Tuple<Answer, bool>(null, false);
        }
    }
}
using Domain.Entities;
using HtmlAgilityPack;

namespace LFedorov.Moodle.QuestionParsers
{
    public class ShortanswerQuestionParser : QuestionParser
    {
        public override Question GetQuestionFromNode(HtmlNode questionNode)
        {
            var questionText = "";
            Answer answer = null;
            var isCorrect = false;

            var questionContentNode = questionNode.SelectSingleNode("./div[@class='content']");
            if (questionContentNode != null)
            {
                var questionTextNode = questionContentNode.SelectSingleNode("./div[@class='qtext']");
                if (questionTextNode != null)
                {
                    questionText = GetQuestionText(questionTextNode);
                }

                var answerNode = questionContentNode.SelectSingleNode("./div[@class='ablock clearfix']/div[@class='answer']");

                var gradingNode = questionContentNode.SelectSingleNode("./div[@class='grading']");
                if (answerNode != null && gradingNode != null)
                {
                    var answerTextNode = answerNode.SelectSingleNode("./input");
                    if (answerTextNode != null)
                    {
                        answer = new Answer(answerTextNode.Attributes["value"].Value.Trim());

                        if (answerTextNode.Attributes["class"].Value == "correct")
                        {
                            isCorrect = true;
                        }
                    }
                }
            }

            //var question = new Question(questionText, questionImage);
            var question = new Question(questionText);

            if (answer != null)
            {
                question.AddAnswer(answer, isCorrect);
            }

            return question;
        }

        private string GetQuestionText(HtmlNode questionTextNode)
        {
            var questionText = questionTextNode.InnerText.Trim();
            return questionText;
        }
    }
}
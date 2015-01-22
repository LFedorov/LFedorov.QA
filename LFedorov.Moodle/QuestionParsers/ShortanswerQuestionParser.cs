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

                //Получаем блок ответа
                var answerNode = questionContentNode.SelectSingleNode("./div[@class='ablock clearfix']/div[@class='answer']");
                //Получаем блок оценки ответов вопроса
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

            //Возвращаем полученный вопрос
            return question;
        }

        private string GetQuestionText(HtmlNode questionTextNode)
        {
            //Получаем текст вопроса
            var questionText = questionTextNode.InnerText.Trim();
            return questionText;
        }
    }
}
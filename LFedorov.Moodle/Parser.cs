using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Entities;
using HtmlAgilityPack;
using LFedorov.Moodle.QuestionParsers;

namespace LFedorov.Moodle
{
    public class Parser
    {
        public List<Question> GetContent(Stream stream, string contentType)
        {
            switch (contentType)
            {
                case "text/html":
                    return ParseContent(GetContentFromHtml(stream));
                case "multipart/related":
                case "application/mime":
                    return ParseContent(GetContentFromMht(stream));
                default:
                    return null;
            }
        }

        private static string GetContentFromHtml(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        private static string GetContentFromMht(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();

                return new Decoder().Decode(bytes);
            }
        }

        private static List<Question> ParseContent(string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(GetCorrectContent(content));

            RemoveUnused(document.DocumentNode);

            var questions = new List<Question>();

            var multichoiceQuestionNodes = document.DocumentNode.SelectNodes(@"//div[@class='que multichoice clearfix']");
            if (multichoiceQuestionNodes != null)
            {
                var multichoiseQuestions = new MultichoiceQuestionParser().ParseQuestions(multichoiceQuestionNodes);
                multichoiseQuestions.ForEach(questions.Add);
            }

            var matchQuestionNodes = document.DocumentNode.SelectNodes(@"//div[@class='que match clearfix']");
            if (matchQuestionNodes != null)
            {
                var matchQuestions = new MatchQuestionParser().ParseQuestions(matchQuestionNodes);
                matchQuestions.ForEach(questions.Add);
            }

            var multianswerQuestionNodes = document.DocumentNode.SelectNodes(@"//div[@class='que multianswer clearfix']");
            if (multianswerQuestionNodes != null)
            {
                var multianswerQuestions = new MultianswerQuestionParser().ParseQuestions(multianswerQuestionNodes);
                multianswerQuestions.ForEach(questions.Add);
            }

            var booleanQuestionNodes = document.DocumentNode.SelectNodes(@"//div[@class='que truefalse clearfix']");
            if (booleanQuestionNodes != null)
            {
                var booleanQuestions = new BooleanQuestionParser().ParseQuestions(booleanQuestionNodes);
                booleanQuestions.ForEach(questions.Add);
            }

            var shortanswerQuestionNodes = document.DocumentNode.SelectNodes(@"//div[@class='que shortanswer clearfix']");
            if (shortanswerQuestionNodes != null)
            {
                var shortansweQuestions = new ShortanswerQuestionParser().ParseQuestions(shortanswerQuestionNodes);
                shortansweQuestions.ForEach(questions.Add);
            }

            var numericalQuestionNodes = document.DocumentNode.SelectNodes(@"//div[@class='que numerical clearfix']");
            if (numericalQuestionNodes != null)
            {
                var numericalQuestions = new NumericalQuestionParser().ParseQuestions(numericalQuestionNodes);
                numericalQuestions.ForEach(questions.Add);
            }

            var disciplineNameNode = document.DocumentNode.SelectSingleNode(@"//title");
            var disciplineName = disciplineNameNode != null ? disciplineNameNode.InnerText.Split('(')[0].Trim() : "Undefined";
            var discipline = new Discipline(disciplineName);

            foreach (var question in questions)
            {
                question.SetDiscipline(discipline);
            }

            return questions;
        }

        protected static string GetCorrectContent(string content)
        {
            return content
                .Replace("<1С: Предприятие>", " \"1С: Предприятие\" ")
                .Replace("–", "-")
                .Replace("—", "-");
        }

        protected static void RemoveUnused(HtmlNode node)
        {
            var scriptNodes = node.SelectNodes(".//script");
            if (scriptNodes != null)
            {
                foreach (var scriptNode in scriptNodes)
                {
                    scriptNode.Remove();
                }
            }

            var styleNodes = node.SelectNodes(".//style");
            if (styleNodes != null)
            {
                foreach (var styleNode in styleNodes)
                {
                    styleNode.Remove();
                }
            }

            var brNodes = node.SelectNodes(".//br");
            if (brNodes != null)
            {
                foreach (var brNode in brNodes)
                {
                    brNode.Remove();
                }
            }

            foreach (var attribute in node.Attributes.ToList().Where(attribute =>
                (attribute.Name.ToLower() != "src") &&
                (attribute.Name.ToLower() != "rowspan") &&
                (attribute.Name.ToLower() != "colspan") &&
                (attribute.Name.ToLower() != "alt") &&
                (attribute.Name.ToLower() != "selected") &&
                (attribute.Name.ToLower() != "value") &&
                (attribute.Name.ToLower() != "class")))
            {
                attribute.Remove();
            }

            foreach (var childNode in node.ChildNodes.ToList())
            {
                if (childNode.InnerText.StartsWith("<!--"))
                    childNode.Remove();
                else
                    RemoveUnused(childNode);
            }
        }
    }
}
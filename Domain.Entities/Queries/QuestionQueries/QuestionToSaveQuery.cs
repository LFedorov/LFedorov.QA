using Domain.Common.Query;

namespace Domain.Entities.Queries.QuestionQueries
{
    public class QuestionToSaveQuery : IQuery<Question>
    {
        public QuestionToSaveQuery(Question question)
        {
            Question = question;
        }

        public Question Question { get; protected set; }
    }
}

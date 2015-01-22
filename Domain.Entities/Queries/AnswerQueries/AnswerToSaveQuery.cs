using Domain.Common.Query;

namespace Domain.Entities.Queries.AnswerQueries
{
    public class AnswerToSaveQuery : IQuery<Answer>
    {
        public AnswerToSaveQuery(Answer answer)
        {
            Answer = answer;
        }

        public Answer Answer { get; protected set; }
    }
}

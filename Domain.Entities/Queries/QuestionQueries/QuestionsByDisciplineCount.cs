using Domain.Common.Query;

namespace Domain.Entities.Queries.QuestionQueries
{
    public class QuestionsByDisciplineCount : IQuery<int>
    {
        public QuestionsByDisciplineCount(int disciplineId)
        {
            DisciplineId = disciplineId;
        }

        public int DisciplineId { get; protected set; }
    }
}
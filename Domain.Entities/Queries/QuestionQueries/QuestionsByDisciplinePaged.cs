using System.Collections.Generic;
using Domain.Common.Query;

namespace Domain.Entities.Queries.QuestionQueries
{
    public class QuestionsByDisciplinePaged : IQuery<IEnumerable<Question>>
    {
        public QuestionsByDisciplinePaged(int disciplineId, int currentPage, int perPage)
        {
            DisciplineId = disciplineId;
            CurrentPage = currentPage;
            PerPage = perPage;
        }

        public int DisciplineId { get; protected set; }
        public int CurrentPage { get; protected set; }
        public int PerPage { get; protected set; }
    }
}
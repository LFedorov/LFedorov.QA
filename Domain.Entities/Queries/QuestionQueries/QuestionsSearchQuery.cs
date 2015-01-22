using System.Collections.Generic;
using Domain.Common.Query;

namespace Domain.Entities.Queries.QuestionQueries
{
    public class QuestionsSearchQuery : IQuery<IEnumerable<Question>>
    {
        public QuestionsSearchQuery(string searchQuery)
        {
            SearchQuery = searchQuery;
        }

        public string SearchQuery { get; protected set; }
    }
}
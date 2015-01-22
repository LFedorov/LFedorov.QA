using Domain.Common.Query;

namespace Domain.Entities.Queries.DisciplineQueries
{
    public class DisciplineToSaveQuery : IQuery<Discipline>
    {
        public DisciplineToSaveQuery(Discipline discipline)
        {
            Discipline = discipline;
        }

        public Discipline Discipline { get; protected set; }
    }
}
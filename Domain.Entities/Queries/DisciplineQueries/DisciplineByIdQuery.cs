using Domain.Common.Query;

namespace Domain.Entities.Queries.DisciplineQueries
{
    public class DisciplineByIdQuery : IQuery<Discipline>
    {
        public DisciplineByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
using System.Collections.Generic;
using Domain.Common.Persistence;
using Domain.Common.Query;

namespace Domain.Entities.Queries.DisciplineQueries
{
    public class DisciplineQueryHandler :
        IQueryHandler<DisciplineToSaveQuery, Discipline>,
        IQueryHandler<DisciplinesCountQuery, int>,
        IQueryHandler<DisciplinesQuery, IEnumerable<Discipline>>,
        IQueryHandler<DisciplineByIdQuery, Discipline>
    {
        private readonly IRepository _repository;

        public DisciplineQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Discipline Execute(DisciplineToSaveQuery query)
        {
            var name = query.Discipline.Name;
            return _repository.Query<Discipline>().Where(x => x.Name.Trim().ToLower() == name.Trim().ToLower()).FirstOrDefault() ?? new Discipline(name);
        }

        public int Execute(DisciplinesCountQuery query)
        {
            return _repository.Query<Discipline>().Count();
        }

        public IEnumerable<Discipline> Execute(DisciplinesQuery query)
        {
            return _repository.Query<Discipline>().OrderBy(x=>x.Name).Include(x => x.Questions).ToList();
        }

        public Discipline Execute(DisciplineByIdQuery query)
        {
            return _repository.Query<Discipline>().Where(x => x.Id == query.Id).FirstOrDefault();
        }
    }
}
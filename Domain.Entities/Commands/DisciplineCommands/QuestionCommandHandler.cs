using System.Linq;
using Domain.Common.Command;
using Domain.Common.Persistence;
using Domain.Common.Query;
using Domain.Entities.Queries.DisciplineQueries;

namespace Domain.Entities.Commands.DisciplineCommands
{
    public class DisciplineCommandHandler :
        ICommandHandler<SaveDisciplinesCommand>
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IRepository _repository;
        private readonly IQueryDispatcher _queryDispatcher;

        public DisciplineCommandHandler(IUnitOfWorkFactory uowFactory, IRepository repository, IQueryDispatcher queryDispatcher)
        {
            _uowFactory = uowFactory;
            _repository = repository;
            _queryDispatcher = queryDispatcher;
        }

        public void Handle(SaveDisciplinesCommand command)
        {
            using (var uow = _uowFactory.Create())
            {
                foreach (var disciplineToSave in command.Disciplines.Select(discipline => _queryDispatcher.Ask(new DisciplineToSaveQuery(discipline))))
                {
                    _repository.Add(disciplineToSave);
                }

                uow.Commit();
            }
        }
    }
}

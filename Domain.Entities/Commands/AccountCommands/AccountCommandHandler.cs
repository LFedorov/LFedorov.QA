using Domain.Common.Command;
using Domain.Common.Persistence;
using Domain.Common.Query;

namespace Domain.Entities.Commands.AccountCommands
{
    public class AccountCommandHandler :
        ICommandHandler<CreateAccountCommand>
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IRepository _repository;
        private readonly IQueryDispatcher _queryDispatcher;

        public AccountCommandHandler(IUnitOfWorkFactory uowFactory, IRepository repository, IQueryDispatcher queryDispatcher)
        {
            _uowFactory = uowFactory;
            _repository = repository;
            _queryDispatcher = queryDispatcher;
        }

        public void Handle(CreateAccountCommand command)
        {
            using (var uow = _uowFactory.Create())
            {
                var account = new Account(command.Email, command.Password);
                _repository.Add(account);
                uow.Commit();
            }
        }
    }
}

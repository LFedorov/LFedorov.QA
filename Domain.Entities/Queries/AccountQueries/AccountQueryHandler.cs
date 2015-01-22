using Domain.Common.Persistence;
using Domain.Common.Query;

namespace Domain.Entities.Queries.AccountQueries
{
    public class AccountQueryHandler :
        IQueryHandler<AccountVerificationQuery, Account>
    {
        private readonly IRepository _repository;

        public AccountQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Account Execute(AccountVerificationQuery query)
        {
            var user = _repository.Query<Account>()
                .Where(x => x.Email == query.Email)
                .FirstOrDefault();

            if (user != null)
            {
                return user.Password.IsVerified(query.Password) ? user : null;
            }
            
            return null;
        }
    }
}
using Domain.Common.Query;

namespace Domain.Entities.Queries.AccountQueries
{
    public class AccountVerificationQuery : IQuery<Account>
    {
        public AccountVerificationQuery(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; protected set; }
        public string Password { get; protected set; }
    }
}
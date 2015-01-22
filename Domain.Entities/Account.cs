using Domain.Common.Entity;

namespace Domain.Entities
{
    public class Account : Entity
    {
        protected Account()
        {
        }

        public Account(string email, string password) : this()
        {
            Email = email;
            Password = new Password(password);
        }

        public virtual string Email { get; protected set; }
        public virtual Password Password { get; protected set; }
    }
}
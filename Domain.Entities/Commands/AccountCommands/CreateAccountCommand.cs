using Domain.Common.Command;

namespace Domain.Entities.Commands.AccountCommands
{
    public class CreateAccountCommand : ICommand
    {
        public CreateAccountCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; protected set; }
        public string Password { get; protected set; }
    }
}

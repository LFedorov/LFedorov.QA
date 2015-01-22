using Domain.Common.IoC;

namespace Domain.Common.Command
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IResolver _resolver;

        public CommandDispatcher(IResolver resolver)
        {
            _resolver = resolver;
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = (ICommandHandler<TCommand>) _resolver.Resolve(typeof(ICommandHandler<TCommand>));
            handler.Handle(command);
        }
    }
}
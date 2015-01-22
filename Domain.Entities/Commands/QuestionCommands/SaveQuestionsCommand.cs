using System.Collections.Generic;
using Domain.Common.Command;

namespace Domain.Entities.Commands.QuestionCommands
{
    public class SaveQuestionsCommand : ICommand
    {
        public SaveQuestionsCommand(IEnumerable<Question> questions)
        {
            Questions = questions;
        }

        public IEnumerable<Question> Questions { get; protected set; }
    }
}

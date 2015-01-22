using System.Collections.Generic;
using Domain.Common.Command;

namespace Domain.Entities.Commands.DisciplineCommands
{
    public class SaveDisciplinesCommand : ICommand
    {
        public SaveDisciplinesCommand(IEnumerable<Discipline> disciplines)
        {
            Disciplines = disciplines;
        }

        public IEnumerable<Discipline> Disciplines { get; protected set; }
    }
}

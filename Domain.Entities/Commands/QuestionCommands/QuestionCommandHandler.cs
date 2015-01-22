using System.Linq;
using Domain.Common.Command;
using Domain.Common.Persistence;
using Domain.Common.Query;
using Domain.Entities.Queries.AnswerQueries;
using Domain.Entities.Queries.DisciplineQueries;
using Domain.Entities.Queries.QuestionQueries;

namespace Domain.Entities.Commands.QuestionCommands
{
    public class QuestionCommandHandler :
        ICommandHandler<SaveQuestionsCommand>
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IRepository _repository;
        private readonly IQueryDispatcher _queryDispatcher;

        public QuestionCommandHandler(IUnitOfWorkFactory uowFactory, IRepository repository, IQueryDispatcher queryDispatcher)
        {
            _uowFactory = uowFactory;
            _repository = repository;
            _queryDispatcher = queryDispatcher;
        }

        public void Handle(SaveQuestionsCommand command)
        {
            using (var uow = _uowFactory.Create())
            {
                foreach (var question in command.Questions)
                {
                    var disciplineToSave = _queryDispatcher.Ask(new DisciplineToSaveQuery(question.Discipline));
                    var questionToSave = _queryDispatcher.Ask(new QuestionToSaveQuery(question));

                    foreach (var answerToSave in question.Correct.Select(answer => _queryDispatcher.Ask(new AnswerToSaveQuery(answer))))
                    {
                        questionToSave.AddAnswer(answerToSave, true);
                    }

                    foreach (var answerToSave in question.Wrong.Select(answer => _queryDispatcher.Ask(new AnswerToSaveQuery(answer))))
                    {
                        questionToSave.AddAnswer(answerToSave, false);
                    }

                    disciplineToSave.AddQuestion(questionToSave);
                    _repository.Add(disciplineToSave);
                }

                uow.Commit();
            }
        }
    }
}

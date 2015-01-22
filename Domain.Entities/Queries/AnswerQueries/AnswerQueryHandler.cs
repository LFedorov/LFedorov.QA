using Domain.Common.Persistence;
using Domain.Common.Query;

namespace Domain.Entities.Queries.AnswerQueries
{
    public class AnswerQueryHandler : 
        IQueryHandler<AnswerToSaveQuery, Answer>,
        IQueryHandler<AnswersCountQuery, int>
    {
        private readonly IRepository _repository;

        public AnswerQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Answer Execute(AnswerToSaveQuery query)
        {
            var aText = query.Answer.Text;

            return _repository.Query<Answer>().Where(x => x.Text.Trim().ToLower() == aText.Trim().ToLower()).FirstOrDefault() ?? new Answer(aText);
        }

        public int Execute(AnswersCountQuery query)
        {
            return _repository.Query<Answer>().Count();
        }
    }
}
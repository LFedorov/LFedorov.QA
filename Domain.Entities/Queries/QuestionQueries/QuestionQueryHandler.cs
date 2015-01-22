using System.Collections.Generic;
using Domain.Common.Persistence;
using Domain.Common.Query;

namespace Domain.Entities.Queries.QuestionQueries
{
    public class QuestionQueryHandler :
        IQueryHandler<QuestionToSaveQuery, Question>,
        IQueryHandler<QuestionsCountQuery, int>,
        IQueryHandler<QuestionsSearchQuery, IEnumerable<Question>>,
        IQueryHandler<QuestionsByDisciplineCount, int>,
        IQueryHandler<QuestionsByDisciplinePaged, IEnumerable<Question>>
    {
        private readonly IRepository _repository;

        public QuestionQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Question Execute(QuestionToSaveQuery query)
        {
            var qText = query.Question.Text.Trim();

            if (query.Question.Discipline == null) return new Question(qText);

            var dName = query.Question.Discipline.Name.Trim();

            return _repository.Query<Question>()
                .Where(x => x.Text.Trim().ToLower() == qText.Trim().ToLower() && x.Discipline.Name.ToLower() == dName.ToLower())
                .FirstOrDefault() ?? new Question(qText);
        }

        public int Execute(QuestionsCountQuery query)
        {
            return _repository.Query<Question>().Count();
        }

        public IEnumerable<Question> Execute(QuestionsSearchQuery query)
        {
            return _repository.Query<Question>()
                .Where(x => x.Text.Contains(query.SearchQuery))
                .Include(x => x.Discipline)
                .Include(x => x.Correct)
                .Include(x => x.Wrong)
                .ToList();
        }

        public int Execute(QuestionsByDisciplineCount query)
        {
            return _repository.Query<Question>()
                .Where(x => x.Discipline.Id == query.DisciplineId)
                .Count();
        }

        public IEnumerable<Question> Execute(QuestionsByDisciplinePaged query)
        {
            return _repository.Query<Question>()
                .Where(x => x.Discipline.Id == query.DisciplineId)
                .Include(x => x.Correct)
                .Include(x => x.Wrong)
                .Page(query.CurrentPage, query.PerPage)
                .ToList();
        }
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Domain.Common.Entity;

namespace Domain.Entities
{
    public class Discipline : Entity
    {
        private readonly IList<Question> _questions;

        protected Discipline()
        {
            _questions = new List<Question>();
        }

        public Discipline(string name)
            : this()
        {
            Name = name.Trim();
        }

        public virtual string Name { get; protected set; }
        public virtual IEnumerable<Question> Questions { get { return new ReadOnlyCollection<Question>(_questions); } }

        public virtual void AddQuestion(Question question)
        {
            if (question == null) return;
            if (_questions.Contains(question)) return;

            _questions.Add(question);
            question.SetDiscipline(this);
        }

        public virtual void RemoveQuestion(Question question)
        {
            if (question == null) return;
            if (!_questions.Contains(question)) return;

            _questions.Remove(question);
            question.SetDiscipline(null);
        }
    }
}
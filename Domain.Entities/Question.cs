using System.Collections.Generic;
using System.Collections.ObjectModel;
using Domain.Common.Entity;

namespace Domain.Entities
{
    public class Question : Entity
    {
        private readonly IList<Answer> _correct;
        private readonly IList<Answer> _wrong;

        protected Question()
        {
            _correct = new List<Answer>();
            _wrong = new List<Answer>();
        }

        public Question(string text)
            : this()
        {
            Text = text.Trim();
        }

        public virtual string Text { get; protected set; }
        public virtual Discipline Discipline { get; protected set; }
        public virtual IEnumerable<Answer> Correct { get { return new ReadOnlyCollection<Answer>(_correct); } }
        public virtual IEnumerable<Answer> Wrong { get { return new ReadOnlyCollection<Answer>(_wrong); } }

        public virtual void SetDiscipline(Discipline discipline)
        {
            if (Discipline == discipline) return;

            if (Discipline != null)
            {
                Discipline.RemoveQuestion(this);
            }

            Discipline = discipline;

            if (discipline != null)
            {
                discipline.AddQuestion(this);
            }
        }

        public virtual void AddAnswer(Answer answer, bool isCorrect)
        {
            if (answer == null) return;
            if (_correct.Contains(answer) || _wrong.Contains(answer)) return;

            if (isCorrect)
            {
                _correct.Add(answer);
                answer.AddQuestion(this, true);
            }
            else
            {
                _wrong.Add(answer);
                answer.AddQuestion(this, false);
            }
        }
    }
}
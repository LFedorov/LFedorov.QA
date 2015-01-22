using System.Collections.Generic;
using Domain.Common.Entity;

namespace Domain.Entities
{
    public class Answer : Entity
    {
        private readonly IList<Question> _correct;
        private readonly IList<Question> _wrong;

        protected Answer()
        {
            _correct = new List<Question>();
            _wrong = new List<Question>();
        }

        public Answer(string text)
            : this()
        {
            Text = text.Trim();
        }

        public virtual string Text { get; protected set; }
        public virtual IEnumerable<Question> Correct { get { return _correct; } }
        public virtual IEnumerable<Question> Wrong { get { return _wrong; } }

        public virtual void AddQuestion(Question question, bool isCorrect)
        {
            if (question == null) return;
            if (_correct.Contains(question) || _wrong.Contains(question)) return;

            if (isCorrect)
            {
                _correct.Add(question);
                question.AddAnswer(this, true);
            }
            else
            {
                _wrong.Add(question);
                question.AddAnswer(this, false);
            }
        }
    }
}
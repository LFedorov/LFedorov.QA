using Domain.Entities;
using FluentNHibernate.Mapping;

namespace Infrastructure.Persistence.Maps
{
    public class AnswerMap : ClassMap<Answer>
    {
        public AnswerMap()
        {
            Table("Answers");
            Id(x => x.Id);
            Map(x => x.Text).CustomSqlType("nvarchar(max)");

            HasManyToMany(x => x.Correct).Table("CorrectAnswers").Inverse();
            HasManyToMany(x => x.Wrong).Table("WrongAnswers").Inverse();
        }
    }
}
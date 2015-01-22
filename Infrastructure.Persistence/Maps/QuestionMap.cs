using Domain.Entities;
using FluentNHibernate.Mapping;
using Prefix = FluentNHibernate.Mapping.Prefix;

namespace Infrastructure.Persistence.Maps
{
    public class QuestionMap : ClassMap<Question>
    {
        public QuestionMap()
        {
            Table("Questions");
            Id(x => x.Id);
            Map(x => x.Text).CustomSqlType("nvarchar(max)");
            References(x => x.Discipline);

            HasManyToMany(x => x.Correct)
                .Table("CorrectAnswers")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.SaveUpdate();
            
            HasManyToMany(x => x.Wrong)
                .Table("WrongAnswers")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.SaveUpdate();
        }
    }
}
using Domain.Entities;
using FluentNHibernate.Mapping;

namespace Infrastructure.Persistence.Maps
{
    public class DisciplineMap : ClassMap<Discipline>
    {
        public DisciplineMap()
        {
            Table("Disciplines");
            Id(x => x.Id);
            Map(x => x.Name).CustomSqlType("nvarchar(max)");
            HasMany(x => x.Questions).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan();
        }
    }
}
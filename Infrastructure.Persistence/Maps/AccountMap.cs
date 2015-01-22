using Domain.Entities;
using FluentNHibernate.Mapping;

namespace Infrastructure.Persistence.Maps
{
    public class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Table("Users");
            Id(x => x.Id);
            Map(x => x.Email);
            Component(x => x.Password);
        }
    }
}
using Domain.Entities;
using FluentNHibernate.Mapping;

namespace Infrastructure.Persistence.Maps
{
    public class PasswordMap : ComponentMap<Password>
    {
        public PasswordMap()
        {
            Map(x => x.Hash);
            Map(x => x.Salt);
        }
    }
}
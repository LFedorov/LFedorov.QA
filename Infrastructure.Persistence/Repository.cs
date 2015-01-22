using System;
using Domain.Common.Entity;
using Domain.Common.Persistence;
using Domain.Common.Query;
using FluentNHibernate.Conventions;
using NHibernate;

namespace Infrastructure.Persistence
{
    public class Repository : IRepository
    {
        private readonly ISession _session;

        public Repository(ISession session)
        {
            _session = session;
            _session.FlushMode = FlushMode.Commit;
        }

        public IQueryBuilder<TEntity> Query<TEntity>() where TEntity : Entity
        {
            return new QueryBuilder<TEntity>(_session);
        }

        public void Add<TEntity>(TEntity entity) where TEntity : Entity
        {
            if (!_session.Transaction.IsActive) throw new Exception("Open Session Transaction!");

            _session.SaveOrUpdate(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : Entity
        {
            if (!_session.Transaction.IsActive) throw new Exception("Open Session Transaction!");

            _session.Delete(entity);
        }
    }
}
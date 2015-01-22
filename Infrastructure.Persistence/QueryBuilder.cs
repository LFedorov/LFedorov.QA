using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Common.Entity;
using Domain.Common.Query;
using NHibernate;
using NHibernate.Linq;

namespace Infrastructure.Persistence
{
    internal class QueryBuilder<TEntity> : IQueryBuilder<TEntity> where TEntity : Entity
    {
        private IQueryable<TEntity> _session;

        public QueryBuilder(ISession session)
        {
            _session = session.Query<TEntity>();
        }

        public IQueryBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            _session = _session.Where(predicate);
            return this;
        }

        public IQueryBuilder<TEntity> Include(Expression<Func<TEntity, object>> path)
        {
            _session.Fetch(path).ToFuture();
            //_session = _session.Fetch(path);
            return this;
        }

        public IQueryBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> path)
        {
            _session = _session.OrderBy(path);
            return this;
        }

        public IQueryBuilder<TEntity> OrderByDescending(Expression<Func<TEntity, object>> path)
        {
            _session = _session.OrderByDescending(path);
            return this;
        }

        public IQueryBuilder<TEntity> Page(int page, int pageSize)
        {
            _session = _session.Skip((page - 1) * pageSize).Take(pageSize);
            return this;
        }

        public TEntity FirstOrDefault()
        {
            return _session.FirstOrDefault();
        }

        public Task<TEntity> FirstOrDefaultAsync()
        {
            throw new NotImplementedException();
        }

        public List<TEntity> ToList()
        {
            return _session.ToList();
        }

        public Task<List<TEntity>> ToListAsync()
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            return _session.Count();
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }
    }
}
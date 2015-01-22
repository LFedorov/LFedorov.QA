using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Common.Query
{
    public interface IQueryBuilder<TEntity> where TEntity : Entity.Entity
    {
        IQueryBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IQueryBuilder<TEntity> Include(Expression<Func<TEntity, object>> path);
        IQueryBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> path);
        IQueryBuilder<TEntity> OrderByDescending(Expression<Func<TEntity, object>> path);
        IQueryBuilder<TEntity> Page(int page, int pageSize);

        TEntity FirstOrDefault();
        Task<TEntity> FirstOrDefaultAsync();

        List<TEntity> ToList();
        Task<List<TEntity>> ToListAsync();

        int Count();
        Task<int> CountAsync();
    }
}
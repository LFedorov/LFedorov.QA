using Domain.Common.Query;

namespace Domain.Common.Persistence
{
    public interface IRepository
    {
        IQueryBuilder<TEntity> Query<TEntity>() where TEntity : Entity.Entity;

        void Add<TEntity>(TEntity entity) where TEntity : Entity.Entity;
        void Delete<TEntity>(TEntity entity) where TEntity : Entity.Entity;
    }
}
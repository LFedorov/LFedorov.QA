namespace Domain.Common.Entity
{
    public abstract class Entity : IEntity
    {
        public virtual int Id { get; protected set; }
    }
}
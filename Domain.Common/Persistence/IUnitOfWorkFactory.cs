namespace Domain.Common.Persistence
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}
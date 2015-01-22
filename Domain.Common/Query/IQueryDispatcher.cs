namespace Domain.Common.Query
{
    public interface IQueryDispatcher
    {
        TResult Ask<TResult>(IQuery<TResult> query);
    }
}
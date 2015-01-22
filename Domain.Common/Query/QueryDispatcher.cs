using Domain.Common.IoC;

namespace Domain.Common.Query
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IResolver _resolver;

        public QueryDispatcher(IResolver resolver)
        {
            _resolver = resolver;
        }

        public TResult Ask<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            var handler = _resolver.Resolve(handlerType);

            return (TResult)((dynamic)handler).Execute((dynamic)query);
        }
    }
}
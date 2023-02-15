namespace Foo.Data.QueryHandlers;

public interface IQueryHandler<TQuery, TOut>
{
    Task<TOut> Execute(TQuery query);
}
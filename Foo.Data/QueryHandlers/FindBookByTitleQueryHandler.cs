using Microsoft.EntityFrameworkCore;

namespace Foo.Data.QueryHandlers;

public class FindBookByTitleQueryHandler : IQueryHandler<FindBookByTitleQuery, IEnumerable<Book>>
{
    private readonly BookContext _dbContext;

    public FindBookByTitleQueryHandler(BookContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Book>> Execute(FindBookByTitleQuery query)
    {
        return await _dbContext.Books
            .FromSqlRaw("EXECUTE dbo.SP_GetBooks {0}", query.Title)
            .ToArrayAsync();
    }
}
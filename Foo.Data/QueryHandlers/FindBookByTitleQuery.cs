using Microsoft.Data.SqlClient;

namespace Foo.Data.QueryHandlers;

public class FindBookByTitleQuery
{
    public FindBookByTitleQuery(string title)
    {
        Title = title;
    }
    
    public string Title { get; set; }
}
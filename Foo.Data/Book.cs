namespace Foo.Data;

public class Book
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    
    public Book()
    {
        
    }

    public Book(Guid id, string title)
    {
        Id = id;
        Title = title;
    }
}

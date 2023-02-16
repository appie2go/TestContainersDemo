using FluentAssertions;
using Foo.Data;
using Foo.Data.QueryHandlers;

namespace Foo.Tests;

public class StoredProcedureTest : IAsyncLifetime
{
    private readonly TestSqlContainer _testSqlContainer = new ();

    public async Task InitializeAsync() => await _testSqlContainer.StartAsync();

    public async Task DisposeAsync() => await _testSqlContainer.StopAsync();

    [Fact]
    public async Task StoredProcedureShouldWork()
    {
        // Arrange
        await _testSqlContainer.Execute(async (context) =>
        {
            await context.Books.AddRangeAsync(new Book(Guid.NewGuid(), "Domain Driven Design"),
                new Book(Guid.NewGuid(), "Value Proposition Design"),
                new Book(Guid.NewGuid(), "The Unicorn Project"));

            await context.SaveChangesAsync();
        });
        
        var sut = new FindBookByTitleQueryHandler(_testSqlContainer.GetDbContext());

        var query = new FindBookByTitleQuery("design");
        
        // Act
        var actual = await sut.Execute(query);
        
        // Assert
        actual.Should().HaveCount(2);
    }
}
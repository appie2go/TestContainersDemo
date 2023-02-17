# Unit testing a stored procedure with xUnit and test containers

This repo contains code to test a stored procedure that's being executed with Entity Framework. To test it, the code spins up a throwaway Docker container and provisiones a database with test data in it.

The code uses
* dotnet core
* EntityFrameworkCore
* A Microsoft SQL Server Stored Procedure
* xUnit
* TestContainers

Find more information about testcontainers [here](https://dotnet.testcontainers.org). Download the nuget testcontainers package [here](https://www.nuget.org/packages/Testcontainers).

Read about how testing a stored procedure with EntityFramework, xUnit, and Testcontainers works here: [https://medium.com/@abstarreveld/unit-testing-with-xunit-and-testcontainers-4a69cd22f888](https://medium.com/@abstarreveld/unit-testing-with-xunit-and-testcontainers-4a69cd22f888)

## Gimme the code

A long story short, this is what a unit test might look like:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Foo.Data;
using Foo.Data.QueryHandlers;
using Microsoft.EntityFrameworkCore;

namespace Foo.Tests;

public class StoredProcedureTest : IAsyncLifetime
{
    private readonly string _temporarySqlServerPassword = Guid.NewGuid().ToString();

    private IContainer _dockerContainer = null!;

    private BookContext _context = null!;
    
    public async Task InitializeAsync()
    {
        _dockerContainer = new ContainerBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPortBinding(1433, 1433)
            .WithEnvironment(new Dictionary<string, string>()
            {
                { "ACCEPT_EULA", "Y" },
                { "MSSQL_SA_PASSWORD", _temporarySqlServerPassword }
            })
            .Build();

        await _dockerContainer.StartAsync();

        await Task.Delay(10000); // Wait for server to have started...
        
        var connectionString =
            $"Server=127.0.0.1;Database=test;User Id=sa;Password={_temporarySqlServerPassword};TrustServerCertificate=True";
        
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseSqlServer(connectionString)
            .Options;

        _context = new BookContext(options);

        await _context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dockerContainer.StopAsync();
    }
    
    [Fact]
    public async Task StoredProcedureShouldWork()
    {
        // Arrange
        await _context.Books.AddRangeAsync(new Book(Guid.NewGuid(), "Domain Driven Design"),
            new Book(Guid.NewGuid(), "Value Proposition Design"),
            new Book(Guid.NewGuid(), "The Unicorn Project"));

        await _context.SaveChangesAsync();
        
        var sut = new FindBookByTitleQueryHandler(_context);
        var query = new FindBookByTitleQuery("design");
        
        // Act
        var actual = await sut.Execute(query);
        
        // Assert
        actual.Should().HaveCount(2);
    }
}
```

Happy testing!!

Don't forget to star this repo if it helped you!
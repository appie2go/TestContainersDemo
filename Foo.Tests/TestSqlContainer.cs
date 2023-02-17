using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Foo.Data;
using Microsoft.EntityFrameworkCore;

namespace Foo.Tests;

public class TestSqlContainer
{
    private readonly string _temporarySqlServerPassword = Guid.NewGuid().ToString();

    private IContainer _dockerContainer = null!;

    private BookContext _context = null!;

    public async Task StartAsync()
    {
        await ProvisionSqlServerContainer();

        await EnsureStarted();

        InstantiateDbContext();

        await _context.Database.MigrateAsync();
    }

    public async Task Execute(Func<BookContext, Task> func) => await func(_context);

    public async Task StopAsync()
    {
        await _dockerContainer?.StopAsync()!;

        await _context.DisposeAsync();
    }

    public BookContext GetDbContext() => _context;

    private async Task ProvisionSqlServerContainer()
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
    }

    private async Task EnsureStarted()
    {
        // Please make me a PR for this BS, because this is terrible!
        var logs = string.Empty;
        for (var i = 0; i < 50 && !logs.Contains("Server is listening on"); i++)
        {
            Console.WriteLine($"Waiting for server to start.. Attempt {i}");
            await Task.Delay(500);
            var (stdout, stderr) = await _dockerContainer.GetLogs();
            logs = stdout;
        }
    }

    private void InstantiateDbContext()
    {
        var connectionString =
            $"Server=127.0.0.1;Database=test;User Id=sa;Password={_temporarySqlServerPassword};TrustServerCertificate=True";
        
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseSqlServer(connectionString)
            .Options;

        _context = new BookContext(options);
    }
}
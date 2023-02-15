using Foo.Data;
using Foo.Data.QueryHandlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookContext>(options => options.UseSqlServer("Server=localhost;Database=demo;User Id=sa;Password=NunnajaBeezwax;TrustServerCertificate=True"));
builder.Services.AddTransient<IQueryHandler<FindBookByTitleQuery, IEnumerable<Book>>, FindBookByTitleQueryHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapGet("/books", async ([FromServices] IQueryHandler<FindBookByTitleQuery, IEnumerable<Book>> queryHandler, [FromQuery] string title) 
    => await queryHandler.Execute(new FindBookByTitleQuery(title)));

app.Run();

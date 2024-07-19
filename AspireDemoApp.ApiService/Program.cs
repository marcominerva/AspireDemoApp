using AspireDemoApp.ApiService.DataAccessLayer;
using Bogus;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

//builder.AddSqlServerDbContext<ApplicationDbContext>("sqldb");
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("sqldb");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.ExecutionStrategy(d => new ApplicationRetryingExecutionStrategy(d));
    });
});

builder.EnrichSqlServerDbContext<ApplicationDbContext>(options =>
{
    options.DisableRetry = true;
});

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/api/people", async (ApplicationDbContext dbContext, CancellationToken cancellationToken=default) =>
{
    var people = await dbContext.People.AsNoTracking()
        .OrderBy(p => p.FirstName).ThenBy(p => p.LastName)
        .Select(p=>new AspireDemoApp.Shared.Models.Person(p.Id, p.FirstName, p.LastName, p.City))
        .ToListAsync(cancellationToken);

    return TypedResults.Ok(people);
})
.WithOpenApi();

app.MapPost("/api/people", async (ApplicationDbContext dbContext, CancellationToken cancellationToken=default) =>
{
    var newPerson = new Faker<AspireDemoApp.ApiService.DataAccessLayer.Person>()
        .RuleFor(p => p.FirstName, f => f.Person.FirstName)
        .RuleFor(p => p.LastName, f => f.Person.LastName)
        .RuleFor(p => p.City, f => f.Address.City());

    var person = newPerson.Generate();

    dbContext.People.Add(person);
    await dbContext.SaveChangesAsync(cancellationToken);

    return TypedResults.NoContent();
})
.WithOpenApi();

app.MapDefaultEndpoints();

app.Run();

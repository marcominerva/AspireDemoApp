using AspireDemoApp.ApiService.DataAccessLayer;
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

app.MapGet("/api/status", async (ApplicationDbContext dbContext) =>
{
    try
    { 
        var people = await dbContext.People.CountAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    return TypedResults.NoContent();
});

app.MapDefaultEndpoints();

app.Run();

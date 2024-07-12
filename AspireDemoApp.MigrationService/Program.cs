using AspireDemoApp.ApiService.DataAccessLayer;
using AspireDemoApp.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<DatabaseInitializer>();

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(t=>t.AddSource(DatabaseInitializer.ActivitySourceName));

builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("sqldb");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
        sqlOptions.ExecutionStrategy(d => new ApplicationRetryingExecutionStrategy(d));
    });
});

builder.EnrichSqlServerDbContext<ApplicationDbContext>(options =>
{
    options.DisableRetry = true;
});

var app=builder.Build();

app.Run();
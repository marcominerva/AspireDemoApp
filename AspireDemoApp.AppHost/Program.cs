using AspireDemoApp.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlServer("sqlserver",
    password: builder.CreateStablePassword("sqlPassword1", minLower: 1, minUpper: 1, minNumeric: 1))
    .WithDataVolume("sqlserver")
    .AddDatabase("sqldb");

builder.AddProject<Projects.AspireDemoApp_MigrationService>("migration")
    .WithReference(db);

var apiService = builder.AddProject<Projects.AspireDemoApp_ApiService>("apiservice")
    .WithReference(db);

builder.AddProject<Projects.AspireDemoApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

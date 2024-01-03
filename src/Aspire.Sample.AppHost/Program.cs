var builder = DistributedApplication.CreateBuilder(args);

var moviesDbName = "Movies2";
var moviesDb = builder.AddPostgresContainer("postgres")
    // Set the name of the default database to auto-create on container startup.
    .WithEnvironment("POSTGRES_DB", moviesDbName)
    // Add the default database to the application model so that it can be referenced by other resources.
    .AddDatabase(moviesDbName);

var apiService = builder.AddProject<Projects.Movies_Api>("apiservice")
    .WithReference(moviesDb);

builder.AddProject<Projects.BlazorSsrDynamic>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
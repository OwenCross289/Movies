var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Movies_Api>("apiservice");

builder.AddProject<Projects.BlazorSsrDynamic>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
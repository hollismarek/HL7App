var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.HL7App_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.HL7App_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.HL7App_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

var sqlIte = builder.AddSqlite("hl7appdb")
    .WithSqliteWeb();

builder.AddProject<Projects.HL7App_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(sqlIte)
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

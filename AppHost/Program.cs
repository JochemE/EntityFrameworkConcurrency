var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject("WebApp", "../SeedGeneration/SeedGeneration.csproj")
    .WithReplicas(4);

builder.Build().Run();
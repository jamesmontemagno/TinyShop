var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithRedisCommander()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("myredisimage")
    .WithPersistence(TimeSpan.FromMinutes(5), 100);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var products = builder.AddProject<Projects.Products>("products")
    .WithReference(redis)
    .WaitFor(redis);

var ollama = builder.AddOllama("ollama")
    .WithDataVolume()
    .WithOpenWebUI();

var phi3 = ollama.AddModel("phi3", "phi3");

builder.AddProject<Projects.Store>("store")
    .WithReference(products)
    .WithExternalHttpEndpoints()
    .WaitFor(products)
    .WithReference(ollama)
    .WaitFor(ollama);

builder.Build().Run();

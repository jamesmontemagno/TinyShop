var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("myredisimage")
    .WithPersistence(TimeSpan.FromMinutes(5), 100);

var ollama = builder.AddOllama("ollama")
    .WithDataVolume();

var chat = ollama.AddModel("chat", "phi3");

var products = builder.AddProject<Projects.Products>("products")
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(chat)
    .WaitFor(chat);

builder.AddProject<Projects.Store>("store")
    .WithReference(products)
    .WithExternalHttpEndpoints()
    .WaitFor(products);

builder.Build().Run();

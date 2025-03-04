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

builder.AddProject<Projects.Store>("store")
    .WithReference(products)
    .WithExternalHttpEndpoints()
    .WaitFor(products);

builder.Build().Run();

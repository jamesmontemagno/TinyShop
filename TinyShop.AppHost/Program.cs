var builder = DistributedApplication.CreateBuilder(args);


var cache = builder.AddRedis("cache");

var products = builder.AddProject<Projects.Products>("products")
	.WithReference(cache);

builder.AddProject<Projects.Store>("store")
	.WithReference(products);

builder.Build().Run();

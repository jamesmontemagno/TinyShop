using Microsoft.EntityFrameworkCore;
using Products.Data;
using Products.Endpoints;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisOutputCache("redis");

// Add services to the container.
builder.Services.AddSingleton<RandomFailureMiddleware>();


builder.Services.AddDbContext<ProductDataContext>(options =>
	options.UseInMemoryDatabase("inmemproducts"));

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseMiddleware<RandomFailureMiddleware>();

app.MapProductEndpoints();

app.UseOutputCache();

app.UseStaticFiles();

app.CreateDbIfNotExists();

app.Run();

[DebuggerStepThrough]
public class RandomFailureMiddleware : IMiddleware
{
	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var path = context.Request.Path.Value;

		if (path is null || !path.Contains("api/Product", StringComparison.InvariantCultureIgnoreCase))
			return next(context);

		if (Random.Shared.NextDouble() >= 1.0)
		{
			throw new Exception("Computer says no.");
		}
		return next(context);
	}
}

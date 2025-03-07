using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using Products.Data;
using Products.Endpoints;
using System.ClientModel;
using System.Diagnostics;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisOutputCache("redis");

builder.AddOllamaApiClient("chat")
    .AddChatClient();

builder.Services.AddDbContext<ProductDataContext>(options =>
    options.UseInMemoryDatabase("inmemproducts"));

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

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

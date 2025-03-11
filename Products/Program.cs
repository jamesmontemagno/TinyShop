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

var credential = new ApiKeyCredential(builder.Configuration["GitHubModels:Token"]!);
var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.inference.ai.azure.com")
};

var ghModelsClient = new OpenAIClient(credential, openAIOptions);
var chatClient = ghModelsClient.AsChatClient("gpt-4o-mini");
builder.Services.AddSingleton(chatClient);

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
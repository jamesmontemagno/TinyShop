using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using Products.Data;
using Products.Endpoints;
using System.ClientModel;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisOutputCache("redis");

// Add services to the container.
builder.Services.AddSingleton<RandomFailureMiddleware>();

// Add AI services
builder.AddOllamaSharpChatClient("phi3");

var credential = new ApiKeyCredential(builder.Configuration["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token. See the README for details."));
var openAIOptions = new OpenAIClientOptions()
{
	Endpoint = new Uri("https://models.inference.ai.azure.com")
};

var ghModelsClient = new OpenAIClient(credential, openAIOptions);
var chatClient = ghModelsClient.AsChatClient("gpt-4o-mini");
builder.Services.AddChatClient(chatClient);

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

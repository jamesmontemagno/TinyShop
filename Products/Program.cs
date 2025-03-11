using System.ClientModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using OllamaSharp;
using OpenAI;
using OpenAI.VectorStores;
using Products.Data;
using Products.Endpoints;
using Products.Services;
using Products.Services.Ingestion;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisOutputCache("redis");

builder.AddOllamaApiClient("chat")
    .AddChatClient();

builder.Services.AddDbContext<ProductDataContext>(options =>
    options.UseInMemoryDatabase("inmemproducts"));

var credential = new ApiKeyCredential(builder.Configuration["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token. See the README for details."));
var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.inference.ai.azure.com")
};

var ghModelsClient = new OpenAIClient(credential, openAIOptions);
var chatClient = ghModelsClient.AsChatClient("gpt-4o-mini");
var embeddingGenerator = ghModelsClient.AsEmbeddingGenerator("text-embedding-3-small");

var vectorStore = new JsonVectorStore(Path.Combine(AppContext.BaseDirectory, "vector-store"));

builder.Services.AddSingleton<IVectorStore>(vectorStore);
builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();
builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();
builder.Services.AddEmbeddingGenerator(embeddingGenerator);

builder.Services.AddDbContext<IngestionCacheDbContext>(options =>
    options.UseSqlite("Data Source=ingestioncache.db"));

var app = builder.Build();
IngestionCacheDbContext.Initialize(app.Services);

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapProductEndpoints();

app.UseOutputCache();

app.UseStaticFiles();

app.CreateDbIfNotExists();

await DataIngestor.IngestDataAsync(
    app.Services,
    new PDFDirectorySource(Path.Combine(builder.Environment.WebRootPath, "data")));

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

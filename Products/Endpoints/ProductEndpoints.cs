using DataEntities;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Products.Data;
using Products.Services;
using System.Text.Json;

namespace Products.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Product");

        group.MapGet("/", [OutputCache] async (ProductDataContext db) =>
        {
            return await db.Product.ToListAsync();
        })
        .WithName("GetAllProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async (int id, ProductDataContext db) =>
        {
            return await db.Product.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Product model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async (int id, Product product, ProductDataContext db) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, product.Id)
                  .SetProperty(m => m.Name, product.Name)
                  .SetProperty(m => m.Description, product.Description)
                  .SetProperty(m => m.Price, product.Price)
                  .SetProperty(m => m.ImageUrl, product.ImageUrl)
                );

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/", async (Product product, ProductDataContext db) =>
        {
            db.Product.Add(product);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Product/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async (int id, ProductDataContext db) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // New chat endpoint
        group.MapPost("/chat", async (ChatRequest request, ProductDataContext db, IChatClient chatClient, SemanticSearch semanticSearch) =>
        {
            var products = await db.Product.ToListAsync();
            var productJson = JsonSerializer.Serialize(products, ProductSerializerContext.Default.ListProduct);

            var systemPrompt = @"You are a TinyShop assistant helping customers find outdoor products.
            Use emojis and HTML with Bootstrap classes in your responses. Please convert any Markdown to HTML. Include product
            images using full URLs like
            https://raw.githubusercontent.com/MicrosoftDocs/mslearn-dotnet-cloudnative/main/dotnet-docker/Products/wwwroot/images/product1.png
            and keep them small. Use media cards where possible.
            Products: " + productJson;

            var messages = new List<ChatMessage> { new ChatMessage(ChatRole.System, systemPrompt) };
            messages.AddRange(request.Messages);

            var response = await chatClient.GetResponseAsync(messages);

            if (response?.Message != null)
            {
                var lastMessageText = request.Messages.LastOrDefault()?.Text;
                if (string.IsNullOrEmpty(lastMessageText))
                {
                    return Results.BadRequest("Invalid request: No message text provided.");
                }

                var searchResults = await semanticSearch.SearchAsync(lastMessageText, null, 5);
                var responseMessage = response.Message.Text;

                foreach (var result in searchResults)
                {
                    var pdfLink = $"<a href='https://localhost:7130/data/{result.FileName}' class='btn btn-primary' target='_blank'><i class='bi bi-file-earmark-pdf'></i> {result.FileName}</a>";
                    responseMessage += $"<br/><br/>{result.Text} {pdfLink}";
                }

                return Results.Ok(new ChatResponse { Message = responseMessage });
            }

            return Results.BadRequest("Failed to generate response");
        })
        .WithName("ChatWithProducts")
        .Produces<ChatResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}

// DTOs for chat API
public class ChatRequest
{
    public List<ChatMessage> Messages { get; set; } = new();
}

public class ChatResponse
{
    public string? Message { get; set; }
}

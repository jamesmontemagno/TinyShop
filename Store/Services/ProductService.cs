using DataEntities;
using System.Text.Json;
using Microsoft.Extensions.AI;

namespace Store.Services;

public class ProductService
{
    private readonly HttpClient httpClient;
    private readonly string productEndpoint;

    public ProductService(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.productEndpoint = configuration["ProductEndpoint"] ?? throw new InvalidOperationException("ProductEndpoint is not set");
    }

    public async Task<List<Product>> GetProducts()
    {
        var json = await GetProductsJson();
        return JsonSerializer.Deserialize(json, ProductSerializerContext.Default.ListProduct) ?? new List<Product>();
    }

    public async Task<string> GetProductsJson()
    {
        var response = await httpClient.GetAsync($"{productEndpoint}/api/Product");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        return "[]";
    }

    public async Task<string?> SendChatMessage(List<ChatMessage> messages)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{productEndpoint}/api/Product/chat", new { Messages = messages });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
                return result?.Message;
            }
            return "Sorry, I couldn't process your request at this time.";
        }
        catch (Exception ex)
        {
            return $"Error communicating with the chat service: {ex.Message}";
        }
    }
}

// DTO to match the Products API response
public class ChatResponse
{
    public string? Message { get; set; }
}
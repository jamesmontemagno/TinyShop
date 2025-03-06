using DataEntities;
using System.Text.Json;

namespace Store.Services;

public class ProductService
{
    HttpClient httpClient;
    public ProductService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<Product>> GetProducts()
    {
        var json = await GetProductsJson();
        return JsonSerializer.Deserialize(json, ProductSerializerContext.Default.ListProduct) ?? new List<Product>();
    }

    public async Task<string> GetProductsJson()
    {
        var response = await httpClient.GetAsync("/api/Product");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        return "[]";
    }
}
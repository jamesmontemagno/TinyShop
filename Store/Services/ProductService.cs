using DataEntities;
using System.Text.Json;

namespace Store.Services;

public class ProductService
{
    private readonly HttpClient httpClient;
    
    public ProductService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<Product>> GetProducts()
    {
        List<Product>? products = null;
        var response = await httpClient.GetAsync("/api/Product");
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            products = await response.Content.ReadFromJsonAsync(ProductSerializerContext.Default.ListProduct);
        }

        return products ?? new List<Product>();
    }

    public async Task<Product?> CreateProduct(Product product)
    {
        var response = await httpClient.PostAsJsonAsync("/api/Product", product);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Product>();
        }
        return null;
    }

    public async Task<bool> UpdateProduct(int id, Product product)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/Product/{id}", product);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var response = await httpClient.DeleteAsync($"/api/Product/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<Product?> GetProduct(int id)
    {
        var response = await httpClient.GetAsync($"/api/Product/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Product>();
        }
        return null;
    }
}
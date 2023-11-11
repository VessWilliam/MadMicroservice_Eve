using MadMicro.Services.OrderAPI.Models.DTO;
using MadMicro.Services.OrderAPI.Services.IService;
using Newtonsoft.Json;

namespace MadMicro.Services.OrderAPI.Services.Service;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<ProductDTO>> GetProducts()
    {
        var client = _httpClientFactory.CreateClient("Product");
        var response = await client.GetAsync($"/api/product");
        var apiContent = await response.Content.ReadAsStringAsync();
        var responseContent = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
        if (responseContent.IsSuccess)
            return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(
                responseContent.Result.ToString());
        return new List<ProductDTO>();
    }
}

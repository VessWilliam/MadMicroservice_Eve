using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using static MadMicro.Web.Utility.StaticDetail;

namespace MadMicro.Web.Services.Service;

public class ProductService : IProductService
{
    private readonly IBaseService _baseService;

    public ProductService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDTO?> CreateProductAsync(ProductDTO productDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.POST,
            Data = productDTO,
            Url = $"{ProductAPIBase}/api/product",
            ContentType = ContentType.MultipartFromData
        });
    }

    public async Task<ResponseDTO?> DeleteProductAsync(int id)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.DELETE,
            Url = $"{ProductAPIBase}/api/product/{id}"
        });
    }

    public async Task<ResponseDTO?> GetAllProductsAsync()
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.GET,
            Url = $"{ProductAPIBase}/api/product"
        });
    }

    public async Task<ResponseDTO?> GetAllProductsByIdAsync(int Id)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.GET,
            Url = $"{ProductAPIBase}/api/product/{Id}"
        });
    }

    public async Task<ResponseDTO?> UpdateProductAsync(ProductDTO productDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.PUT,
            Data = productDTO,
            Url = $"{ProductAPIBase}/api/product",
            ContentType = ContentType.MultipartFromData
        });
    }
}

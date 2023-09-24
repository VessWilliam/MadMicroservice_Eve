using MadMicro.Web.Models;

namespace MadMicro.Web.Services.IService;

public interface IProductService
{
    Task<ResponseDTO?> GetAllProductsAsync();
    Task<ResponseDTO?> GetAllProductsByIdAsync(int Id);
    Task<ResponseDTO?> CreateProductAsync(ProductDTO productDTO);
    Task<ResponseDTO?> UpdateProductAsync(ProductDTO productDTO);
    Task<ResponseDTO?> DeleteProductAsync(int id);
}

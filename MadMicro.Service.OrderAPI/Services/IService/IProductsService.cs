using MadMicro.Services.OrderAPI.Models.DTO;

namespace MadMicro.Services.OrderAPI.Services.IService;

public interface IProductsService
{

    Task<IEnumerable<ProductDTO>> GetProducts();


}

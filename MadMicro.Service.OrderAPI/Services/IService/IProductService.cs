using MadMicro.Services.OrderAPI.Models.DTO;

namespace MadMicro.Services.OrderAPI.Services.IService;

public interface IProductService
{

    Task<IEnumerable<ProductDTO>> GetProducts();


}

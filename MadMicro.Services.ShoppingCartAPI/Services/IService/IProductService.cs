using MadMicro.Services.ShoppingCartAPI.Models.DTO;

namespace MadMicro.Services.ShoppingCartAPI.Services.IService;

public interface IProductService
{

    Task<IEnumerable<ProductDTO>> GetProducts();


}

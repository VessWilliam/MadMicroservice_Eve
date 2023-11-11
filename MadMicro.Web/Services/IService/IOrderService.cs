using MadMicro.Web.Models;

namespace MadMicro.Web.Services.IService;

public interface IOrderService
{
    Task<ResponseDTO> CreateOrderAsync(CartDTO cartDTO);    

}

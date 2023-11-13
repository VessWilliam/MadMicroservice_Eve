using MadMicro.Web.Models;

namespace MadMicro.Web.Services.IService;

public interface IOrderService
{
    Task<ResponseDTO?> CreateOrderAsync(CartDTO cartDTO);
                    
    Task<ResponseDTO?> CreateStripeSessionAsync(StripeRequestDTO stripeRequestDTO);
                    
    Task<ResponseDTO?> ValidateStripeSessionAsync(int orderHeaderId);

}

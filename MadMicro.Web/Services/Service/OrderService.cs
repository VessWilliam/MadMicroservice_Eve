using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using static MadMicro.Web.Utility.StaticDetail;

namespace MadMicro.Web.Services.Service;

public class OrderService : IOrderService
{
    private readonly IBaseService _baseService; 

    public OrderService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDTO?> CreateOrderAsync(CartDTO cartDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.POST,
            Data = cartDTO,
            Url = $"{OrderAPIBase}/api/order/CreateOrder"
        });
    }

    public async Task<ResponseDTO?> CreateStripeSessionAsync(StripeRequestDTO stripeRequestDTO)
    {

        return await _baseService.SendAsync(new RequestDTO() 
        {
           ApiType = ApiType.POST,
           Data = stripeRequestDTO, 
           Url = $"{OrderAPIBase}/api/order/CreateStripeSession"

        });
    }
}

using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using static MadMicro.Web.Utility.StaticDetail;

namespace MadMicro.Web.Services.Service;

public class OrderService : IOrderService
{
    private readonly IBaseService _baseService;
    private readonly IOrderService _orderService;   

    public OrderService(IBaseService baseService, 
        IOrderService orderService)
    {
        _baseService = baseService;
        _orderService = orderService;   
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
}

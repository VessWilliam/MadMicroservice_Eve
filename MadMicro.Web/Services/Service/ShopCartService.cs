using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using static MadMicro.Web.Utility.StaticDetail;

namespace MadMicro.Web.Services.Service;

public class ShopCartService : IShopCartService
{
    private readonly IBaseService _baseService;

    public ShopCartService(IBaseService baseService)
    {
        _baseService = baseService;
    }


    public async Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.POST,
            Data = cartDTO,
            Url = $"{ShopCartAPIBase}/api/cart/ApplyCoupon"

        });
    }

    public async Task<ResponseDTO?> EmailCart(CartDTO cartDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        { 
            ApiType = ApiType.POST,
            Data = cartDTO,
            Url = $"{ShopCartAPIBase}/api/cart/EmailCartRequest"
        });
    }

    public async Task<ResponseDTO?> GetCartByUserIdAsync(string userId)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.GET,
            Url = $"{ShopCartAPIBase}/api/cart/GetCart/{userId}"
        });
    }

    public async Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.POST,
            Data = cartDetailsId,
            Url = $"{ShopCartAPIBase}/api/cart/RemoveCart"
        });

    }

    public async Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.POST,
            Data = cartDTO,
            Url = $"{ShopCartAPIBase}/api/cart/CartUpsert"
        });
    }
}

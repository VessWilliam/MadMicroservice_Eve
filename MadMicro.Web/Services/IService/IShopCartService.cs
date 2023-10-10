using MadMicro.Web.Models;

namespace MadMicro.Web.Services.IService;

public interface IShopCartService
{
    Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId);
    Task<ResponseDTO?> GetCartByUserIdAsync(string userId);
    Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO);
    Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO);
    Task<ResponseDTO?> EmailCart(CartDTO cartDTO);
}

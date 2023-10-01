using MadMicro.Services.ShoppingCartAPI.Models.DTO;

namespace MadMicro.Services.ShoppingCartAPI.Services.IService;

public interface ICouponService
{

    Task<CouponDTO> GetCoupon(string couponCode);


}

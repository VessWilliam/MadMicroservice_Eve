using MadMicro.Web.Models;

namespace MadMicro.Web.Services.IService
{
    public interface ICouponService
    {
        Task<ResponseDTO?> GetCouponByCode(string CouponCode);
        Task<ResponseDTO?> GetAllCouponAsync();
        Task<ResponseDTO?> GetCouponByIdAsync(int Id);
        Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDTO);
        Task<ResponseDTO?> UpdateCouponAsync(CouponDTO couponDTO);
        Task<ResponseDTO?> DeleteCouponAsync(int id);
    }
}

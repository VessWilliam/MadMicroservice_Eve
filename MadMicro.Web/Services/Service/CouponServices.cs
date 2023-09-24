using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using static MadMicro.Web.Utility.StaticDetail;

namespace MadMicro.Web.Services.Service;

public class CouponServices : ICouponService
{
    private readonly IBaseService _baseService;
    public CouponServices(IBaseService baseService)
    {
        _baseService = baseService;
    }
    public async Task<ResponseDTO?> GetAllCouponAsync()
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.GET,
            Url =  $"{CouponAPIBase}/api/coupon"
        });
    }

    public async Task<ResponseDTO?> GetCouponByCode(string CouponCode)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType= ApiType.GET,
            Url = $"{CouponAPIBase}/api/coupon/GetByCode/{CouponCode}"
        });
    }

    public async Task<ResponseDTO?> GetCouponByIdAsync(int Id)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.GET,
            Url = $"{CouponAPIBase}/api/coupon/{Id}"
        });
    }

    public async Task<ResponseDTO?> DeleteCouponAsync(int Id)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.DELETE,
            Url = $"{CouponAPIBase}/api/coupon/{Id}"
        });
    }


    public async Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.POST,
            Data=couponDTO,
            Url = $"{CouponAPIBase}/api/coupon"
        });
    }

    public async Task<ResponseDTO?> UpdateCouponAsync(CouponDTO couponDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = ApiType.PUT,
            Data = couponDTO,
            Url = $"{CouponAPIBase}/api/coupon"
        });
    }
}

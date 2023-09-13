using AutoMapper;
using MadMicro.Services.CouponAPI.Models;
using MadMicro.Services.CouponAPI.Models.DTO;

namespace MadMicro.Services.CouponAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<CouponDTO, Coupon>();
            config.CreateMap<Coupon, CouponDTO>();
        });
        return mappingConfig;   
    }
}

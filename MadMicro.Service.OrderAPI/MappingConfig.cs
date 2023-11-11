using AutoMapper;
using MadMicro.Services.OrderAPI.Models;
using MadMicro.Services.OrderAPI.Models.DTO;

namespace MadMicro.Services.OrderAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
           config.CreateMap<OrderHeaderDTO, CartHeadersDTO>()
            .ForMember(d => d.CartTotal, sd => sd.MapFrom(i => i.OrderTotal)).ReverseMap();

            config.CreateMap<CartDetailsDTO, OrderDetailsDTO>()
             .ForMember(d => d.ProductName, sd => sd.MapFrom(i => i.Product.Name))
             .ForMember(d => d.Price, sd => sd.MapFrom(i => i.Product.Price));

            config.CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
            config.CreateMap<OrderDetails, OrderDetailsDTO>().ReverseMap();


        });
        return mappingConfig;
    }
}

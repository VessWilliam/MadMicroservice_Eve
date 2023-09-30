using AutoMapper;
using MadMicro.Services.ShoppingCartAPI.Models;
using MadMicro.Services.ShoppingCartAPI.Models.DTO;

namespace MadMicro.Service.ShoppingCartAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<CartHeaders, CartHeadersDTO>().ReverseMap();
            config.CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();

        });
        return mappingConfig;
    }
}

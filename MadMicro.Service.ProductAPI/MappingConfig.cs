using AutoMapper;
using MadMicro.Services.ProductAPI.Models;
using MadMicro.Services.ProductAPI.Models.DTO;

namespace MadMicro.Services.ProductAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
			config.CreateMap<ProductDTO, Product>().ReverseMap();
        });
        return mappingConfig;
    }
}

using AutoMapper;
using MadMicro.Service.ProductAPI.Models;
using MadMicro.Service.ProductAPI.Models.DTO;

namespace MadMicro.Service.ProductAPI;

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

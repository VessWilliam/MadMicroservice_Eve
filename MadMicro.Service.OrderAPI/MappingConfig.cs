using AutoMapper;


namespace MadMicro.Services.OrderAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
           

        });
        return mappingConfig;
    }
}

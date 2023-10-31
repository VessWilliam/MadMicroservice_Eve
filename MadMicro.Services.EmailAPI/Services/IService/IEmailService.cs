using MadMicro.Services.EmailAPI.Models.DTO;

namespace MadMicro.Services.EmailAPI.Services.IService;

public interface IEmailService
{
    Task EmailCartAndLog(CartDTO cartDTO);
}

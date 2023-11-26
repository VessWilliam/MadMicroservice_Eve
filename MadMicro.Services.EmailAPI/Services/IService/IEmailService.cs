using MadMicro.Services.EmailAPI.Message;
using MadMicro.Services.EmailAPI.Models.DTO;

namespace MadMicro.Services.EmailAPI.Services.IService;

public interface IEmailService
{
    Task EmailCartAndLog(CartDTO cartDTO);
    Task RegisterUserEmailAndLog(string email);

    Task LogOrderPlaced(OrderConfirmation rewardMessage);
}
